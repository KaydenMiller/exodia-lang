using System.Text;
using Antlr4.Runtime;
using Interperter.ExodiaLang;

try
{
    var text = new StringBuilder();

    var lines = File.ReadAllLines("./tests/simple-script.ex", Encoding.UTF8);

    foreach (var line in lines)
    {
        text.AppendLine(line);
    }

    var inputStream = new AntlrInputStream(text.ToString());
    var exodiaParserLexer = new ExodiaLexer(inputStream);
    var commonTokenStream = new CommonTokenStream(exodiaParserLexer);
    var parser = new ExodiaParser(commonTokenStream);

    var tree = parser.program();

    var eval = new EvalExodiaVisitor(new Stack<object>());
    eval.Visit(tree);
}
catch (Exception ex)
{
    Console.WriteLine("Errors" + ex);
}