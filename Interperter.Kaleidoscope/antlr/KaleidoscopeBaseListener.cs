//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from D:/workspaces/compilers/cspg/Grammer.ParserGenerator/Interperter.Kaleidoscope/Kaleidoscope.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419


using Antlr4.Runtime.Misc;
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IKaleidoscopeListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.Diagnostics.DebuggerNonUserCode]
[System.CLSCompliant(false)]
public partial class KaleidoscopeBaseListener : IKaleidoscopeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.program"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterProgram([NotNull] KaleidoscopeParser.ProgramContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.program"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitProgram([NotNull] KaleidoscopeParser.ProgramContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpression([NotNull] KaleidoscopeParser.ExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpression([NotNull] KaleidoscopeParser.ExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.definition"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDefinition([NotNull] KaleidoscopeParser.DefinitionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.definition"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDefinition([NotNull] KaleidoscopeParser.DefinitionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.external"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExternal([NotNull] KaleidoscopeParser.ExternalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.external"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExternal([NotNull] KaleidoscopeParser.ExternalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.prototype"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrototype([NotNull] KaleidoscopeParser.PrototypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.prototype"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrototype([NotNull] KaleidoscopeParser.PrototypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>continuedBinaryExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.binaryExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterContinuedBinaryExpression([NotNull] KaleidoscopeParser.ContinuedBinaryExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>continuedBinaryExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.binaryExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitContinuedBinaryExpression([NotNull] KaleidoscopeParser.ContinuedBinaryExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>finalBinaryExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.binaryExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFinalBinaryExpression([NotNull] KaleidoscopeParser.FinalBinaryExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>finalBinaryExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.binaryExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFinalBinaryExpression([NotNull] KaleidoscopeParser.FinalBinaryExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>callExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.identifierExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCallExpression([NotNull] KaleidoscopeParser.CallExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>callExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.identifierExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCallExpression([NotNull] KaleidoscopeParser.CallExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>variableExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.identifierExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVariableExpression([NotNull] KaleidoscopeParser.VariableExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>variableExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.identifierExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVariableExpression([NotNull] KaleidoscopeParser.VariableExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.numberExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNumberExpression([NotNull] KaleidoscopeParser.NumberExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.numberExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNumberExpression([NotNull] KaleidoscopeParser.NumberExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.parenExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParenExpression([NotNull] KaleidoscopeParser.ParenExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.parenExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParenExpression([NotNull] KaleidoscopeParser.ParenExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.primary"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrimary([NotNull] KaleidoscopeParser.PrimaryContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.primary"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrimary([NotNull] KaleidoscopeParser.PrimaryContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}