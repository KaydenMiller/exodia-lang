using Antlr4.Runtime;
using Interperter.ExodiaLang;

namespace Interpreter.Test.Unit;

public static class Helpers
{
    public static ExodiaParser SetupParser(string text)
    {
        var inputStream = new AntlrInputStream(text);
        var lexer = new ExodiaLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        return new ExodiaParser(tokenStream);
    } 
}