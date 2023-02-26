using FluentAssertions;
using Interperter.ExodiaLang;

namespace Interpreter.Test.Unit;

public class Literals
{
    [Theory]
    [InlineData("1", 1)]
    [InlineData("100", 100)]
    [InlineData("9", 9)]
    [InlineData("0", 0)]
    public void GivenNumber_WhenValidInt_ShouldCreateIntLiteral(string value, int expected)
    {
        // Arrange
        var parser = Helpers.SetupParser(value);
        var stack = new Stack<object>();
        
        // Act
        var program = parser.program();
        var visitor = new EvalExodiaVisitor(stack);
        visitor.Visit(program);

        // Assert
        var top = stack.Pop();
        top.Should().BeOfType<int>();
        ((int)top).Should().Be(expected);
    }
}