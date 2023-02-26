using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Interperter.ExodiaLang;

try
{
    var input = "";
    var text = new StringBuilder();
    Console.WriteLine("Input your expression");

    while ((input = Console.ReadLine()) != "u0004")
    {
        text.AppendLine(input);
    }

    var inputStream = new AntlrInputStream(text.ToString());
    var exodiaParserLexer = new ExodiaLexer(inputStream);
    var commonTokenStream = new CommonTokenStream(exodiaParserLexer);
    var parser = new ExodiaParser(commonTokenStream);
    var walker = new ParseTreeWalker();
 
   var listener = new EvalExodiaListener(new Stack<object>());
   var program = parser.program();
    walker.Walk(listener, program);
    // var visitor = new EvalExodiaVisitor(new Stack<object>());
    // visitor.Visit(program);
}
catch (Exception ex)
{
    Console.WriteLine("Errors" + ex);
}