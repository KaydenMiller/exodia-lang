using Antlr4.Runtime;

namespace Exodia.Lang.Test;

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