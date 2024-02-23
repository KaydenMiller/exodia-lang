using System.Text;
using Antlr4.Runtime;
using Interperter.Kaleidoscope;

Console.WriteLine("Building Kaleidoscope Lang");

try
{
    var text = File.ReadAllText("./tests/simple-script.ex", Encoding.UTF8);

    var inputStream = new AntlrInputStream(text);
    var parserLexer = new KaleidoscopeLexer(inputStream);
    var commonTokenStream = new CommonTokenStream(parserLexer);
    var parser = new KaleidoscopeParser(commonTokenStream);
    var tree = parser.program();
    var eval = new KaleidoscopeVisitor();
    eval.Visit(tree);
}
catch (Exception ex)
{
    Console.WriteLine("Errors" + ex);
}