using FluentAssertions;
using Interperter.ExodiaLang;

namespace Interpreter.Test.Unit;

public class MultiplicativeExpressions
{
    [Theory]
    [InlineData("1 * 1", 1)]
    [InlineData("1 * 2", 2)]
    [InlineData("2 * 5", 10)]
    [InlineData("0 * 56", 0)]
    public void GivenValidInput_WhenMultiExpression_ShouldResolveToCorrectValue(string value, int expected)
    {
        // Arrange
        var parser = Helpers.SetupParser(value);

        // Act
        var program = parser.multiplicative_expression();
        var visitor = new EvalExodiaVisitor(new Stack<object>());
        var result = visitor.Visit(program);

        // Assert
        result.Should().Be(expected);
    }
}