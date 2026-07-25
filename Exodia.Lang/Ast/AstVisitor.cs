using LLVMSharp.Interop;

namespace Exodia.Lang.Ast;


public interface IAstVisitor<T>
{
    T VisitProgram(ProgramNode node);
    T VisitFnDeclaration(FnDeclaration node);
    T VisitVariableDeclaration(VariableDeclaration node);
    T VisitNameRef(NameRef node);
    T VisitBlock(Block node);
    T VisitIf(IfStatement node);
    T VisitReturn(ReturnStatement node);
    T VisitAssignmentExpr(AssignExpr node);
    T VisitExpressionStatement(ExpressionStatement node);
    T VisitCast(CastExpr node);
    T VisitCall(CallExpr node);
    T VisitBinary(BinaryExpr node);
    T VisitUnary(UnaryExpr node);
    T VisitWhile(WhileStatement node);
    T VisitIntLiteral(IntLiteral node);
    T VisitFloatLiteral(FloatLiteral node);
    T VisitBoolLiteral(BoolLiteral node);
    T VisitNew(NewExpr node);
    T VisitFieldAccess(FieldAccess node);
    T VisitMethodCall(MethodCall node);
    T VisitThis(ThisExpr node);
}

public class AstVisitor : IAstVisitor<LLVMValueRef>
{
    public readonly LLVMModuleRef _module;
    public readonly LLVMBuilderRef _builder;
    private readonly Dictionary<string, Symbol> _symbols = [];
    private readonly Dictionary<CallableKey, Callable> _functions = [];
    private readonly Dictionary<string, StructInfo> _structs = [];
    private readonly Dictionary<CallableKey, Callable> _constructors = [];
    private readonly Dictionary<CallableKey, Callable> _methods = [];
    private readonly Dictionary<string, InterfaceDeclaration> _interfaces = [];   // captured for dyn (increment B); no IR
    private readonly Dictionary<VTableKey, LLVMValueRef> _vtables = [];
    private readonly Dictionary<string, LLVMTypeRef> _dynTypes = [];
    private readonly Dictionary<CallableKey, FnDeclaration> _fnTemplates = [];
    private readonly Dictionary<string, Callable> _instances = [];
    private Dictionary<string, LLVMTypeRef> _activeSubstitutionEnv = [];
    private readonly Dictionary<string, StructDeclaration> _structTemplates = [];   // generic structs: parked until instantiated
    // struct declaration is builder-free (safe eagerly), but body emission needs the builder,
    // so instantiated structs queue their ctor/method bodies here for the post-body drain phase.
    private readonly List<(StructDeclaration Concrete, Dictionary<string, LLVMTypeRef> Env)> _pendingStructBodies = [];
    // reverse map: mangled instance name ("Box$i32") -> what template + type-args produced it.
    // lets structural inference recover T from a concrete argument of type %Box$i32.
    private readonly Dictionary<string, (StructDeclaration Template, Dictionary<string, LLVMTypeRef> Env)> _structInstances = [];
    // generic methods: parked keyed by (owner, name, arity). StructEnv is the owner's substitution
    // at park time ({} for a plain struct, {T->i32} for Box$i32) -- composed with the method's own
    // {U->...} at call time. Owner may be a mangled struct name or an impl target type.
    private readonly Dictionary<CallableKey, (MethodDeclaration Method, Dictionary<string, LLVMTypeRef> StructEnv)> _methodTemplates = [];
    
    
    public AstVisitor(LLVMModuleRef module)
    {
        _module = module;
        _builder = _module.Context.CreateBuilder();
    }

    public LLVMValueRef VisitProgram(ProgramNode node)
    {
        foreach (var s in node.Structs)                               // phase 1: struct types + ctor/method decls
            if (s.TypeParams.Count > 0) _structTemplates[s.Name] = s; //   generic -> park (no LLVM layout until T known)
            else RegisterStruct(s);
        foreach (var i in node.Interfaces) _interfaces[i.Name] = i;   // phase 1b: capture interfaces (dyn/B; no IR)
        foreach (var impl in node.Impls) DeclareImpl(impl);           // phase 1c: impl method signatures
        foreach (var impl in node.Impls) EmitVTable(impl);
        
        // We cannot emit a generic fn as LLVM cannot represent it
        foreach (var fn in node.Functions)
            if (fn.TypeParams.Count > 0)
            {
                var key = new CallableKey("", fn.Name, fn.Params.Count);
                _fnTemplates[key] = fn;
            }
        var concreteFns = node.Functions
            .Where(fn => fn.TypeParams.Count == 0)
            .ToList();
        
        foreach (var fn in concreteFns) DeclareFunction(fn);       // phase 2: function signatures
        foreach (var fn in concreteFns) fn.Accept(this);           // phase 3a: function bodies
        foreach (var s in node.Structs.Where(s => s.TypeParams.Count == 0))
            EmitStructBodies(s);                                     // phase 3b: concrete struct ctor/method bodies
        foreach (var impl in node.Impls) EmitImpl(impl);              // phase 3c: impl method bodies
        DrainStructBodies();                                          // phase 3d: emit instantiated generic-struct bodies
        return default;
    }

