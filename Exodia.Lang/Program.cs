using Antlr4.Runtime;
using Exodia.Lang;
using Exodia.Lang.Ast;
using LLVMSharp.Interop;

try
{
    // LEXING and PARSING
    // Source comes from a file arg (`exodia foo.ex` -- easy to debug in Rider via
    // Program arguments), falling back to stdin (`echo '…' | dotnet run`).
    var charStream = args.Length > 0
        ? CharStreams.fromString(File.ReadAllText(args[0]))
        : CharStreams.fromStream(Console.OpenStandardInput());

    var lexer = new ExodiaLexer(charStream);
    var tokens = new CommonTokenStream(lexer);
    var parser = new ExodiaParser(tokens);

    var tree = parser.program();
    // Console.WriteLine(tree.ToStringTree(parser));
    
    // LLVM: parse tree -> AST (lowering) -> IR (codegen)
    var module = LLVMModuleRef.CreateWithName("exodia");
    var ast = new AstLowering().LowerProgram(tree);
    var codegen = new AstVisitor(module);
    ast.Accept(codegen);

    Console.WriteLine(module.PrintToString());
}
catch (Exception ex)
{
    Console.WriteLine("Errors" + ex);
}