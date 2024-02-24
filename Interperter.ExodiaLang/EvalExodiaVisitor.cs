using LLVMSharp.Interop;

namespace Interperter.ExodiaLang;

public class EvalExodiaVisitor : ExodiaBaseVisitor<LLVMValueRef>
{
    private readonly Dictionary<string, int> _memory = new();
    private readonly Stack<object> _stack;

    public EvalExodiaVisitor(Stack<object> stack)
    {
        _stack = stack;
    }

    // public override LLVMValueRef VisitProgram(ExodiaParser.ProgramContext context)
    // {
    //     foreach (var statement in context.statement())
    //     {
    //         Visit(statement);
    //     }
    //
    //     return 0;
    // }
    //
    // public override LLVMValueRef VisitStatement(ExodiaParser.StatementContext context)
    // {
    //     Visit(context.expression_statement());
    //     return 0;
    // }
    //
    // public override LLVMValueRef VisitExpression_statement(ExodiaParser.Expression_statementContext context)
    // {
    //     Visit(context.expression());
    //     return 0;
    // }
    //
    // public override LLVMValueRef VisitExpression(ExodiaParser.ExpressionContext context)
    // {
    //     Visit(context.additive_expression());
    //     return 0;
    // }
    //
    // public override LLVMValueRef VisitAdditive_expression(ExodiaParser.Additive_expressionContext context)
    // {
    //     var left = context.left is not null
    //         ? Visit(context.left) 
    //         : Visit(context.multiplicative_expression());
    //     var right = context.right is not null
    //         ? Visit(context.right)
    //         : 0;
    //
    //     if (context.ADDITIVE_OPERATOR()?.GetText() == "-")
    //     {
    //         return left - right;
    //     }
    //     return left + right;
    // }
    //
    // public override LLVMValueRef VisitMultiplicative_expression(ExodiaParser.Multiplicative_expressionContext context)
    // {
    //     var left = context.left is not null 
    //         ? Visit(context.left)
    //         : Visit(context.unary_expression());
    //     var right = context.right is not null
    //         ? Visit(context.right)
    //         : 1;
    //     return left * right;
    // }
    //
    // public override LLVMValueRef VisitUnary_expression(ExodiaParser.Unary_expressionContext context)
    // {
    //     var value = Visit(context.primary_expression());
    //     if (context.ADDITIVE_OPERATOR()?.GetText() == "-")
    //         return -value;
    //     return value;
    // }
    //
    // public override LLVMValueRef VisitPrimary_expression(ExodiaParser.Primary_expressionContext context)
    // {
    //     return Visit(context.literal());
    // }
    //
    // public override LLVMValueRef VisitNumeric_literal(ExodiaParser.Numeric_literalContext context)
    // {
    //     var value = int.Parse(context.INT().GetText());
    //     
    //     _stack.Push(value);
    //     return value;
    // }
}