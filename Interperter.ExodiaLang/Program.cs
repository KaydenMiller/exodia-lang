using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Interperter.ExodiaLang;

try
{
    do
    {
        var text = new StringBuilder();
        Console.WriteLine("Input your expression");
        var input = Console.ReadLine();

        if (input == "quit();")
        {
            break;
        }
        
        text.AppendLine(input);

        var inputStream = new AntlrInputStream(text.ToString());
        var exodiaParserLexer = new ExodiaLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(exodiaParserLexer);
        var parser = new ExodiaParser(commonTokenStream);
        var tree = parser.init();
        var walker = new ParseTreeWalker();
        walker.Walk(new EvalExodiaListener(new Stack<object>()), tree);
    } while (true);
}
catch (Exception ex)
{
    Console.WriteLine("Errors" + ex);
}