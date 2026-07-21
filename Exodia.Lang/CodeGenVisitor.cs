using LLVMSharp.Interop;

namespace Exodia.Lang;

public class CodeGenVisitor : ExodiaBaseVisitor<LLVMValueRef>
{
    public readonly LLVMModuleRef Module;
    public readonly LLVMBuilderRef Builder;
    private readonly Dictionary<string, LLVMValueRef> _symbols = [];

    public CodeGenVisitor(LLVMModuleRef module)
    {
        Module = module;
        Builder = Module.Context.CreateBuilder();
    }
    
    // fn <name>(...): int32 { ... }   -- for now assume i32 return, no params
    public override LLVMValueRef VisitFunction_declaration(ExodiaParser.Function_declarationContext context)
    {
        var name = context.identifier().GetText(); // "main"
        var fnType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, Array.Empty<LLVMTypeRef>());
        var fn = Module.AddFunction(name, fnType);

        var entry = fn.AppendBasicBlock("entry");
        Builder.PositionAtEnd(entry);

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
        var value = ulong.Parse(context.GetText());
        return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, value);
    }

    public override LLVMValueRef VisitAdditive_expression(ExodiaParser.Additive_expressionContext context)
    {
        if (context.op == null)
            return Visit(context.multiplicative_expression());

        var left = Visit(context.left);
        var right = Visit(context.right);
        return context.op.Text switch
        {
            "+" => Builder.BuildAdd(left, right, "add"),
            "-" => Builder.BuildSub(left, right, "sub"),
            _ => throw new NotSupportedException($"Additive op '{context.op.Text}'")
        };
    }

    public override LLVMValueRef VisitMultiplicative_expression(ExodiaParser.Multiplicative_expressionContext context)
    {
        if (context.op == null)
            return Visit(context.cast_expression());

        var left = Visit(context.left);
        var right = Visit(context.right);
        return context.op.Text switch
        {
            "*" => Builder.BuildMul(left, right, "mul"),
            "/" => Builder.BuildSDiv(left, right, "div"),
            _ => throw new NotSupportedException($"Multiplicative op '{context.op.Text}'")
        };
    }

    public override LLVMValueRef VisitParenthesized_expression(ExodiaParser.Parenthesized_expressionContext context)
    {
        return Visit(context.expression());
    }

    public override LLVMValueRef VisitVariable_statement(ExodiaParser.Variable_statementContext context)
        => Visit(context.variable_declaration_list());

    public override LLVMValueRef VisitVariable_declaration(ExodiaParser.Variable_declarationContext context)
    {
        var id = context.identifier().GetText();
        var type = context.type()?.GetText();
        var value = Visit(context.variable_initializer());
        var slot = Builder.BuildAlloca(LLVMTypeRef.Int32, id);
        Builder.BuildStore(value, slot);
        _symbols[id] = slot;
        return value;
    }

    public override LLVMValueRef VisitQualified_name(ExodiaParser.Qualified_nameContext context)
    {
        var name = context.GetText();
        if (_symbols.TryGetValue(name, out var slot))
            return Builder.BuildLoad2(LLVMTypeRef.Int32, slot, name);

        throw new NotSupportedException($"Unknown name '{name}'");
    }
}