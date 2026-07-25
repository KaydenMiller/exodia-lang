namespace Exodia.Lang.Ast;

public interface INode
{
    public T Accept<T>(IAstVisitor<T> visitor);
}

public record TextSpan(int Start, int Length);

public abstract record AstNode(TextSpan TextSpan) : INode
{
    public abstract T Accept<T>(IAstVisitor<T> visitor);
}
public abstract record Statement(TextSpan TextSpan) : AstNode(TextSpan);
public abstract record Expr(TextSpan TextSpan) : AstNode(TextSpan);

public abstract record TypeRef(TextSpan TextSpan);
public record NamedType(string Name, TextSpan TextSpan) : TypeRef(TextSpan);


public record ProgramNode(IReadOnlyList<FnDeclaration> Functions, TextSpan TextSpan) : AstNode(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitProgram(this);
}

public record FnDeclaration(string Name, TypeRef ReturnType, Block Body, TextSpan TextSpan) : AstNode(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitFnDeclaration(this);
}

public record Block(IReadOnlyList<Statement> Statements, TextSpan TextSpan) : Statement(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitBlock(this);
}

public record IfStatement(Expr Condition, Statement Then, Statement? Else, TextSpan TextSpan) : Statement(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitIf(this);
}

public record ReturnStatement(Expr? Value, TextSpan TextSpan) : Statement(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitReturn(this);
}

public record CastExpr(Expr Value, TypeRef Target, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitCast(this);
}

public record BinaryExpr(Expr Left, string Operation, Expr Right, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitBinary(this);
}

public record IntLiteral(ulong Value, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitIntLiteral(this);
}

public record FloatLiteral(double Value, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitFloatLiteral(this);
}
