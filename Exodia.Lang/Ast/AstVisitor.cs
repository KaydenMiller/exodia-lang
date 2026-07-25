using LLVMSharp;
using LLVMSharp.Interop;

namespace Exodia.Lang.Ast;


public interface IAstVisitor<T>
{
    T VisitProgram(ProgramNode node);
    T VisitFnDeclaration(FnDeclaration node);
    T VisitBlock(Block node);
    T VisitIf(IfStatement node);
    T VisitReturn(ReturnStatement node);
    T VisitCast(CastExpr node);
    T VisitBinary(BinaryExpr node);
    T VisitIntLiteral(IntLiteral node);
    T VisitFloatLiteral(FloatLiteral node);
}

public class AstVisitor : IAstVisitor<LLVMValueRef>
{
    public readonly LLVMModuleRef _module;
    public readonly LLVMBuilderRef _builder;

    public AstVisitor(LLVMModuleRef module)
    {
        _module = module;
        _builder = _module.Context.CreateBuilder();
    }
    
    public LLVMValueRef VisitProgram(ProgramNode node)
    {
        foreach (var fn in node.Functions)
            fn.Accept(this);
        return default;
    }

    public LLVMValueRef VisitFnDeclaration(FnDeclaration node)
    {
        var fnType = LLVMTypeRef.CreateFunction(ResolveType(node.ReturnType), []);
        var fn = _module.AddFunction(node.Name, fnType);
        _builder.PositionAtEnd(fn.AppendBasicBlock("entry"));
        node.Body.Accept(this);
        return fn;
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

    public LLVMValueRef VisitCast(CastExpr node)
    {
        return _builder.EmitCast(node.Value.Accept(this), ResolveType(node.Target));
    }

    public LLVMValueRef VisitBinary(BinaryExpr node)
    {
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

    public LLVMValueRef VisitIntLiteral(IntLiteral node)
    {
        return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, node.Value);
    }

    public LLVMValueRef VisitFloatLiteral(FloatLiteral node)
    {
        return LLVMValueRef.CreateConstReal(LLVMTypeRef.Double, node.Value);
    }

    private LLVMTypeRef ResolveType(TypeRef type) => type switch
    {
        NamedType n => AstHelpers.MapPrimitiveType(n.Name),
        _ => throw new NotSupportedException($"type {type} not supported yet")
    };
}

