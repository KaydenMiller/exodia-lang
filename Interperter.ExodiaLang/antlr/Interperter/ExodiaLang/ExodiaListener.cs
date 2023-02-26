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

namespace Interperter.ExodiaLang {
using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="ExodiaParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.11.1")]
[System.CLSCompliant(false)]
public interface IExodiaListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProgram([NotNull] ExodiaParser.ProgramContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProgram([NotNull] ExodiaParser.ProgramContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.statement_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement_list([NotNull] ExodiaParser.Statement_listContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.statement_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement_list([NotNull] ExodiaParser.Statement_listContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] ExodiaParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] ExodiaParser.StatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.class_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClass_declaration([NotNull] ExodiaParser.Class_declarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.class_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClass_declaration([NotNull] ExodiaParser.Class_declarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.class_extends"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClass_extends([NotNull] ExodiaParser.Class_extendsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.class_extends"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClass_extends([NotNull] ExodiaParser.Class_extendsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.iteration_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIteration_statement([NotNull] ExodiaParser.Iteration_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.iteration_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIteration_statement([NotNull] ExodiaParser.Iteration_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.for_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFor_statement([NotNull] ExodiaParser.For_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.for_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFor_statement([NotNull] ExodiaParser.For_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.do_while_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDo_while_statement([NotNull] ExodiaParser.Do_while_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.do_while_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDo_while_statement([NotNull] ExodiaParser.Do_while_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.while_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhile_statement([NotNull] ExodiaParser.While_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.while_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhile_statement([NotNull] ExodiaParser.While_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.variable_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariable_statement([NotNull] ExodiaParser.Variable_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.variable_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariable_statement([NotNull] ExodiaParser.Variable_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.variable_declaration_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariable_declaration_list([NotNull] ExodiaParser.Variable_declaration_listContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.variable_declaration_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariable_declaration_list([NotNull] ExodiaParser.Variable_declaration_listContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.variable_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariable_declaration([NotNull] ExodiaParser.Variable_declarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.variable_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariable_declaration([NotNull] ExodiaParser.Variable_declarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.variable_initializer"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariable_initializer([NotNull] ExodiaParser.Variable_initializerContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.variable_initializer"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariable_initializer([NotNull] ExodiaParser.Variable_initializerContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.if_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIf_statement([NotNull] ExodiaParser.If_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.if_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIf_statement([NotNull] ExodiaParser.If_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.empty_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEmpty_statement([NotNull] ExodiaParser.Empty_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.empty_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEmpty_statement([NotNull] ExodiaParser.Empty_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.return_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturn_statement([NotNull] ExodiaParser.Return_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.return_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturn_statement([NotNull] ExodiaParser.Return_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.block_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBlock_statement([NotNull] ExodiaParser.Block_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.block_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBlock_statement([NotNull] ExodiaParser.Block_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.function_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunction_declaration([NotNull] ExodiaParser.Function_declarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.function_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunction_declaration([NotNull] ExodiaParser.Function_declarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.formal_parameter_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFormal_parameter_list([NotNull] ExodiaParser.Formal_parameter_listContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.formal_parameter_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFormal_parameter_list([NotNull] ExodiaParser.Formal_parameter_listContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.expression_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_statement([NotNull] ExodiaParser.Expression_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.expression_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_statement([NotNull] ExodiaParser.Expression_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression([NotNull] ExodiaParser.ExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression([NotNull] ExodiaParser.ExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.assignment_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssignment_expression([NotNull] ExodiaParser.Assignment_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.assignment_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssignment_expression([NotNull] ExodiaParser.Assignment_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.assignment_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssignment_operator([NotNull] ExodiaParser.Assignment_operatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.assignment_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssignment_operator([NotNull] ExodiaParser.Assignment_operatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.left_hand_side_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLeft_hand_side_expression([NotNull] ExodiaParser.Left_hand_side_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.left_hand_side_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLeft_hand_side_expression([NotNull] ExodiaParser.Left_hand_side_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.member_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMember_expression([NotNull] ExodiaParser.Member_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.member_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMember_expression([NotNull] ExodiaParser.Member_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.this_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterThis_expression([NotNull] ExodiaParser.This_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.this_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitThis_expression([NotNull] ExodiaParser.This_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.identifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIdentifier([NotNull] ExodiaParser.IdentifierContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.identifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIdentifier([NotNull] ExodiaParser.IdentifierContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.logical_OR_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogical_OR_expression([NotNull] ExodiaParser.Logical_OR_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.logical_OR_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogical_OR_expression([NotNull] ExodiaParser.Logical_OR_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.logical_AND_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogical_AND_expression([NotNull] ExodiaParser.Logical_AND_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.logical_AND_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogical_AND_expression([NotNull] ExodiaParser.Logical_AND_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.equality_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEquality_expression([NotNull] ExodiaParser.Equality_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.equality_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEquality_expression([NotNull] ExodiaParser.Equality_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.relational_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRelational_expression([NotNull] ExodiaParser.Relational_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.relational_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRelational_expression([NotNull] ExodiaParser.Relational_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.additive_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAdditive_expression([NotNull] ExodiaParser.Additive_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.additive_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAdditive_expression([NotNull] ExodiaParser.Additive_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.multiplicative_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMultiplicative_expression([NotNull] ExodiaParser.Multiplicative_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.multiplicative_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMultiplicative_expression([NotNull] ExodiaParser.Multiplicative_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.unary_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnary_expression([NotNull] ExodiaParser.Unary_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.unary_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnary_expression([NotNull] ExodiaParser.Unary_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.call_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCall_expression([NotNull] ExodiaParser.Call_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.call_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCall_expression([NotNull] ExodiaParser.Call_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.super"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSuper([NotNull] ExodiaParser.SuperContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.super"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSuper([NotNull] ExodiaParser.SuperContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.callee"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCallee([NotNull] ExodiaParser.CalleeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.callee"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCallee([NotNull] ExodiaParser.CalleeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.arguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArguments([NotNull] ExodiaParser.ArgumentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.arguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArguments([NotNull] ExodiaParser.ArgumentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.argument_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArgument_list([NotNull] ExodiaParser.Argument_listContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.argument_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArgument_list([NotNull] ExodiaParser.Argument_listContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.new_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNew_expression([NotNull] ExodiaParser.New_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.new_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNew_expression([NotNull] ExodiaParser.New_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.primary_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrimary_expression([NotNull] ExodiaParser.Primary_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.primary_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrimary_expression([NotNull] ExodiaParser.Primary_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.parenthesized_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParenthesized_expression([NotNull] ExodiaParser.Parenthesized_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.parenthesized_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParenthesized_expression([NotNull] ExodiaParser.Parenthesized_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLiteral([NotNull] ExodiaParser.LiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLiteral([NotNull] ExodiaParser.LiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.true_literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTrue_literal([NotNull] ExodiaParser.True_literalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.true_literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTrue_literal([NotNull] ExodiaParser.True_literalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.false_literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFalse_literal([NotNull] ExodiaParser.False_literalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.false_literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFalse_literal([NotNull] ExodiaParser.False_literalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.numeric_literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumeric_literal([NotNull] ExodiaParser.Numeric_literalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.numeric_literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumeric_literal([NotNull] ExodiaParser.Numeric_literalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ExodiaParser.string_literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterString_literal([NotNull] ExodiaParser.String_literalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ExodiaParser.string_literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitString_literal([NotNull] ExodiaParser.String_literalContext context);
}
} // namespace Interperter.ExodiaLang
