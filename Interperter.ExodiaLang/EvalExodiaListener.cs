namespace Interperter.ExodiaLang;

public class EvalExodiaListener : ExodiaBaseListener
{
    private readonly Stack<object> _stack;

    public EvalExodiaListener(Stack<object> stack)
    {
        _stack = stack;
    }
}