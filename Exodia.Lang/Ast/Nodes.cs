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
public record DynType(string InterfaceName, TextSpan TextSpan) : TypeRef(TextSpan);
// A generic type application written in a type position, e.g. `Box<int32>` or `Box<T>`.
// TypeArgs may themselves be type parameters (resolved via the active substitution env).
public record GenericType(string Name, IReadOnlyList<TypeRef> TypeArgs, TextSpan TextSpan) : TypeRef(TextSpan);

public record TypeParam(string Name, IReadOnlyList<string> Bounds, TextSpan TextSpan);

public record Param(string Name, TypeRef Type, TextSpan TextSpan);


public record ProgramNode(
    IReadOnlyList<FnDeclaration> Functions,
    IReadOnlyList<StructDeclaration> Structs,
    IReadOnlyList<InterfaceDeclaration> Interfaces,
    IReadOnlyList<ImplDeclaration> Impls,
    IReadOnlyList<EnumDeclaration> Enums,
    TextSpan TextSpan) : AstNode(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitProgram(this);
}

public record MatchArm(Pattern Pattern, Expr? Guard, Expr? BodyExpr, Block? BodyBlock, TextSpan TextSpan);
public record MatchExpr(Expr Scrutinee, IReadOnlyList<MatchArm> Arms, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitMatch(this);
}

// --- enums (data; processed by AstVisitor's RegisterEnum phase, not visited) ---
public record EnumVariant(string Name, IReadOnlyList<TypeRef> PayloadTypes, TextSpan TextSpan);
public record EnumDeclaration(string Name, IReadOnlyList<TypeParam> TypeParams, IReadOnlyList<EnumVariant> Variants, TextSpan TextSpan);

// --- patterns (sort of their own; consumed by match codegen, not visited via Accept) ---
public abstract record Pattern(TextSpan TextSpan);
public record VariantPattern(string VariantName, IReadOnlyList<Pattern> Payload, string? Binding, TextSpan TextSpan) : Pattern(TextSpan);  // Some(x), or `Red r` (Binding="r")
public record NamePattern(string Name, TextSpan TextSpan) : Pattern(TextSpan);        // bare name: payload-less variant (None) OR a binding -- resolved at match time
public record WildcardPattern(TextSpan TextSpan) : Pattern(TextSpan);                 // _

// --- interfaces / impls (data; processed by AstVisitor's phases, not visited) ---
public record MethodSignature(string Name, IReadOnlyList<TypeParam> TypeParams, IReadOnlyList<Param> Params, TypeRef ReturnType, TextSpan TextSpan);
public record InterfaceDeclaration(string Name, IReadOnlyList<TypeParam> TypeParams, IReadOnlyList<MethodSignature> Methods, TextSpan TextSpan);
public record ImplDeclaration(string InterfaceName, string TargetType, IReadOnlyList<TypeParam> TypeParams, IReadOnlyList<MethodDeclaration> Methods, TextSpan TextSpan);

// --- struct declarations (data; processed by AstVisitor's 3-phase, not visited) ---
public record FieldDeclaration(string Name, TypeRef Type, TextSpan TextSpan);
public record ConstructorDeclaration(string? Name, IReadOnlyList<Param> Params, Block Body, TextSpan TextSpan);
public record MethodDeclaration(string Name, IReadOnlyList<TypeParam> TypeParams, IReadOnlyList<Param> Params, TypeRef ReturnType, Block Body, TextSpan TextSpan);
public record StructDeclaration(
    string Name,
    IReadOnlyList<TypeParam> TypeParams,
    IReadOnlyList<FieldDeclaration> Fields,
    IReadOnlyList<ConstructorDeclaration> Constructors,
    IReadOnlyList<MethodDeclaration> Methods,
    TextSpan TextSpan);

// --- struct expressions (visited) ---
public record NewExpr(string StructName, IReadOnlyList<TypeRef> TypeArgs, string? ConstructorName, IReadOnlyList<Expr> Args, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitNew(this);
}
// explicit generic-enum construction: `Option<float>::Some(1.4f)`
public record EnumConstructExpr(string EnumName, IReadOnlyList<TypeRef> TypeArgs, string VariantName, IReadOnlyList<Expr> Args, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitEnumConstruct(this);
}
public record FieldAccess(Expr Target, string FieldName, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitFieldAccess(this);
}
public record MethodCall(Expr Receiver, string MethodName, IReadOnlyList<Expr> Args, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitMethodCall(this);
}
public record ThisExpr(TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitThis(this);
}

public record FnDeclaration(
    string Name,
    IReadOnlyList<TypeParam> TypeParams,
    IReadOnlyList<Param> Params,
    TypeRef ReturnType, 
    Block? Body,
    bool IsExtern,
    string? LinkName,
    TextSpan TextSpan) : AstNode(TextSpan)
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

public record UnaryExpr(string Operation, Expr Operand, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitUnary(this);
}

public record WhileStatement(Expr Condition, Statement Body, TextSpan TextSpan) : Statement(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitWhile(this);
}

public record IntLiteral(ulong Value, TypeRef? Type, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitIntLiteral(this);
}

public record FloatLiteral(double Value, TypeRef? Type, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitFloatLiteral(this);
}

public record BoolLiteral(bool Value, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitBoolLiteral(this);
}

public record StringLiteral(string Value, TextSpan TextSpan) : Expr(TextSpan)
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitStringLiteral(this);
}