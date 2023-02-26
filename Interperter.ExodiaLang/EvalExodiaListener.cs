namespace Interperter.ExodiaLang;

public class EvalExodiaListener : ExodiaBaseListener
{
    private readonly Stack<object> _stack;

    public EvalExodiaListener(Stack<object> stack)
    {
        _stack = stack;
    }

    public override void EnterInit(ExodiaParser.InitContext context)
    {
        Console.Write("\"");
    }

    public override void ExitInit(ExodiaParser.InitContext context)
    {
        Console.Write("\"");
    }

    public override void EnterValue(ExodiaParser.ValueContext context)
    {
        int value = int.Parse(context.INT().GetText());
        Console.Write(value.ToString("X") + " ");
    }
}