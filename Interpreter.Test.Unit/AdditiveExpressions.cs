using FluentAssertions;
using Interperter.ExodiaLang;

namespace Interpreter.Test.Unit;

public class AdditiveExpressions
{
    [Theory]
    [InlineData("1 + 2;", 3)]
    [InlineData("5 + 5;", 10)]
    [InlineData("100 + 5;", 105)]
    [InlineData("100 - 5;", 95)]
    public void GivenValidInput_WhenAddExpression_ShouldResolveToCorrectValue(string value, int expected)
    {
        // Arrange
        var parser = Helpers.SetupParser(value);
        var stack = new Stack<object>();

        // Act
        var program = parser.additive_expression();
        var visitor = new EvalExodiaVisitor(stack);
        var result = visitor.Visit(program);

        // Assert
        result.Should().Be(expected);
    }
}