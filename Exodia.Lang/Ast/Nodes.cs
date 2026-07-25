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

public record Param(string Name, TypeRef Type, TextSpan TextSpan);


public record ProgramNode(IReadOnlyList<FnDeclaration> Functions, TextSpan TextSpan) : AstNode(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitProgram(this);
}

public record FnDeclaration(string Name, IReadOnlyList<Param> Params, TypeRef ReturnType, Block Body, TextSpan TextSpan) : AstNode(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitFnDeclaration(this);
}

public record VariableDeclaration(string Name, bool IsMutable, TypeRef? Type, Expr Initializer, TextSpan TextSpan) : Statement(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitVariableDeclaration(this);
}

public record NameRef(string Name, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitNameRef(this);
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

public record ExpressionStatement(Expr Expression, TextSpan TextSpan) : Statement(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitExpressionStatement(this);
}

public record CallExpr(string Callee, IReadOnlyList<Expr> Args, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitCall(this);
}

public record AssignExpr(Expr Target, Expr Value, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitAssignmentExpr(this);
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