    // A stable, symbol-safe spelling for a type in a mangled name. Primitives -> "i32"/"double";
    // named structs -> their StructName ("Circle", "Box$i32"), NOT LLVMTypeRef.ToString() which
    // for a struct returns the whole "%Circle = type { i32 }" definition.
    private static string MangleType(LLVMTypeRef t) =>
        t.Kind == LLVMTypeKind.LLVMStructTypeKind ? t.StructName : t.ToString();

    public static string Mangle(FnDeclaration template, Dictionary<string, LLVMTypeRef> env) =>
        $"{template.Name}${template.Params.Count}${string.Join("$", template.TypeParams.Select(tp => MangleType(env[tp.Name])))}";

    // Expansion of the fn template into an actual fn
    private Callable Instantiate(FnDeclaration template, Dictionary<string, LLVMTypeRef> substitutionEnv)
    {
        var mangled = Mangle(template, substitutionEnv); // "id$1$i32" -- unique name for THIS instantiation 
        if (_instances.TryGetValue(mangled, out var cached))
            return cached; // already built -> then reuse

        // This is now the concrete version of the fn
        var concrete = template with { Name = mangled, TypeParams = [] };

        // IMPORTANT : When we do this we are in the middle of a call somewhere so we need to save our
        // current LLVM builder state so we can restore it later.
        var savedBlock = _builder.InsertBlock;
        var savedSymbols = new Dictionary<string, Symbol>(_symbols);
        var savedEnv = _activeSubstitutionEnv;
        
        _activeSubstitutionEnv = substitutionEnv;
        DeclareFunction(concrete);
        var callableKey = new CallableKey("", mangled, concrete.Params.Count);
        var instance = _functions[callableKey];
        _instances[mangled] = instance;
        concrete.Accept(this);

        _activeSubstitutionEnv = savedEnv;
        _symbols.Clear();
        foreach (var kv in savedSymbols)
        {
            _symbols[kv.Key] = kv.Value;
        }
        if (savedBlock.Handle != IntPtr.Zero)
            _builder.PositionAtEnd(savedBlock);

        return instance;
    }

    public static string MangleStruct(StructDeclaration template, Dictionary<string, LLVMTypeRef> env) =>
        $"{template.Name}${string.Join("$", template.TypeParams.Select(tp => MangleType(env[tp.Name])))}";

    // Turn a parked generic struct into a concrete one under `env` (T -> i32).
    // Only DECLARES here (named type + field layout + ctor/method signatures) -- builder-free,
    // so it is safe to call from any phase, including while resolving another type. Bodies are
    // queued and emitted later by DrainStructBodies (they need the builder).
    private StructInfo InstantiateStruct(StructDeclaration template, Dictionary<string, LLVMTypeRef> env)
    {
        var mangled = MangleStruct(template, env);
        if (_structs.TryGetValue(mangled, out var cached))
            return cached;                                   // instantiate each (struct, type-args) once

        var concrete = template with { Name = mangled, TypeParams = [] };

        var savedEnv = _activeSubstitutionEnv;
        _activeSubstitutionEnv = env;
        RegisterStruct(concrete);                            // %Box$i32 + field layout + ctor/method signatures
        _activeSubstitutionEnv = savedEnv;

        _structInstances[mangled] = (template, env);         // record for structural inference
        _pendingStructBodies.Add((concrete, env));           // emit ctor/method bodies in the drain phase
        return _structs[mangled];
    }

    private void DrainStructBodies()
    {
        // Emitting one struct's bodies may instantiate more structs (nested `new`), which append
        // to the queue -- so loop until it stops growing.
        while (_pendingStructBodies.Count > 0)
        {
            var (concrete, env) = _pendingStructBodies[0];
            _pendingStructBodies.RemoveAt(0);
            var savedEnv = _activeSubstitutionEnv;
            _activeSubstitutionEnv = env;
            EmitStructBodies(concrete);
            _activeSubstitutionEnv = savedEnv;
        }
    }

    // Match a declared parameter type against a concrete argument's LLVM type, binding any of
    // `ours` (this template's type params) that appear. Handles bare `T` and nested `Box<T>`.
    private void Unify(TypeRef paramType, LLVMTypeRef argType, HashSet<string> ours, Dictionary<string, LLVMTypeRef> env)
    {
        switch (paramType)
        {
            case NamedType n when ours.Contains(n.Name):                        // x: T           -> T = argType
                env[n.Name] = argType;
                break;
            case GenericType g when _structInstances.TryGetValue(argType.StructName, out var inst) && inst.Template.Name == g.Name:
                // x: Box<T> matched against a %Box$i32 arg -> recover Box's env, then unify each arg.
                for (var i = 0; i < g.TypeArgs.Count; i++)
                    Unify(g.TypeArgs[i], inst.Env[inst.Template.TypeParams[i].Name], ours, env);
                break;
            // concrete param (e.g. int32) or non-inferable shape: nothing to bind.
        }
    }

    // An `impl I for T { … }` just adds methods to T -- reuse the struct-method machinery.
    private void DeclareImpl(ImplDeclaration impl)
    {
        var structType = _structs[impl.TargetType].Type;
        foreach (var method in impl.Methods)
            DeclareOrParkMethod(impl.TargetType, structType, method);
    }

