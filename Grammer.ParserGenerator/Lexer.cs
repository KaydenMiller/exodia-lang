using System.Text.RegularExpressions;

namespace Grammer.ParserGenerator;

public interface IParseable
{
    public void Parse();
}

public class Token
{
    private readonly Regex _regex;
    private readonly string _tokenId;

    public Token(Regex regex, string tokenId)
    {
        _regex = regex;
        _tokenId = tokenId;
    }
}

public class Lexer
{
    public List<Token> PossibleTokens = new List<Token>()
    {
        new Token("/b/b", "")
    };
}