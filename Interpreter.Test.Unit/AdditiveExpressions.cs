using Antlr4.Runtime.Tree;
using FluentAssertions;
using Interperter.ExodiaLang;

namespace Interpreter.Test.Unit;

public class AdditiveExpressions
{
    [Theory]
    [InlineData("1 + 2;", 3)]
    public void GivenValidInput_WhenAddExpression_ShouldResolveToCorrectValue(string value, int expected)
    {
        // Arrange
        var parser = Helpers.SetupParser(value);
        var stack = new Stack<object>();
        
        // Act
        var program = parser.program();
        var listener = new EvalExodiaListener(stack);
        var walker = new ParseTreeWalker();
        walker.Walk(listener, program);

        // Assert
        var top = stack.Pop();
        top.Should().BeOfType<int>();
        ((int)top).Should().Be(expected);
    }
}