    private void EmitImpl(ImplDeclaration impl)
    {
        foreach (var method in impl.Methods)
            if (method.TypeParams.Count == 0)                                          // generic methods emit at call time
                EmitMethod(impl.TargetType, method);
    }

    private void EmitVTable(ImplDeclaration impl)
    {
        var slotOrder = _interfaces[impl.InterfaceName].Methods;
        var ptrType = LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0);
        var slots = slotOrder
            .Select(sig =>
            {
                var callableKey = new CallableKey(impl.TargetType, sig.Name, sig.Params.Count);
                return _methods[callableKey].Fn;
            })
            .ToArray();
        var init = LLVMValueRef.CreateConstArray(ptrType, slots);
        var arrayType = LLVMTypeRef.CreateArray(ptrType, (uint)slots.Length);
        var global = _module.AddGlobal(arrayType, $"{impl.TargetType}.{impl.InterfaceName}.vtable");
        global.Initializer = init;
        global.IsGlobalConstant = true;
        var vtableKey = new VTableKey(impl.TargetType, impl.InterfaceName);
        _vtables[vtableKey] = global;
    }
    
    private LLVMTypeRef GetDynType(string interfaceName)
    {
        if (_dynTypes.TryGetValue(interfaceName, out var t)) 
            return t;
        var ptrType = LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0);
        var dyn = _module.Context.CreateNamedStruct($"dyn.{interfaceName}");
        dyn.StructSetBody([ptrType, ptrType], false);   // { data ptr, vtable ptr }
        _dynTypes[interfaceName] = dyn;
        return dyn;
    }

    private LLVMValueRef EmitDynCall(LLVMValueRef fatPtrAddr, LLVMTypeRef dynType, MethodCall node)
    {
        var ptrType = LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0);
        var interfaceName = dynType.StructName["dyn.".Length..];          // "%dyn.IShape".StructName -> "IShape"
        var iface = _interfaces[interfaceName];

        // (a) which slot is this method? -- same interface order the vtable was built with
        var slot = iface.Methods
            .Select((m, i) => (m, i))
            .First(x => x.m.Name == node.MethodName && x.m.Params.Count == node.Args.Count);

        // (b) pull the two halves out of the fat pointer { data, vtable }
        var dataPtr = _builder.BuildLoad2(ptrType, _builder.BuildStructGEP2(dynType, fatPtrAddr, 0, "dyn.data.ptr"), "dyn.data");
        var vtable  = _builder.BuildLoad2(ptrType, _builder.BuildStructGEP2(dynType, fatPtrAddr, 1, "dyn.vtable.ptr"), "dyn.vtable");

        // (c) load the function pointer from vtable[slot]
        var llvmValueRefs = new[]
        {
            LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, (ulong)slot.i)
        };
        var slotAddr = _builder.BuildInBoundsGEP2(ptrType, vtable, llvmValueRefs, "vtable.slot");
        var fnPtr    = _builder.BuildLoad2(ptrType, slotAddr, "fn");

        // (d) rebuild the callee signature from the interface method: retType (ptr this, ...params)
        var paramTypes = new LLVMTypeRef[slot.m.Params.Count + 1];
        paramTypes[0] = ptrType;                                          // this
        for (var i = 0; i < slot.m.Params.Count; i++)
            paramTypes[i + 1] = ResolveType(slot.m.Params[i].Type);
        var fnSig = LLVMTypeRef.CreateFunction(ResolveType(slot.m.ReturnType), paramTypes);

        // (e) indirect call: fnPtr(dataPtr as this, ...args)
        var args = new[] { dataPtr }
            .Concat(node.Args.Select(a => a.Accept(this)))
            .ToArray();
        return _builder.BuildCall2(fnSig, fnPtr, args, "dyn.call");
    }

    // --- structs ---
    private void RegisterStruct(StructDeclaration node)
    {
        var fieldTypes = new List<LLVMTypeRef>();
        var fields = new Dictionary<string, StructInfoField>();
        uint index = 0;
        foreach (var f in node.Fields)
        {
            var t = ResolveType(f.Type);
            fields[f.Name] = new StructInfoField(index, t);
            fieldTypes.Add(t);
            index++;
        }
        var structType = _module.Context.CreateNamedStruct(node.Name);
        structType.StructSetBody(fieldTypes.ToArray(), false);
        _structs[node.Name] = new StructInfo(structType, fields);

        foreach (var ctor in node.Constructors) DeclareConstructor(node.Name, structType, ctor);
        foreach (var method in node.Methods) DeclareOrParkMethod(node.Name, structType, method);
    }

    // A generic method (`fn echo<U>(...)`) has no LLVM signature until U is known, so park it as a
    // template capturing the owner's current substitution env (the struct's {T->...}); a plain
    // method is declared immediately.
    private void DeclareOrParkMethod(string owner, LLVMTypeRef structType, MethodDeclaration method)
    {
        if (method.TypeParams.Count > 0)
            _methodTemplates[new CallableKey(owner, method.Name, method.Params.Count)] =
                (method, new Dictionary<string, LLVMTypeRef>(_activeSubstitutionEnv));
        else
            DeclareMethod(owner, structType, method);
    }

    private void DeclareConstructor(string structName, LLVMTypeRef structType, ConstructorDeclaration ctor)
    {
        var name = ctor.Name ?? "";
        var key = new CallableKey(structName, name, ctor.Params.Count);
        if (_constructors.ContainsKey(key))
            throw new NotSupportedException($"'{structName}' already has constructor '{name}' with {ctor.Params.Count} params");
        var paramTypes = ctor.Params.Select(p => ResolveType(p.Type)).ToArray();
        var sig = LLVMTypeRef.CreateFunction(structType, paramTypes);                 // returns the struct by value
        var mangled = $"{structName}.{(name == "" ? "ctor" : name)}.{ctor.Params.Count}";
        _constructors[key] = new Callable(_module.AddFunction(mangled, sig), sig);
    }

    private void DeclareMethod(string structName, LLVMTypeRef structType, MethodDeclaration method)
    {
        var paramTypes = new LLVMTypeRef[method.Params.Count + 1];
        paramTypes[0] = LLVMTypeRef.CreatePointer(structType, 0);                     // `this` by pointer
        for (var i = 0; i < method.Params.Count; i++)
            paramTypes[i + 1] = ResolveType(method.Params[i].Type);
        var sig = LLVMTypeRef.CreateFunction(ResolveType(method.ReturnType), paramTypes);
        _methods[new CallableKey(structName, method.Name, method.Params.Count)] =
            new Callable(_module.AddFunction($"{structName}.{method.Name}.{method.Params.Count}", sig), sig);
    }

    private void EmitStructBodies(StructDeclaration node)
    {
        foreach (var ctor in node.Constructors) EmitConstructor(node.Name, ctor);
        foreach (var method in node.Methods)
            if (method.TypeParams.Count == 0)                                          // generic methods emit at call time
                EmitMethod(node.Name, method);
    }

    private void EmitConstructor(string structName, ConstructorDeclaration ctor)
    {
        var structType = _structs[structName].Type;
        var fn = _constructors[new CallableKey(structName, ctor.Name ?? "", ctor.Params.Count)].Fn;
        _builder.PositionAtEnd(fn.AppendBasicBlock("entry"));
        _symbols.Clear();
        var thisSlot = _builder.BuildAlloca(structType, "this");                      // ctor allocates `this`
        _symbols["this"] = new Symbol(thisSlot, structType);
        BindParams(fn, ctor.Params, 0);
        ctor.Body.Accept(this);
        _builder.BuildRet(_builder.BuildLoad2(structType, thisSlot, "thisval"));      // implicit `return this`
    }

    private void EmitMethod(string structName, MethodDeclaration method)
    {
        var structType = _structs[structName].Type;
        var fn = _methods[new CallableKey(structName, method.Name, method.Params.Count)].Fn;
        _builder.PositionAtEnd(fn.AppendBasicBlock("entry"));
        _symbols.Clear();
        _symbols["this"] = new Symbol(fn.GetParam(0), structType);                    // `this` is the incoming pointer
        BindParams(fn, method.Params, 1);
        method.Body.Accept(this);
    }

    private void BindParams(LLVMValueRef fn, IReadOnlyList<Param> parameters, uint offset)
    {
        for (var i = 0; i < parameters.Count; i++)
        {
            var t = ResolveType(parameters[i].Type);
            var slot = _builder.BuildAlloca(t, parameters[i].Name);
            _builder.BuildStore(fn.GetParam((uint)i + offset), slot);
            _symbols[parameters[i].Name] = new Symbol(slot, t);
        }
    }

    public LLVMValueRef VisitNew(NewExpr node)
    {
        var args = node.Args.Select(a => a.Accept(this)).ToArray();
        var ctorName = node.ConstructorName ?? "";

        if (!_structs.TryGetValue(node.StructName, out var info))
        {
            if (!_structTemplates.TryGetValue(node.StructName, out var template))
                throw new NotSupportedException($"unknown struct '{node.StructName}'");
            if (node.TypeArgs.Count == 0)
                throw new NotSupportedException(
                    $"generic struct '{node.StructName}' requires explicit type arguments, e.g. new {node.StructName}<...>(...)");
            info = InstantiateStruct(template, BuildStructEnv(template, node.TypeArgs));
        }
        var structName = info.Type.StructName;                                        // "Vec" or mangled "Box$i32"

        if (_constructors.TryGetValue(new CallableKey(structName, ctorName, args.Length), out var ctor))
            return _builder.BuildCall2(ctor.Signature, ctor.Fn, args, $"{structName}.new");
        if (ctorName == "" && args.Length == info.Fields.Count)                       // positional fallback
        {
            var value = info.Type.Undef;
            for (var i = 0; i < args.Length; i++)
                value = _builder.BuildInsertValue(value, args[i], (uint)i, $"{structName}.f{i}");
            return value;
        }
        throw new NotSupportedException($"'{structName}' has no constructor '{ctorName}' taking {args.Length} args");
    }

    public LLVMValueRef VisitFieldAccess(FieldAccess node)
    {
        var (ptr, field) = ResolveField(node);
        return _builder.BuildLoad2(field.Type, ptr, node.FieldName);
    }

    // Infer a generic method's own type args ({U->...}) from its argument types. Same bare +
    // structural unification as functions/structs; the method's params, not the receiver.
    private Dictionary<string, LLVMTypeRef> InferMethodTypeArgs(MethodDeclaration template, LLVMTypeRef[] argTypes)
    {
        var names = template.TypeParams.Select(tp => tp.Name).ToHashSet();
        var env = new Dictionary<string, LLVMTypeRef>();
        for (var i = 0; i < template.Params.Count; i++)
            Unify(template.Params[i].Type, argTypes[i], names, env);
        foreach (var tp in template.TypeParams)
            if (!env.ContainsKey(tp.Name))
                throw new NotSupportedException($"could not infer type argument '{tp.Name}' for method '{template.Name}'");
        return env;
    }

    // Emit a concrete instance of a generic method under the COMPOSED env (struct's {T->...} plus
    // the method's {U->...}). Reuses DeclareMethod/EmitMethod -- the concrete method has a mangled
    // name and no type params, so it is just an ordinary method to that machinery.
    private Callable InstantiateMethod(string owner, LLVMTypeRef structType, MethodDeclaration template,
        Dictionary<string, LLVMTypeRef> structEnv, Dictionary<string, LLVMTypeRef> methodEnv)
    {
        var mangledName = $"{template.Name}${string.Join("$", template.TypeParams.Select(tp => MangleType(methodEnv[tp.Name])))}";
        var key = new CallableKey(owner, mangledName, template.Params.Count);
        if (_methods.TryGetValue(key, out var cached))
            return cached;                                   // instantiate each (method, type-args) once

        var concrete = template with { Name = mangledName, TypeParams = [] };
        var composed = new Dictionary<string, LLVMTypeRef>(structEnv);
        foreach (var kv in methodEnv) composed[kv.Key] = kv.Value;    // method params win over struct params on clash

        // instantiation happens mid-emission of the caller -> save/restore builder + symbols + env.
        var savedBlock = _builder.InsertBlock;
        var savedSymbols = new Dictionary<string, Symbol>(_symbols);
        var savedEnv = _activeSubstitutionEnv;

        _activeSubstitutionEnv = composed;
        DeclareMethod(owner, structType, concrete);          // adds _methods[key]
        var instance = _methods[key];                        // cache-before-body: recursive self-calls resolve here
        EmitMethod(owner, concrete);

        _activeSubstitutionEnv = savedEnv;
        _symbols.Clear();
        foreach (var kv in savedSymbols) _symbols[kv.Key] = kv.Value;
        if (savedBlock.Handle != IntPtr.Zero) _builder.PositionAtEnd(savedBlock);

        return instance;
    }

    public LLVMValueRef VisitMethodCall(MethodCall node)
    {
        var (receiverPtr, structType) = ResolveStructPointer(node.Receiver);
        if (structType.StructName.StartsWith("dyn."))
            return EmitDynCall(receiverPtr, structType, node);
        var structName = structType.StructName;
        var args = new LLVMValueRef[node.Args.Count + 1];
        args[0] = receiverPtr;                                                        // `this`
        for (var i = 0; i < node.Args.Count; i++)
            args[i + 1] = node.Args[i].Accept(this);
        var key = new CallableKey(structName, node.MethodName, node.Args.Count);
        if (_methods.TryGetValue(key, out var m))                                     // concrete or already-instantiated
            return _builder.BuildCall2(m.Signature, m.Fn, args, $"{structName}.{node.MethodName}.call");
        if (_methodTemplates.TryGetValue(key, out var template))                      // generic method: infer -> instantiate
        {
            var methodEnv = InferMethodTypeArgs(template.Method, args.Skip(1).Select(a => a.TypeOf).ToArray());
            var instance = InstantiateMethod(structName, structType, template.Method, template.StructEnv, methodEnv);
            return _builder.BuildCall2(instance.Signature, instance.Fn, args, $"{structName}.{node.MethodName}.call");
        }
        throw new NotSupportedException($"struct '{structName}' has no method '{node.MethodName}' taking {node.Args.Count} args");
    }

    public LLVMValueRef VisitThis(ThisExpr node)
    {
        var self = _symbols["this"];
        return _builder.BuildLoad2(self.Type, self.Slot, "this");
    }

    // Field access needs the base struct's ADDRESS (to GEP), not a loaded value.
    private (LLVMValueRef Ptr, StructInfoField Field) ResolveField(FieldAccess node)
    {
        var (basePtr, structType) = ResolveStructPointer(node.Target);
        var info = _structs[structType.StructName];
        if (!info.Fields.TryGetValue(node.FieldName, out var field))
            throw new NotSupportedException($"struct '{structType.StructName}' has no field '{node.FieldName}'");
        return (_builder.BuildStructGEP2(structType, basePtr, field.Index, $"{node.FieldName}.ptr"), field);
    }

    private (LLVMValueRef Ptr, LLVMTypeRef StructType) ResolveStructPointer(Expr target) => target switch
    {
        NameRef n when _symbols.TryGetValue(n.Name, out var sym) => (sym.Slot, sym.Type),
        ThisExpr => (_symbols["this"].Slot, _symbols["this"].Type),
        _ => throw new NotSupportedException($"cannot resolve struct pointer for {target}")
    };

    private Dictionary<string, LLVMTypeRef> InferTypeArgs(FnDeclaration template, LLVMTypeRef[] argTypes)
    {
        var typeParamNames = template.TypeParams.Select(tp => tp.Name).ToHashSet();
        var substitutionEnv = new Dictionary<string, LLVMTypeRef>();
        for (var i = 0; i < template.Params.Count; i++)
        {
            // increment 1 infers only params written as bare type parameter (x: T).
            // nested forms (x: Box<T>) come with generic structs -- structural unification.
            if (template.Params[i].Type is NamedType n && typeParamNames.Contains(n.Name))
                substitutionEnv[n.Name] = argTypes[i];
        }
        foreach (var tp in template.TypeParams)
            if (!substitutionEnv.ContainsKey(tp.Name))
                throw new NotSupportedException($"could not infer type argument '{tp.Name}' for '{template.Name}'");
        return substitutionEnv;
    }

    private void DeclareFunction(FnDeclaration node)
    {
        var paramTypes = node.Params
            .Select(p => ResolveType(p.Type))
            .ToArray();
        var fnType = LLVMTypeRef.CreateFunction(ResolveType(node.ReturnType), paramTypes);
        var fn = _module.AddFunction(node.Name, fnType);
        var key = new CallableKey("", node.Name, node.Params.Count);
        _functions[key] = new Callable(fn, fnType);
    }

    public LLVMValueRef VisitFnDeclaration(FnDeclaration node)
    {
        var key = new CallableKey("", node.Name, node.Params.Count);
        var fn = _functions[key].Fn;
        var paramTypes = node.Params
            .Select(p => ResolveType(p.Type))
            .ToArray();
        _builder.PositionAtEnd(fn.AppendBasicBlock("entry"));
        _symbols.Clear();
        for (var i = 0; i < node.Params.Count; i++)
        {
            var slot = _builder.BuildAlloca(paramTypes[i], node.Params[i].Name);
            _builder.BuildStore(fn.GetParam((uint)i), slot);
            _symbols[node.Params[i].Name] = new Symbol(slot, paramTypes[i]);
        }
        node.Body.Accept(this);
        // if control falls off the end without a terminator (e.g. a dead if-merge block where
        // both arms returned), cap it -- keeps IR valid instead of an empty, terminatorless block.
        if (_builder.InsertBlock.Terminator.Handle == IntPtr.Zero)
            _builder.BuildUnreachable();
        return fn;
    }

    public LLVMValueRef VisitVariableDeclaration(VariableDeclaration node)
    {
        var value = node.Initializer.Accept(this);
        var type = node.Type is not null
            ? ResolveType(node.Type)
            : value.TypeOf;
        var slot = _builder.BuildAlloca(type, node.Name);
        _builder.BuildStore(value, slot);
        _symbols[node.Name] = new Symbol(slot, type);
        return value;
    }

    public LLVMValueRef VisitNameRef(NameRef node)
    {
        if (_symbols.TryGetValue(node.Name, out var sym))
            return _builder.BuildLoad2(sym.Type, sym.Slot, node.Name);
        throw new NotSupportedException($"Unknown name '{node.Name}'");
    }

    public LLVMValueRef VisitBlock(Block node)
    {
        foreach (var statement in node.Statements)
            statement.Accept(this);
        return default;
    }

    public LLVMValueRef VisitIf(IfStatement node)
    {
        var condition = node.Condition.Accept(this);
        var fn = _builder.InsertBlock.Parent;
        var hasElse = node.Else is not null;
        
        var thenBasicBlock = fn.AppendBasicBlock("then");
        var elseBasicBlock = hasElse ? fn.AppendBasicBlock("else") : default;
        var mergeBasicBlock = fn.AppendBasicBlock("ifcont");    
        
        // with no else, the false edge jumps straight to the merge block
        _builder.BuildCondBr(condition, thenBasicBlock, hasElse ? elseBasicBlock : mergeBasicBlock);
        
        // --- then arm ---
        _builder.PositionAtEnd(thenBasicBlock);
        node.Then.Accept(this);
        if (_builder.InsertBlock.Terminator.Handle == IntPtr.Zero)
            _builder.BuildBr(mergeBasicBlock);
        
        // --- else arm ---
        if (hasElse)
        {
            _builder.PositionAtEnd(elseBasicBlock);
            node.Else!.Accept(this);
            if (_builder.InsertBlock.Terminator.Handle == IntPtr.Zero)
                _builder.BuildBr(mergeBasicBlock);
        }
        
        // subsequent statements continue in the merge block
        _builder.PositionAtEnd(mergeBasicBlock);
        return default;
    }

    public LLVMValueRef VisitReturn(ReturnStatement node)
    {
        var value = node.Value?.Accept(this);
        if (value is null)
            return _builder.BuildRetVoid();
        return _builder.BuildRet(value.Value);
    }

    public LLVMValueRef VisitAssignmentExpr(AssignExpr node)
    {
        var value = node.Value.Accept(this);
        _builder.BuildStore(value, ResolveLValue(node.Target));
        return value;
    }

    // The address to store INTO: a local's slot, or a field's GEP (p.x = ..., this.x = ...).
    private LLVMValueRef ResolveLValue(Expr target) => target switch
    {
        NameRef n when _symbols.TryGetValue(n.Name, out var sym) => sym.Slot,
        FieldAccess f => ResolveField(f).Ptr,
        _ => throw new NotSupportedException($"unsupported assignment target: {target}")
    };

    public LLVMValueRef VisitExpressionStatement(ExpressionStatement node)
    {
        node.Expression.Accept(this);
        return default;
    }

    public LLVMValueRef VisitCast(CastExpr node)
    {
        if (node.Target is DynType dyn)
            return EmitUpcast(node.Value, dyn.InterfaceName);
        return _builder.EmitCast(node.Value.Accept(this), ResolveType(node.Target));
    }

    private LLVMValueRef EmitUpcast(Expr value, string interfaceName)
    {
        var (dataPtr, structType) = ResolveStructPointer(value);
        var vtableKey = new VTableKey(structType.StructName, interfaceName);
        var vtable = _vtables[vtableKey];
        var dynType = GetDynType(interfaceName);
        var fatPtr = dynType.Undef;
        fatPtr = _builder.BuildInsertValue(fatPtr, dataPtr, 0, "dyn.data");
        fatPtr = _builder.BuildInsertValue(fatPtr, vtable, 1, "dyn.vtable");
        return fatPtr;
    }

    public LLVMValueRef VisitCall(CallExpr node)
    {
        var args = node.Args
            .Select(arg => arg.Accept(this))
            .ToArray();
        if (node.Callee == "print")
            return EmitPrint(args);
        var key = new CallableKey("", node.Callee, args.Length);
        if (_functions.TryGetValue(key, out var target))        // already a real fn? then call it
            return _builder.BuildCall2(target.Signature, target.Fn, args, "call");
        if (_fnTemplates.TryGetValue(key, out var template)) // a recipe/template for a fn? build it, then call.
        {
            var substitutionEnv = InferTypeArgs(template, args.Select(arg => arg.TypeOf).ToArray());
            var instance = Instantiate(template, substitutionEnv);
            return _builder.BuildCall2(instance.Signature, instance.Fn, args, "call");
        }
        throw new NotSupportedException($"no function '{node.Callee}' taking {args.Length} args");
    }

    public LLVMValueRef VisitBinary(BinaryExpr node)
    {
        // && / || short-circuit BEFORE evaluating the right side (unlike arithmetic/comparison,
        // which need both operands). Same phi machinery as the CST path -- but here it collapses
        // into one node kind (BinaryExpr) rather than two separate visitors.
        if (node.Operation is "&&" or "||")
            return EmitShortCircuit(node);

        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);
        var isFloat = ExodiaHelpers.IsFloat(left.TypeOf);

        if (node.Operation is "<" or ">" or "<=" or ">=" or "==" or "!=")
            return isFloat
                ? _builder.BuildFCmp(AstHelpers.FloatPredicate(node.Operation), left, right, "fcmp")
                : _builder.BuildICmp(AstHelpers.IntPredicate(node.Operation), left, right, "cmp");
        
        return node.Operation switch
        {
            "+" => isFloat ? _builder.BuildFAdd(left, right, "fadd") : _builder.BuildAdd(left, right, "add"),
            "-" => isFloat ? _builder.BuildFSub(left, right, "fsub") : _builder.BuildSub(left, right, "sub"),
            "*" => isFloat ? _builder.BuildFMul(left, right, "fmul") : _builder.BuildMul(left, right, "mul"),
            "/" => isFloat ? _builder.BuildFDiv(left, right, "fdiv") : _builder.BuildSDiv(left, right, "div"),
            _ => throw new NotSupportedException($"binary op '{node.Operation}'")
        }; 
    }

    private LLVMValueRef EmitShortCircuit(BinaryExpr node)
    {
        var isAnd = node.Operation == "&&";
        var left = node.Left.Accept(this);
        var fn = _builder.InsertBlock.Parent;
        var startBB = _builder.InsertBlock;
        var rhsBB = fn.AppendBasicBlock(isAnd ? "and.rhs" : "or.rhs");
        var mergeBB = fn.AppendBasicBlock(isAnd ? "and.end" : "or.end");

        // &&: left true -> eval rhs, false -> merge.   ||: left true -> merge, false -> eval rhs.
        if (isAnd) _builder.BuildCondBr(left, rhsBB, mergeBB);
        else       _builder.BuildCondBr(left, mergeBB, rhsBB);

        _builder.PositionAtEnd(rhsBB);
        var right = node.Right.Accept(this);
        var rhsEndBB = _builder.InsertBlock;            // block we ACTUALLY end in (nested logic may move it)
        _builder.BuildBr(mergeBB);

        _builder.PositionAtEnd(mergeBB);
        var phi = _builder.BuildPhi(LLVMTypeRef.Int1, isAnd ? "and" : "or");
        var shortCircuit = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, isAnd ? 0UL : 1UL);
        phi.AddIncoming([shortCircuit, right], [startBB, rhsEndBB], 2);
        return phi;
    }

    public LLVMValueRef VisitUnary(UnaryExpr node)
    {
        var operand = node.Operand.Accept(this);
        return node.Operation switch
        {
            "+" => operand,
            "-" => ExodiaHelpers.IsFloat(operand.TypeOf) ? _builder.BuildFNeg(operand, "fneg") : _builder.BuildNeg(operand, "neg"),
            "!" => _builder.BuildNot(operand, "not"),
            _ => throw new NotSupportedException($"unary op '{node.Operation}'")
        };
    }

    public LLVMValueRef VisitWhile(WhileStatement node)
    {
        var fn = _builder.InsertBlock.Parent;
        var condBB = fn.AppendBasicBlock("while.cond");
        var bodyBB = fn.AppendBasicBlock("while.body");
        var exitBB = fn.AppendBasicBlock("while.exit");

        _builder.BuildBr(condBB);
        _builder.PositionAtEnd(condBB);
        _builder.BuildCondBr(node.Condition.Accept(this), bodyBB, exitBB);

        _builder.PositionAtEnd(bodyBB);
        node.Body.Accept(this);
        if (_builder.InsertBlock.Terminator.Handle == IntPtr.Zero)
            _builder.BuildBr(condBB);                   // back-edge

        _builder.PositionAtEnd(exitBB);
        return default;
    }

    public LLVMValueRef VisitIntLiteral(IntLiteral node)
        => LLVMValueRef.CreateConstInt(node.Type is { } t ? ResolveType(t) : LLVMTypeRef.Int32, node.Value);

    public LLVMValueRef VisitFloatLiteral(FloatLiteral node)
        => LLVMValueRef.CreateConstReal(node.Type is { } t ? ResolveType(t) : LLVMTypeRef.Double, node.Value);

    public LLVMValueRef VisitBoolLiteral(BoolLiteral node)
        => LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, node.Value ? 1UL : 0UL);

