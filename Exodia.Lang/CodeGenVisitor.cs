using System.Globalization;
using LLVMSharp.Interop;

namespace Exodia.Lang;

public class CodeGenVisitor : ExodiaBaseVisitor<LLVMValueRef>
{
    public readonly LLVMModuleRef Module;
    public readonly LLVMBuilderRef Builder;
    
    private readonly Dictionary<string, Symbol> _symbols = [];
    private readonly Dictionary<string, Callable> _functions = [];
    private readonly Dictionary<string, StructInfo> _structs = [];
    private readonly Dictionary<string, Callable> _constructors = [];
    private readonly Dictionary<(string Struct, string Method), Callable> _methods = [];

    public CodeGenVisitor(LLVMModuleRef module)
    {
        Module = module;
        Builder = Module.Context.CreateBuilder();
    }
    
    public override LLVMValueRef VisitProgram(ExodiaParser.ProgramContext context)
    {
        // phase 1: struct types (so signatures/fields can reference them)
        foreach (var statement in context.statement())
            if (statement.struct_declaration() is { } sd) 
                RegisterStruct(sd);
        
        // phase 2: function signatures
        foreach (var statement in context.statement())
            if (statement.function_declaration() is { } fd)
                DeclareFunction(fd);
        
        // phase 3: bodies
        return VisitChildren(context);
    }

#region printf
    private readonly Dictionary<string, LLVMValueRef> _formats = new();
    
    private (LLVMValueRef Fn, LLVMTypeRef Type) GetPrintf()
    {
        var ptrType = LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0);
        var printfType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, [ptrType], IsVarArg: true);

        var existing = Module.GetNamedFunction("printf");
        if (existing.Handle != IntPtr.Zero)
            return (existing, printfType);

        var fn = Module.AddFunction("printf", printfType);
        return (fn, printfType);
    }

    private LLVMValueRef EmitPrint(LLVMValueRef[] args)
    {
        if (args.Length != 1)
            throw new NotSupportedException("print expects exactly one argument (for now)");

        var value = args[0];
        var type = value.TypeOf;
        string fmt;

        if (ExodiaHelpers.IsFloat(type))
        {
            // varargs: a float is promoted to double; %f reads a double.
            if (type.Kind == LLVMTypeKind.LLVMFloatTypeKind)
                value = Builder.BuildFPExt(value, LLVMTypeRef.Double, "promote");
            fmt = "%f\n";
        }
        else
        {
            // varargs: integers narrower than int are promoted to int.
            var bits = type.IntWidth;
            if (bits < 32)
            {
                value = bits == 1
                    ? Builder.BuildZExt(value, LLVMTypeRef.Int32, "promote")   // bool -> 0/1
                    : Builder.BuildSExt(value, LLVMTypeRef.Int32, "promote");
                fmt = "%d\n";
            }
            else fmt = bits == 32 ? "%d\n" : "%ld\n";   // i64 -> %ld
        }

        var printf = GetPrintf();
        var callArgs = new[] { GetFormat(fmt), value };
        return Builder.BuildCall2(printf.Type, printf.Fn, callArgs, "");
        
        LLVMValueRef GetFormat(string fmt)
        {
            if (_formats.TryGetValue(fmt, out var g)) return g;
            g = Builder.BuildGlobalStringPtr(fmt, "fmt");
            _formats[fmt] = g;
            return g;
        }
    }
