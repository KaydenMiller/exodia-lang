//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from /Users/kaydenmiller/Documents/workspaces/personal/exodia-lang/Interperter.Kaleidoscope/Kaleidoscope.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="KaleidoscopeParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public interface IKaleidoscopeListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProgram([NotNull] KaleidoscopeParser.ProgramContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProgram([NotNull] KaleidoscopeParser.ProgramContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression([NotNull] KaleidoscopeParser.ExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression([NotNull] KaleidoscopeParser.ExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.definition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDefinition([NotNull] KaleidoscopeParser.DefinitionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.definition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDefinition([NotNull] KaleidoscopeParser.DefinitionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.external"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExternal([NotNull] KaleidoscopeParser.ExternalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.external"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExternal([NotNull] KaleidoscopeParser.ExternalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.prototype"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrototype([NotNull] KaleidoscopeParser.PrototypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.prototype"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrototype([NotNull] KaleidoscopeParser.PrototypeContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>continuedBinaryExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.binaryExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterContinuedBinaryExpression([NotNull] KaleidoscopeParser.ContinuedBinaryExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>continuedBinaryExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.binaryExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitContinuedBinaryExpression([NotNull] KaleidoscopeParser.ContinuedBinaryExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>finalBinaryExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.binaryExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFinalBinaryExpression([NotNull] KaleidoscopeParser.FinalBinaryExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>finalBinaryExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.binaryExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFinalBinaryExpression([NotNull] KaleidoscopeParser.FinalBinaryExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>callExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.identifierExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCallExpression([NotNull] KaleidoscopeParser.CallExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>callExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.identifierExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCallExpression([NotNull] KaleidoscopeParser.CallExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>variableExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.identifierExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariableExpression([NotNull] KaleidoscopeParser.VariableExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>variableExpression</c>
	/// labeled alternative in <see cref="KaleidoscopeParser.identifierExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariableExpression([NotNull] KaleidoscopeParser.VariableExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.numberExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumberExpression([NotNull] KaleidoscopeParser.NumberExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.numberExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumberExpression([NotNull] KaleidoscopeParser.NumberExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.parenExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParenExpression([NotNull] KaleidoscopeParser.ParenExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.parenExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParenExpression([NotNull] KaleidoscopeParser.ParenExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="KaleidoscopeParser.primary"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrimary([NotNull] KaleidoscopeParser.PrimaryContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="KaleidoscopeParser.primary"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrimary([NotNull] KaleidoscopeParser.PrimaryContext context);
}