#region printf
    // --- print built-in (libc printf bootstrap) ---
    private readonly Dictionary<string, LLVMValueRef> _formats = [];

    private LLVMValueRef EmitPrint(LLVMValueRef[] args)
    {
        if (args.Length != 1)
            throw new NotSupportedException("print expects exactly one argument (for now)");

        var value = args[0];
        var type = value.TypeOf;
        string fmt;
        if (ExodiaHelpers.IsFloat(type))
        {
            if (type.Kind == LLVMTypeKind.LLVMFloatTypeKind)
                value = _builder.BuildFPExt(value, LLVMTypeRef.Double, "promote");
            fmt = "%f\n";
        }
        else
        {
            var bits = type.IntWidth;
            if (bits < 32)
            {
                value = bits == 1
                    ? _builder.BuildZExt(value, LLVMTypeRef.Int32, "promote")
                    : _builder.BuildSExt(value, LLVMTypeRef.Int32, "promote");
                fmt = "%d\n";
            }
            else fmt = bits == 32 ? "%d\n" : "%ld\n";
        }

        var printf = GetPrintf();
        return _builder.BuildCall2(printf.Type, printf.Fn, new[] { GetFormat(fmt), value }, "");
    }

    private (LLVMValueRef Fn, LLVMTypeRef Type) GetPrintf()
    {
        var printfType = LLVMTypeRef.CreateFunction(
            LLVMTypeRef.Int32, [LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0)], IsVarArg: true);
        var existing = _module.GetNamedFunction("printf");
        return existing.Handle != IntPtr.Zero
            ? (existing, printfType)
            : (_module.AddFunction("printf", printfType), printfType);
    }

    private LLVMValueRef GetFormat(string fmt)
    {
        if (_formats.TryGetValue(fmt, out var g)) return g;
        g = _builder.BuildGlobalStringPtr(fmt, "fmt");
        _formats[fmt] = g;
        return g;
    }
