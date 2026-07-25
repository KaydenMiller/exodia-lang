using System.Globalization;
using Antlr4.Runtime;
using LLVMSharp.Interop;

namespace Exodia.Lang.Ast;

public class AstLowering
{
    private readonly ExpressionLowering _expressions = new();
    
    internal static TypeRef LowerType(ExodiaParser.TypeContext context)
        => new NamedType(context.qualified_name().GetText(), Span(context));

    internal static TextSpan Span(ParserRuleContext context)
        => new(context.Start.StartIndex, context.Stop.StopIndex - context.Start.StartIndex + 1);

    public ProgramNode LowerProgram(ExodiaParser.ProgramContext context)
    {
        var functions = new List<FnDeclaration>();
        foreach (var statement in context.statement()) 
            if (statement.function_declaration() is {} fn)
                functions.Add(LowerFunction(fn));
        return new ProgramNode(functions, Span(context));
    }

    private FnDeclaration LowerFunction(ExodiaParser.Function_declarationContext context)
    {
        var formals = context.formal_parameter_list()
            ?.formal_parameter() ?? [];
        var parameters = formals
            .Select(f => 
                new Param(f.identifier().GetText(), LowerType(f.type()), Span(f)))
            .ToList();
        return new FnDeclaration(
            context.identifier().GetText(),
            parameters,
            LowerType(context.type()),
            LowerBody(context.function_body()),
            Span(context));
    }

    private VariableDeclaration LowerVariableDeclaration(ExodiaParser.Variable_statementContext context)
    {
        var declaration = context.variable_declaration_list().variable_declaration();
        var initializer = declaration.variable_initializer()
                          ?? throw new NotSupportedException("variable without initializer not lowered yet");
        return new VariableDeclaration(
            declaration.identifier().GetText(),
            context.MUT() is not null,
            declaration.type() is {} t ? LowerType(t) : null,
            _expressions.Visit(initializer.assignment_expression()),
            Span(context));
    }

    private Block LowerBody(ExodiaParser.Function_bodyContext context)
    {
        var block = context.block_statement()
                    ?? throw new NotSupportedException("expression-bodied functions not lowered yet");
        return LowerBlockStatement(block);
    }

    private Block LowerBlockStatement(ExodiaParser.Block_statementContext context)
    {
        var statements = new List<Statement>();
        foreach (var statement in context.statement())
            statements.Add(LowerStatement(statement));
        return new Block(statements, Span(context));
    }

    private Statement LowerStatement(ExodiaParser.StatementContext context)
    {
        if (context.return_statement() is { } ret)
            return LowerReturn(ret);
        if (context.if_statement() is { } ifstat)
            return LowerIf(ifstat);
        if (context.block_statement() is { } block)
            return LowerBlockStatement(block);
        if (context.variable_statement() is { } var)
            return LowerVariableDeclaration(var);
        if (context.expression_statement() is { } expr)
            return new ExpressionStatement(_expressions.Visit(expr.expression()), Span(expr));
        throw new NotSupportedException($"statement not lowered yet: {context.GetText()}");
    }

    private ReturnStatement LowerReturn(ExodiaParser.Return_statementContext context)
    {
        var value = context.expression() is { } expr 
            ? _expressions.Visit(expr) 
            : null;
        return new ReturnStatement(value, Span(context));
    }

    private IfStatement LowerIf(ExodiaParser.If_statementContext context)
    {
        var cond = _expressions.Visit(context.expression());
        var then = LowerStatement(context.statement(0));
        var @else = context.ELSE() is not null 
            ? LowerStatement(context.statement(1))
            : null;
        return new IfStatement(cond, then, @else, Span(context));
    }
}

public class ExpressionLowering : ExodiaBaseVisitor<Expr>
{
    public override Expr VisitQualified_name(ExodiaParser.Qualified_nameContext context)
    {
        return new NameRef(context.GetText(), AstLowering.Span(context));
    }

    public override Expr VisitNumeric_literal(ExodiaParser.Numeric_literalContext context)
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
            return new FloatLiteral(d, AstLowering.Span(context));
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

        return new IntLiteral(ulong.Parse(intDigits), AstLowering.Span(context));
    }

    public override Expr VisitAssignment_expression(ExodiaParser.Assignment_expressionContext context)
    {
        if (context.assignment_expression() is null)
            return Visit(context.logical_OR_expression());

        var op = context.assignment_operator().GetText();
        if (op is not "=")
            throw new NotSupportedException($"compound assignment '{op}' not lowered yet");

        var target = Visit(context.left_hand_side_expression());
        var value = Visit(context.assignment_expression());
        return new AssignExpr(target, value, AstLowering.Span(context));
    }

    public override Expr VisitAdditive_expression(ExodiaParser.Additive_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.multiplicative_expression());
        return new BinaryExpr(Visit(context.left), context.op.Text, Visit(context.right), AstLowering.Span(context));
    }

    public override Expr VisitMultiplicative_expression(ExodiaParser.Multiplicative_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.cast_expression());
        return new BinaryExpr(Visit(context.left), context.op.Text, Visit(context.right), AstLowering.Span(context));
    }

    public override Expr VisitEquality_expression(ExodiaParser.Equality_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.relational_expression());
        return new BinaryExpr(Visit(context.left), context.op.Text, Visit(context.right), AstLowering.Span(context));
    }

    public override Expr VisitRelational_expression(ExodiaParser.Relational_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.shift_expression());
        return new BinaryExpr(Visit(context.left), context.op.Text, Visit(context.right), AstLowering.Span(context));
    }

    public override Expr VisitCast_expression(ExodiaParser.Cast_expressionContext context)
    {
        var value = Visit(context.unary_expression());
        foreach (var typeCtx in context.type())
            value = new CastExpr(value, AstLowering.LowerType(typeCtx), AstLowering.Span(typeCtx));
        return value;
    }

    public override Expr VisitPostfix_expression(ExodiaParser.Postfix_expressionContext context)
    {
        var ops = context.postfix_op();
        if (ops.Length == 0)
            return Visit(context.primary_expression());

        if (ops.Length == 1 && ops[0].arguments() is { } argsCtx)
        {
            var callee = context.primary_expression().GetText();
            var args = ExodiaHelpers.CollectArgs(argsCtx.argument_list())
                .Select(arg => Visit(arg.assignment_expression()))
                .ToList();
            return new CallExpr(callee, args, AstLowering.Span(context));
        }

        throw new NotSupportedException($"postfix form not lowered yet: {context.GetText()}");
    }
}