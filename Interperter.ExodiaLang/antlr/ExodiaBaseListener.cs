//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.11.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from D:/workspaces/compilers/cspg/Grammer.ParserGenerator/Interperter.ExodiaLang\Exodia.g4 by ANTLR 4.11.1

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
/// This class provides an empty implementation of <see cref="IExodiaListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.11.1")]
[System.Diagnostics.DebuggerNonUserCode]
[System.CLSCompliant(false)]
public partial class ExodiaBaseListener : IExodiaListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.program"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterProgram([NotNull] ExodiaParser.ProgramContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.program"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitProgram([NotNull] ExodiaParser.ProgramContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStatement([NotNull] ExodiaParser.StatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStatement([NotNull] ExodiaParser.StatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.class_declaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterClass_declaration([NotNull] ExodiaParser.Class_declarationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.class_declaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitClass_declaration([NotNull] ExodiaParser.Class_declarationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.class_extends"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterClass_extends([NotNull] ExodiaParser.Class_extendsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.class_extends"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitClass_extends([NotNull] ExodiaParser.Class_extendsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.iteration_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIteration_statement([NotNull] ExodiaParser.Iteration_statementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.iteration_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIteration_statement([NotNull] ExodiaParser.Iteration_statementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.for_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFor_statement([NotNull] ExodiaParser.For_statementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.for_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFor_statement([NotNull] ExodiaParser.For_statementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.do_while_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDo_while_statement([NotNull] ExodiaParser.Do_while_statementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.do_while_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDo_while_statement([NotNull] ExodiaParser.Do_while_statementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.while_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWhile_statement([NotNull] ExodiaParser.While_statementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.while_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWhile_statement([NotNull] ExodiaParser.While_statementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.variable_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVariable_statement([NotNull] ExodiaParser.Variable_statementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.variable_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVariable_statement([NotNull] ExodiaParser.Variable_statementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.variable_declaration_list"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVariable_declaration_list([NotNull] ExodiaParser.Variable_declaration_listContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.variable_declaration_list"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVariable_declaration_list([NotNull] ExodiaParser.Variable_declaration_listContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.variable_declaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVariable_declaration([NotNull] ExodiaParser.Variable_declarationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.variable_declaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVariable_declaration([NotNull] ExodiaParser.Variable_declarationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.variable_initializer"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVariable_initializer([NotNull] ExodiaParser.Variable_initializerContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.variable_initializer"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVariable_initializer([NotNull] ExodiaParser.Variable_initializerContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.if_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIf_statement([NotNull] ExodiaParser.If_statementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.if_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIf_statement([NotNull] ExodiaParser.If_statementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.empty_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEmpty_statement([NotNull] ExodiaParser.Empty_statementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.empty_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEmpty_statement([NotNull] ExodiaParser.Empty_statementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.return_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReturn_statement([NotNull] ExodiaParser.Return_statementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.return_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReturn_statement([NotNull] ExodiaParser.Return_statementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.block_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBlock_statement([NotNull] ExodiaParser.Block_statementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.block_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBlock_statement([NotNull] ExodiaParser.Block_statementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.function_declaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFunction_declaration([NotNull] ExodiaParser.Function_declarationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.function_declaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFunction_declaration([NotNull] ExodiaParser.Function_declarationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.formal_parameter_list"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFormal_parameter_list([NotNull] ExodiaParser.Formal_parameter_listContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.formal_parameter_list"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFormal_parameter_list([NotNull] ExodiaParser.Formal_parameter_listContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.expression_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpression_statement([NotNull] ExodiaParser.Expression_statementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.expression_statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpression_statement([NotNull] ExodiaParser.Expression_statementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpression([NotNull] ExodiaParser.ExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpression([NotNull] ExodiaParser.ExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.assignment_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAssignment_expression([NotNull] ExodiaParser.Assignment_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.assignment_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAssignment_expression([NotNull] ExodiaParser.Assignment_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.assignment_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAssignment_operator([NotNull] ExodiaParser.Assignment_operatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.assignment_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAssignment_operator([NotNull] ExodiaParser.Assignment_operatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.left_hand_side_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLeft_hand_side_expression([NotNull] ExodiaParser.Left_hand_side_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.left_hand_side_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLeft_hand_side_expression([NotNull] ExodiaParser.Left_hand_side_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.member_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMember_expression([NotNull] ExodiaParser.Member_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.member_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMember_expression([NotNull] ExodiaParser.Member_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.this_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterThis_expression([NotNull] ExodiaParser.This_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.this_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitThis_expression([NotNull] ExodiaParser.This_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.identifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIdentifier([NotNull] ExodiaParser.IdentifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.identifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIdentifier([NotNull] ExodiaParser.IdentifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.logical_OR_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLogical_OR_expression([NotNull] ExodiaParser.Logical_OR_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.logical_OR_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLogical_OR_expression([NotNull] ExodiaParser.Logical_OR_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.logical_AND_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLogical_AND_expression([NotNull] ExodiaParser.Logical_AND_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.logical_AND_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLogical_AND_expression([NotNull] ExodiaParser.Logical_AND_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.equality_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEquality_expression([NotNull] ExodiaParser.Equality_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.equality_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEquality_expression([NotNull] ExodiaParser.Equality_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.relational_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRelational_expression([NotNull] ExodiaParser.Relational_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.relational_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRelational_expression([NotNull] ExodiaParser.Relational_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.additive_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAdditive_expression([NotNull] ExodiaParser.Additive_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.additive_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAdditive_expression([NotNull] ExodiaParser.Additive_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.multiplicative_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMultiplicative_expression([NotNull] ExodiaParser.Multiplicative_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.multiplicative_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMultiplicative_expression([NotNull] ExodiaParser.Multiplicative_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.unary_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnary_expression([NotNull] ExodiaParser.Unary_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.unary_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnary_expression([NotNull] ExodiaParser.Unary_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.call_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCall_expression([NotNull] ExodiaParser.Call_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.call_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCall_expression([NotNull] ExodiaParser.Call_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.super"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSuper([NotNull] ExodiaParser.SuperContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.super"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSuper([NotNull] ExodiaParser.SuperContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.callee"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCallee([NotNull] ExodiaParser.CalleeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.callee"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCallee([NotNull] ExodiaParser.CalleeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArguments([NotNull] ExodiaParser.ArgumentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArguments([NotNull] ExodiaParser.ArgumentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.argument_list"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArgument_list([NotNull] ExodiaParser.Argument_listContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.argument_list"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArgument_list([NotNull] ExodiaParser.Argument_listContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.new_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNew_expression([NotNull] ExodiaParser.New_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.new_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNew_expression([NotNull] ExodiaParser.New_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.primary_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrimary_expression([NotNull] ExodiaParser.Primary_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.primary_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrimary_expression([NotNull] ExodiaParser.Primary_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.parenthesized_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParenthesized_expression([NotNull] ExodiaParser.Parenthesized_expressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.parenthesized_expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParenthesized_expression([NotNull] ExodiaParser.Parenthesized_expressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>atom</c>
	/// labeled alternative in <see cref="ExodiaParser.literal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAtom([NotNull] ExodiaParser.AtomContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>atom</c>
	/// labeled alternative in <see cref="ExodiaParser.literal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAtom([NotNull] ExodiaParser.AtomContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.true_literal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTrue_literal([NotNull] ExodiaParser.True_literalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.true_literal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTrue_literal([NotNull] ExodiaParser.True_literalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.false_literal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFalse_literal([NotNull] ExodiaParser.False_literalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.false_literal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFalse_literal([NotNull] ExodiaParser.False_literalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.numeric_literal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNumeric_literal([NotNull] ExodiaParser.Numeric_literalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.numeric_literal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNumeric_literal([NotNull] ExodiaParser.Numeric_literalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.string_literal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterString_literal([NotNull] ExodiaParser.String_literalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.string_literal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitString_literal([NotNull] ExodiaParser.String_literalContext context) { }

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