#endregion

    private LLVMTypeRef ResolveType(TypeRef type) => type switch
    {
        NamedType n when _activeSubstitutionEnv.TryGetValue(n.Name, out var bound) => bound,  // type param -> concrete
        NamedType n when _structs.TryGetValue(n.Name, out var info) => info.Type,   // struct name -> %Struct
        NamedType n => AstHelpers.MapPrimitiveType(n.Name),
        DynType d => GetDynType(d.InterfaceName),
        GenericType g => ResolveGenericType(g),                                     // Box<int32> -> %Box$i32
        _ => throw new NotSupportedException($"type {type} not supported yet")
    };

    // Resolve an explicit generic application (`Box<int32>`, or `Box<T>` under an active env)
    // to its instantiated LLVM struct type.
    private LLVMTypeRef ResolveGenericType(GenericType g)
    {
        if (!_structTemplates.TryGetValue(g.Name, out var template))
            throw new NotSupportedException($"'{g.Name}' is not a generic struct");
        return InstantiateStruct(template, BuildStructEnv(template, g.TypeArgs)).Type;
    }

    // Map explicit type arguments (from `Box<int32>` or `new Box<int32>(...)`) to a substitution
    // env. Args may themselves be type params, resolved via the currently-active env.
    private Dictionary<string, LLVMTypeRef> BuildStructEnv(StructDeclaration template, IReadOnlyList<TypeRef> typeArgs)
    {
        if (template.TypeParams.Count != typeArgs.Count)
            throw new NotSupportedException($"'{template.Name}' expects {template.TypeParams.Count} type argument(s), got {typeArgs.Count}");
        var env = new Dictionary<string, LLVMTypeRef>();
        for (var i = 0; i < template.TypeParams.Count; i++)
            env[template.TypeParams[i].Name] = ResolveType(typeArgs[i]);
        return env;
    }
}