#endregion

    private void DeclareFunction(ExodiaParser.Function_declarationContext context)
    {
        var name = context.identifier().GetText(); // "main"
        if (_functions.ContainsKey(name)) return;
        
        // parameters -- all int32 for now
        var formals = context.formal_parameter_list()?.formal_parameter() ?? [];
        var paramTypes = formals
            .Select(f => ExodiaHelpers.MapType(f.type()))
            .ToArray();
        var fnType = LLVMTypeRef.CreateFunction(ExodiaHelpers.MapType(context.type()), paramTypes);
        _functions[name] = new Callable(Module.AddFunction(name, fnType), fnType);
    }

    private void DeclareMethod(string structName, LLVMTypeRef structType, ExodiaParser.Method_declarationContext method)
    {
        var methodName = method.identifier().GetText();
        var formals = method.formal_parameter_list()?.formal_parameter() ?? [];
        var paramTypes = new LLVMTypeRef[formals.Length + 1];
        paramTypes[0] = LLVMTypeRef.CreatePointer(structType, 0); // `this` by pointer
        for (var i = 0; i < formals.Length; i++)
            paramTypes[i + 1] = ExodiaHelpers.MapType(formals[i].type());
        var sig = LLVMTypeRef.CreateFunction(ExodiaHelpers.MapType(method.type()), paramTypes);
        _methods[(structName, methodName)] = new Callable(Module.AddFunction($"{structName}.{methodName}", sig), sig);
    }
    
    private void EmitMethod(string structName, ExodiaParser.Method_declarationContext method)
    {
        var methodName = method.identifier().GetText();
        var fn = _methods[(structName, methodName)].Fn;
        var structType = _structs[structName].Type;
        var formals = method.formal_parameter_list()?.formal_parameter() ?? [];

        Builder.PositionAtEnd(fn.AppendBasicBlock("entry"));
        _symbols.Clear();
        _symbols["this"] = new Symbol(fn.GetParam(0), structType);        // received pointer
        for (var i = 0; i < formals.Length; i++)
        {
            var t = ExodiaHelpers.MapType(formals[i].type());
            var slot = Builder.BuildAlloca(t, formals[i].identifier().GetText());
            Builder.BuildStore(fn.GetParam((uint)(i + 1)), slot);
            _symbols[formals[i].identifier().GetText()] = new Symbol(slot, t);
        }
        Visit(method.function_body());
    }

    public override LLVMValueRef VisitFunction_body(ExodiaParser.Function_bodyContext context)
    {
        if (context.expression() is { } expr)
            return Builder.BuildRet(Visit(expr));   // `=> expr;`
        return Visit(context.block_statement());    // `{...}`
    }

    public override LLVMValueRef VisitStruct_declaration(ExodiaParser.Struct_declarationContext context)
    {
        var structName = context.identifier().GetText();
        foreach (var member in context.member())
        {
            var kind = member.member_kind();
            if (kind.constructor_declaration() is { } ctor && ctor.identifier() is null)
                EmitConstructor(structName, ctor);
            else if (kind.method_declaration() is { } method)
                EmitMethod(structName, method);
        }
        return default;
    }
    
    private void DeclareConstructor(string structName, LLVMTypeRef structType,
        ExodiaParser.Constructor_declarationContext ctor)
    {
        var formals = ctor.formal_parameter_list()?.formal_parameter() ?? [];
        var paramTypes = formals.Select(f => ExodiaHelpers.MapType(f.type())).ToArray();
        var sig = LLVMTypeRef.CreateFunction(structType, paramTypes);
        _constructors[structName] = new Callable(Module.AddFunction($"{structName}.ctor", sig), sig);
    }

    private void EmitConstructor(string structName, ExodiaParser.Constructor_declarationContext ctor)
    {
        var fn = _constructors[structName].Fn;
        var structType = _structs[structName].Type;
        var formals = ctor.formal_parameter_list()?.formal_parameter() ?? [];
        
        Builder.PositionAtEnd(fn.AppendBasicBlock("entry"));
        
        // `this` is a fresh struct slot the body fills in -- bound as an ordinary symbol,
        // so `this.x = ...;` flows through ResolveLValue exactly like `p.x = ...;`.
        var thisSlot = Builder.BuildAlloca(structType, "this");
        _symbols["this"] = new Symbol(thisSlot, structType);

        for (var i = 0; i < formals.Length; i++) // bind params as locals
        {
            var type = ExodiaHelpers.MapType(formals[i].type());
            var slot = Builder.BuildAlloca(type, formals[i].identifier().GetText());
            Builder.BuildStore(fn.GetParam((uint)i), slot);
            _symbols[formals[i].identifier().GetText()] = new Symbol(slot, type);
        }

        Visit(ctor.block_statement()); // this.x = a; this.y = a * 2; ...

        // implicit "return this": load the initialized struct and return it by value
        Builder.BuildRet(Builder.BuildLoad2(structType, thisSlot, "thisval"));
    }
    
    private void RegisterStruct(ExodiaParser.Struct_declarationContext context)
    {
        var name = context.identifier().GetText();
        if (_structs.ContainsKey(name)) return;

        var fields = new Dictionary<string, StructInfoField>();
        var fieldTypes = new List<LLVMTypeRef>();
        uint index = 0;
        foreach (var member in context.member())
        {
            if (member.member_kind().field_declaration() is not { } fieldDeclaration) 
                continue;
            var memberType = ExodiaHelpers.MapType(fieldDeclaration.type());
            fields[fieldDeclaration.identifier().GetText()] = new StructInfoField(index, memberType);
            fieldTypes.Add(memberType);
            index++;
        }

        var structType = Module.Context.CreateNamedStruct(name);
        structType.StructSetBody(fieldTypes.ToArray(), false);
        _structs[name] = new StructInfo(structType, fields);
        foreach (var member in context.member())
            if (member.member_kind().constructor_declaration() is { } ctor && ctor.identifier() is null)
                DeclareConstructor(name, structType, ctor);
            else if (member.member_kind().method_declaration() is { } method)
                DeclareMethod(name, structType, method);
    }

    // fn <name>(...): int32 { ... }   -- for now assume i32 return, no params
    public override LLVMValueRef VisitFunction_declaration(ExodiaParser.Function_declarationContext context)
    {
        DeclareFunction(context);
        var name = context.identifier().GetText();
        var fn = _functions[name].Fn;
        var formals = context.formal_parameter_list()?.formal_parameter() ?? [];
        
        Builder.PositionAtEnd(fn.AppendBasicBlock("entry"));
        _symbols.Clear(); // fresh scope per function

        for (var i = 0; i < formals.Length; i++)
        {
            var t = ExodiaHelpers.MapType(formals[i].type());
            var slot = Builder.BuildAlloca(t, formals[i].identifier().GetText());
            Builder.BuildStore(fn.GetParam((uint)i), slot);
            _symbols[formals[i].identifier().GetText()] = new Symbol(slot, t);
        }

        Visit(context.function_body());
        return fn;
    }

    // return <expr>;
    public override LLVMValueRef VisitReturn_statement(ExodiaParser.Return_statementContext context)
    {
        var value = Visit(context.expression());
        return Builder.BuildRet(value);
    }
    
    // an integer literal, e.g. 0, 42, 69, 420
    public override LLVMValueRef VisitNumeric_literal(ExodiaParser.Numeric_literalContext context)
    {
        var raw = context.GetText().Replace("_", ""); // strip digit separators

        if (context.FLOAT() is not null)
        {
            var last = raw[^1];
            var hasSuffix = last is 'f' or 'd' or 'm';
            var type = hasSuffix ? ExodiaHelpers.MapFloatSuffixType(last) : LLVMTypeRef.Double;
            var digits = hasSuffix ? raw[..^1] : raw;
            // real literal -> double default.
            if (!double.TryParse(digits, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                throw new NotSupportedException($"Float literal suffix not supported yet: '{context.GetText()}'");
            return LLVMValueRef.CreateConstReal(type, d);
        }
        
        // integer: digits are [0-9], so the first i/u marks the suffix start.
        var suffixIdx = raw.IndexOfAny(['i', 'u']);
        var intType = LLVMTypeRef.Int32; // Default
        var intDigits = raw;

        if (suffixIdx >= 0)
        {
            var intSuffix = raw[suffixIdx..];
            intType = ExodiaHelpers.MapIntSuffixType(intSuffix);
            intDigits = raw[..suffixIdx];
        }
        
        return LLVMValueRef.CreateConstInt(intType, ulong.Parse(intDigits));
    }

    public override LLVMValueRef VisitAdditive_expression(ExodiaParser.Additive_expressionContext context)
    {
        if (context.op == null)
            return Visit(context.multiplicative_expression());

        var left = Visit(context.left);
        var right = Visit(context.right);
        var isFloat = ExodiaHelpers.IsFloat(left.TypeOf);
        return context.op.Text switch
        {
            "+" => isFloat 
                ? Builder.BuildFAdd(left, right, "fadd") 
                : Builder.BuildAdd(left, right, "add"),
            "-" => isFloat
                ? Builder.BuildFSub(left, right, "fsub")
                : Builder.BuildSub(left, right, "sub"),
            _ => throw new NotSupportedException($"Additive op '{context.op.Text}'")
        };
    }

    public override LLVMValueRef VisitMultiplicative_expression(ExodiaParser.Multiplicative_expressionContext context)
    {
        if (context.op == null)
            return Visit(context.cast_expression());

        var left = Visit(context.left);
        var right = Visit(context.right);
        var isFloat = ExodiaHelpers.IsFloat(left.TypeOf);
        return context.op.Text switch
        {
            "*" => isFloat 
                ? Builder.BuildFMul(left, right, "fmul") 
                : Builder.BuildMul(left, right, "mul"),
            "/" => isFloat
                ? Builder.BuildFDiv(left, right, "fdiv") 
                : Builder.BuildSDiv(left, right, "div"),
            _ => throw new NotSupportedException($"Multiplicative op '{context.op.Text}'")
        };
    }

    public override LLVMValueRef VisitParenthesized_expression(ExodiaParser.Parenthesized_expressionContext context)
    {
        return Visit(context.expression());
    }

    public override LLVMValueRef VisitLogical_AND_expression(ExodiaParser.Logical_AND_expressionContext context)
    {
        if (context.op == null)
            return Visit(context.equality_expression());        // passthrough

        var left = Visit(context.left);                          // i1
        var fn = Builder.InsertBlock.Parent;
        var startBB = Builder.InsertBlock;                       // block holding the condbr

        var rhsBB   = fn.AppendBasicBlock("and.rhs");
        var mergeBB = fn.AppendBasicBlock("and.end");

        Builder.BuildCondBr(left, rhsBB, mergeBB);               // left false -> straight to merge

        Builder.PositionAtEnd(rhsBB);
        var right = Visit(context.right);                        // i1
        var rhsEndBB = Builder.InsertBlock;                      // where we ACTUALLY end (nested && may move it)
        Builder.BuildBr(mergeBB);

        Builder.PositionAtEnd(mergeBB);
        var phi = Builder.BuildPhi(LLVMTypeRef.Int1, "and");
        phi.AddIncoming(
            [LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, 0), right],  // false from start, else b
            [startBB, rhsEndBB],
            2);
        return phi;
    }
    
    public override LLVMValueRef VisitLogical_OR_expression(ExodiaParser.Logical_OR_expressionContext context)
    {
        if (context.op == null)
            return Visit(context.logical_AND_expression());     // passthrough

        var left = Visit(context.left);
        var fn = Builder.InsertBlock.Parent;
        var startBB = Builder.InsertBlock;

        var rhsBB   = fn.AppendBasicBlock("or.rhs");
        var mergeBB = fn.AppendBasicBlock("or.end");

        Builder.BuildCondBr(left, mergeBB, rhsBB);               // left TRUE -> skip b, result true

        Builder.PositionAtEnd(rhsBB);
        var right = Visit(context.right);
        var rhsEndBB = Builder.InsertBlock;
        Builder.BuildBr(mergeBB);

        Builder.PositionAtEnd(mergeBB);
        var phi = Builder.BuildPhi(LLVMTypeRef.Int1, "or");
        phi.AddIncoming(
            [LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, 1), right],  // true from start, else b
            [startBB, rhsEndBB],
            2);
        return phi;
    }

    public override LLVMValueRef VisitUnary_expression(ExodiaParser.Unary_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.postfix_expression());

        var operand = Visit(context.unary_expression());
        return context.op.Text switch
        {
            "!" => Builder.BuildNot(operand, "not"),
            "+" => operand,                 // Unary plus: same as no-op
            "-" => ExodiaHelpers.IsFloat(operand.TypeOf)
                ? Builder.BuildFNeg(operand, "fneg")
                : Builder.BuildNeg(operand, "neg"),
            _ => throw new NotSupportedException($"Unary op '{context.op.Text}'")
        };
    }

    public override LLVMValueRef VisitCast_expression(ExodiaParser.Cast_expressionContext context)
    {
        var value = Visit(context.unary_expression());
        foreach (var typeCtx in context.type())
            value = EmitCast(value, ExodiaHelpers.MapType(typeCtx));
        return value;

        LLVMValueRef EmitCast(LLVMValueRef value, LLVMTypeRef target)
        {
            var source = value.TypeOf;
            if (source.Handle == target.Handle)
                return value; // same type = no-op

            var srcFloat = ExodiaHelpers.IsFloat(source);
            var dstFloat = ExodiaHelpers.IsFloat(target);
            
            // int -> int : widen (sext; zext for i1/bool) or narrow (trunc)
            if (!srcFloat && !dstFloat)
            {
                // ZExt -> zero-extend; extends an int with more bits where new bits are 0, preserves as an unsigned number
                // SExt -> sign-extend; extends an int with more bits but preserves the MSB for signed numbers 
                if (source.IntWidth == target.IntWidth) return value;
                if (source.IntWidth < target.IntWidth)
                    return source.IntWidth == 1
                        ? Builder.BuildZExt(value, target, "zext")  // bool/i1 => 0/1, not 0/-1
                        : Builder.BuildSExt(value, target, "sext");
                return Builder.BuildTrunc(value, target, "trunc");
            }

            if (!srcFloat && dstFloat) return Builder.BuildSIToFP(value, target, "sitofp"); // int -> float
            if (srcFloat && !dstFloat) return Builder.BuildFPToSI(value, target, "fptosi"); // float -> int
            
            // float -> float : float(32) <-> double(64)
            var srcBits = source.Kind == LLVMTypeKind.LLVMDoubleTypeKind ? 64 : 32;
            var dstBits = target.Kind == LLVMTypeKind.LLVMDoubleTypeKind ? 64 : 32;
            if (srcBits == dstBits) return value;
            return srcBits < dstBits
                ? Builder.BuildFPExt(value, target, "fpext")
                : Builder.BuildFPTrunc(value, target, "fptrunc");
        }
    }

    public override LLVMValueRef VisitVariable_statement(ExodiaParser.Variable_statementContext context)
        => Visit(context.variable_declaration_list());

    public override LLVMValueRef VisitVariable_declaration(ExodiaParser.Variable_declarationContext context)
    {
        var id = context.identifier().GetText();
        var value = Visit(context.variable_initializer()); // must visit first to allow type inference
        var type = context.type() is { } t ? ExodiaHelpers.MapType(t) : value.TypeOf; // annotation, else infer
        var slot = Builder.BuildAlloca(type, id);
        Builder.BuildStore(value, slot);
        _symbols[id] = new Symbol(slot, type);
        return value;
    }

    // a == b , a != b   -> i1
    public override LLVMValueRef VisitEquality_expression(ExodiaParser.Equality_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.relational_expression());

        var left = Visit(context.left);
        var right = Visit(context.right);        
        var isFloat = ExodiaHelpers.IsFloat(left.TypeOf);

        if (isFloat)
        {
            var fpredicate = context.op.Text switch
            {
                "==" => LLVMRealPredicate.LLVMRealOEQ,
                "!=" => LLVMRealPredicate.LLVMRealONE,
                _ => throw new NotSupportedException($"Equality op '{context.op.Text}'")
            };
            return Builder.BuildFCmp(fpredicate, left, right, "fcmp");
        }

        var predicate = context.op.Text switch
        {
            "==" => LLVMIntPredicate.LLVMIntEQ,
            "!=" => LLVMIntPredicate.LLVMIntNE,
            _ => throw new NotSupportedException($"Equality op '{context.op.Text}'")
        };
        return Builder.BuildICmp(predicate, left, right, "cmp");
    }

    public override LLVMValueRef VisitTrue_literal(ExodiaParser.True_literalContext context)
        => LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, 1);

    public override LLVMValueRef VisitFalse_literal(ExodiaParser.False_literalContext context)
        => LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, 0);
    
    // a < b , a > b , a <= b , a >= b   -> i1  (signed comparisons)
    public override LLVMValueRef VisitRelational_expression(ExodiaParser.Relational_expressionContext context)
    {
        if (context.op == null)
            return Visit(context.shift_expression());

        var left = Visit(context.left);
        var right = Visit(context.right);
        var isFloat = ExodiaHelpers.IsFloat(left.TypeOf);

        if (isFloat)
        {
            var fpred = context.op.Text switch
            {
                "<"  => LLVMRealPredicate.LLVMRealOLT,   // Signed Less Than
                ">"  => LLVMRealPredicate.LLVMRealOGT,
                "<=" => LLVMRealPredicate.LLVMRealOLE,
                ">=" => LLVMRealPredicate.LLVMRealOGE,
                _ => throw new NotSupportedException($"Relational op '{context.op.Text}'")
            };
            return Builder.BuildFCmp(fpred, left, right, "fcmp");
        }
        
        var pred = context.op.Text switch
        {
            "<"  => LLVMIntPredicate.LLVMIntSLT,   // Signed Less Than
            ">"  => LLVMIntPredicate.LLVMIntSGT,
            "<=" => LLVMIntPredicate.LLVMIntSLE,
            ">=" => LLVMIntPredicate.LLVMIntSGE,
            _ => throw new NotSupportedException($"Relational op '{context.op.Text}'")
        };
        return Builder.BuildICmp(pred, left, right, "cmp");
    }

    public override LLVMValueRef VisitIf_statement(ExodiaParser.If_statementContext context)
    {
        var condition = Visit(context.expression()); // an i1 from a comparison
        var fn = Builder.InsertBlock.Parent;    // the function we're emitting into
        var hasElse = context.ELSE() is not null;

        var thenBasicBlock = fn.AppendBasicBlock("then");
        var elseBasicBlock = hasElse ? fn.AppendBasicBlock("else") : default;
        var mergeBasicBlock = fn.AppendBasicBlock("ifcont");
        
        // with no else, the false edge jumps straight to the merge block
        Builder.BuildCondBr(condition, thenBasicBlock, hasElse ? elseBasicBlock : mergeBasicBlock);
        
        // --- then arm ---
        Builder.PositionAtEnd(thenBasicBlock);
        Visit(context.statement(0));
        
        // Only branch to merge if this arm didn't already terminate. If the then-branch
        // ended in `return`, the block already has a `ret` -- adding a `br` after it would 
        // be a SECOND terminator which is invalid IR
        if (Builder.InsertBlock.Terminator.Handle == IntPtr.Zero)
            Builder.BuildBr(mergeBasicBlock);
        
        // --- else arm ---
        if (hasElse)
        {
            Builder.PositionAtEnd(elseBasicBlock);
            Visit(context.statement(1));
            if (Builder.InsertBlock.Terminator.Handle == IntPtr.Zero)
                Builder.BuildBr(mergeBasicBlock);
        }
        
        // subsequent statements continue in the merge block
        Builder.PositionAtEnd(mergeBasicBlock);
        return default;
    }

    public override LLVMValueRef VisitWhile_statement(ExodiaParser.While_statementContext context)
    {
        var fn = Builder.InsertBlock.Parent;

        var condBB = fn.AppendBasicBlock("while.cond");
        var bodyBB = fn.AppendBasicBlock("while.body");
        var exitBB = fn.AppendBasicBlock("while.exit");
        
        // enter the loop: jump to the condition test
        Builder.BuildBr(condBB);
        
        // --- cond: eval the test every iteration, branch in or out ---
        Builder.PositionAtEnd(condBB);
        var condition = Visit(context.expression());
        Builder.BuildCondBr(condition, bodyBB, exitBB);
        
        // --- body: run it, then jump BACK to cond ---
        Builder.PositionAtEnd(bodyBB);
        Visit(context.statement());
        if (Builder.InsertBlock.Terminator.Handle == IntPtr.Zero)
            Builder.BuildBr(condBB);
        
        // --- exit: code after the loop continues here ---
        Builder.PositionAtEnd(exitBB);
        return default;
    }

    // x = <expr> -- mutation of an existing local
    public override LLVMValueRef VisitAssignment_expression(ExodiaParser.Assignment_expressionContext context)
    {
        // passthrough alt: `assignment_expression :  logical_OR_expression`.
        // EVERY ordinary expression flows through here -- must be preserved.
        if (context.assignment_expression() is null)
            return Visit(context.logical_OR_expression());

        var op = context.assignment_operator().GetText();
        if (op is not "=")
            throw new NotSupportedException($"Compound assignment '{op}' not supported yet");
        
        // RHS first (visit right-recursive, so `a = b = 5` chains for free
        var value = Visit(context.assignment_expression());
        // LHS
        var ptr = ExodiaHelpers.ResolveLValue(
            context.left_hand_side_expression().postfix_expression(),
            Builder,
            _structs,
            _symbols);
        Builder.BuildStore(value, ptr);
        return value; // assignment yields the assigned value
    }

    public override LLVMValueRef VisitPostfix_expression(ExodiaParser.Postfix_expressionContext context)
    {
        var ops = context.postfix_op();

        if (ops.Length == 0)
            return Visit(context.primary_expression());

        if (ops.Length == 1 && ops[0].arguments() is { } argsCtx)
        {
            // the callee is resolved by NAME here -- NOT visited as a variable
            var fnName = context.primary_expression().GetText(); // "add"

            // collect args (walk the left-recursive list), eval each
            var argCtxs = ExodiaHelpers.CollectArgs(argsCtx.argument_list());
            var args = new LLVMValueRef[argCtxs.Count];
            for (var i = 0; i < argCtxs.Count; i++)
                args[i] = Visit(argCtxs[i].assignment_expression());
            
            if (fnName == "print")
                return EmitPrint(args);
            
            if (!_functions.TryGetValue(fnName, out var target))
                throw new NotSupportedException($"Unknown function '{fnName}'");
            
            // target.Type is already the full function type from declaration -- use directly
            return Builder.BuildCall2(target.Signature, target.Fn, args, "call");
        }

        if (ops.Length == 1 && ops[0].identifier() is { } memberId)
        {
            // p.x -- field read on a struct literal. Resolve the base to its SLOT (pointer)
            // NOT a loaded field, because GEP needs the address to index into
            var baseName = context.primary_expression().GetText();
            if (!_symbols.TryGetValue(baseName, out var sym))
                throw new NotSupportedException($"Unknown name '{baseName}'");
            if (sym.Type.Kind != LLVMTypeKind.LLVMStructTypeKind)
                throw new NotSupportedException($"'{baseName}' is not a struct");
            
            // find the field's index + type via the struct's name -> StructInfo
            var structName = sym.Type.StructName;
            var info = _structs[structName];
            var fieldName = memberId.GetText();
            if (!info.Fields.TryGetValue(fieldName, out var field))
                throw new NotSupportedException($"struct '{structName}' has no field '{fieldName}'");
            
            // GEP to the field's address, then load through it
            var fieldPtr = Builder.BuildStructGEP2(sym.Type, sym.Slot, field.Index, $"{baseName}.{fieldName}.ptr");
            return Builder.BuildLoad2(field.Type, fieldPtr, $"{baseName}.{fieldName}");
        }
        
        if (ops.Length == 2 && ops[0].identifier() is { } methodId && ops[1].arguments() is { } methodArgsCtx)
        {
            var baseName = context.primary_expression().GetText();
            if (!_symbols.TryGetValue(baseName, out var sym))
                throw new NotSupportedException($"Unknown name '{baseName}'");
            if (sym.Type.Kind != LLVMTypeKind.LLVMStructTypeKind)
                throw new NotSupportedException($"'{baseName}' is not a struct");
            var methodName = methodId.GetText();
            if (!_methods.TryGetValue((sym.Type.StructName, methodName), out var m))
                throw new NotSupportedException($"struct '{sym.Type.StructName}' has no method '{methodName}'");

            var argCtxs = ExodiaHelpers.CollectArgs(methodArgsCtx.argument_list());
            var args = new LLVMValueRef[argCtxs.Count + 1];
            args[0] = sym.Slot;                                              // pass &receiver as `this`
            for (var i = 0; i < argCtxs.Count; i++)
                args[i + 1] = Visit(argCtxs[i].assignment_expression());
            return Builder.BuildCall2(m.Signature, m.Fn, args, $"{sym.Type.StructName}.{methodName}.call");
        }

        throw new NotSupportedException("Only simple f(args) calls supported so far");
    }

    public override LLVMValueRef VisitNew_expression(ExodiaParser.New_expressionContext context)
    {
        var name = context.qualified_name().GetText();
        if (!_structs.TryGetValue(name, out var info))
            throw new NotSupportedException($"Unknown struct '{name}");
        
        var argCtxs = ExodiaHelpers.CollectArgs(context.arguments().argument_list());
        var args = new LLVMValueRef[argCtxs.Count];
        for (var i = 0; i < argCtxs.Count; i++)
            args[i] = Visit(argCtxs[i].assignment_expression());

        if (_constructors.TryGetValue(name, out var ctor)) // has a ctor -> call it
            return Builder.BuildCall2(ctor.Signature, ctor.Fn, args, $"{name}.new");
        
        // build the struct value: start from undef, insertvalue each field in order
        if (argCtxs.Count != info.Fields.Count)
            throw new NotSupportedException($"struct '{name}' expects {info.Fields.Count} field args, got {argCtxs.Count}");
        var value = info.Type.Undef;
        for (var i = 0; i < argCtxs.Count; i++)
            value = Builder.BuildInsertValue(value, args[i], (uint)i, $"{name}.f{i}");
        return value;
    }

    public override LLVMValueRef VisitQualified_name(ExodiaParser.Qualified_nameContext context)
    {
        var name = context.GetText();
        if (_symbols.TryGetValue(name, out var sym))
            return Builder.BuildLoad2(sym.Type, sym.Slot, name);

        throw new NotSupportedException($"Unknown name '{name}'");
    }
}