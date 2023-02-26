using Antlr4.Runtime.Tree;

namespace Interperter.ExodiaLang;

public class EvalExodiaVisitor : ExodiaBaseVisitor<object>
{
    private readonly Stack<object> _stack;

    public EvalExodiaVisitor(Stack<object> stack)
    {
        _stack = stack;
    }

    public override object VisitNumeric_literal(ExodiaParser.Numeric_literalContext context)
    {
        // var value = int.Parse(context.INT());
        //
        // _stack.Push(value);

        // return value;
        return base.VisitNumeric_literal(context);
    }

    public override object VisitAdditive_expression(ExodiaParser.Additive_expressionContext context)
    {
        var op = context.op.Text;

        if (op.Equals("+"))
        {
            // return Visit(context.left) + Visit(context.right);
        }
        else if (op.Equals("-"))
        {
            throw new NotImplementedException();
        }
        
        return base.VisitAdditive_expression(context);
    }
}