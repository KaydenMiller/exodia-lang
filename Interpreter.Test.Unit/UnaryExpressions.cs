using FluentAssertions;
using Interperter.ExodiaLang;

namespace Interpreter.Test.Unit;

public class UnaryExpressions
{
    [Theory]
    [InlineData("-7", -7)]
    public void GivenNegativeSignInput_WhenUnaryExpression_ShouldResolveToNegativeValue(string value, int expected)
    {
        // Arrange
        var parser = Helpers.SetupParser(value);

        // Act
        var program = parser.unary_expression();
        var visitor = new EvalExodiaVisitor(new Stack<object>());
        var result = visitor.Visit(program);

        // Assert
        result.Should().Be(expected);
    }
}