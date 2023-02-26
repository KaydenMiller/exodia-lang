namespace Interperter.ExodiaLang;

public class EvalExodiaVisitor : ExodiaBaseVisitor<int>
{
    private readonly Stack<object> _stack;
    private readonly Dictionary<string, int> _memory = new();

    public EvalExodiaVisitor(Stack<object> stack)
    {
        _stack = stack;
    }

    public override int VisitAssign(ExodiaParser.AssignContext context)
    {
        var id = context.IDENTIFIER().GetText();
        var value = Visit(context.expr());

        _memory.Add(id, value);

        return value;
    }

    public override int VisitPrintExpr(ExodiaParser.PrintExprContext context)
    {
        var value = Visit(context.expr());
        Console.WriteLine(value);
        return 0;
    }

    public override int VisitInt(ExodiaParser.IntContext context)
    {
        return int.Parse(context.INT().GetText());
    }

    public override int VisitId(ExodiaParser.IdContext context)
    {
        var id = context.IDENTIFIER().GetText();
        if (_memory.ContainsKey(id))
        {
            _memory.TryGetValue(id, out var value);
            return value;
        }

        return 0;
    }

    public override int VisitMulDiv(ExodiaParser.MulDivContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (context.op.Type == ExodiaParser.MUL)
        {
            return left * right;
        }

        return left / right;
    }

    public override int VisitAddSub(ExodiaParser.AddSubContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        if (context.op.Type == ExodiaParser.ADD)
        {
            return left + right;
        }

        return left - right;
    }

    public override int VisitParens(ExodiaParser.ParensContext context)
    {
        return Visit(context.expr());
    }
}