namespace Interperter.ExodiaLang;

public class EvalExodiaListener : ExodiaBaseListener
{
    private readonly Stack<object> _stack;

    public EvalExodiaListener(Stack<object> stack)
    {
        _stack = stack;
    }
    
    public override void ExitNumeric_literal(ExodiaParser.Numeric_literalContext context)
    {
        _stack.Push(int.Parse(context.atom.Text));
        base.ExitNumeric_literal(context);
    }

    public override void ExitAdditive_expression(ExodiaParser.Additive_expressionContext context)
    {
        var right = (int)_stack.Pop();
        var left = (int)_stack.Pop();

        if (context.op.Text == "+")
        {
            _stack.Push(left + right);
        } 
        else if (context.op.Text == "-")
        {
            _stack.Push(left - right);
        }

        base.ExitAdditive_expression(context);
    }
}