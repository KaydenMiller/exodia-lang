using Antlr4.Runtime;
using Exodia.Lang;
using LLVMSharp.Interop;

try
{
    // LEXING and PARSING
    var charStream = CharStreams.fromStream(Console.OpenStandardInput());

    var lexer = new ExodiaLexer(charStream);
    var tokens = new CommonTokenStream(lexer);
    var parser = new ExodiaParser(tokens);

    var tree = parser.program();
    Console.WriteLine(tree.ToStringTree(parser));
    
    // LLVM
    var module = LLVMModuleRef.CreateWithName("exodia");
    var codegen = new CodeGenVisitor(module);
    Console.WriteLine(codegen.EmitTrivialMain());
}
catch (Exception ex)
{
    Console.WriteLine("Errors" + ex);
}