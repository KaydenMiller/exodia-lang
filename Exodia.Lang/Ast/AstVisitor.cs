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
}

public class AstVisitor : IAstVisitor<LLVMValueRef>
{
    public readonly LLVMModuleRef _module;
    public readonly LLVMBuilderRef _builder;
    private readonly Dictionary<string, Symbol> _symbols = [];
    private readonly Dictionary<CallableKey, Callable> _functions = [];

    public AstVisitor(LLVMModuleRef module)
    {
        _module = module;
        _builder = _module.Context.CreateBuilder();
    }
    
    public LLVMValueRef VisitProgram(ProgramNode node)
    {
        foreach (var fn in node.Functions) DeclareFunction(fn);       // pass 1: signatures
        foreach (var fn in node.Functions) fn.Accept(this);     // pass 2: bodies
        return default;
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
        _builder.BuildStore(value, _symbols.ResolveLValue(node.Target));
        return value;
    }

    public LLVMValueRef VisitExpressionStatement(ExpressionStatement node)
    {
        node.Expression.Accept(this);
        return default;
    }

    public LLVMValueRef VisitCast(CastExpr node)
    {
        return _builder.EmitCast(node.Value.Accept(this), ResolveType(node.Target));
    }

    public LLVMValueRef VisitCall(CallExpr node)
    {
        var args = node.Args
            .Select(arg => arg.Accept(this))
            .ToArray();
        if (node.Callee == "print")
            return EmitPrint(args);
        var key = new CallableKey("", node.Callee, args.Length);
        if (!_functions.TryGetValue(key, out var target))
            throw new NotSupportedException($"no function '{node.Callee}' taking {args.Length} args");
        return _builder.BuildCall2(target.Signature, target.Fn, args, "call");
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

    private LLVMTypeRef ResolveType(TypeRef type) => type switch
    {
        NamedType n => AstHelpers.MapPrimitiveType(n.Name),
        _ => throw new NotSupportedException($"type {type} not supported yet")
    };
}

