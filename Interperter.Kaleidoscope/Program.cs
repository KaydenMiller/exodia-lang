using System.Text;
using Antlr4.Runtime;
using Interperter.Kaleidoscope;
using LLVMSharp.Interop;

Console.WriteLine("Building Kaleidoscope Lang");

try
{
    var text = File.ReadAllText("./tests/test_1.kl", Encoding.UTF8);

    var inputStream = new AntlrInputStream(text);
    var parserLexer = new KaleidoscopeLexer(inputStream);
    var commonTokenStream = new CommonTokenStream(parserLexer);
    var parser = new KaleidoscopeParser(commonTokenStream);
    var tree = parser.program();
    
    unsafe
    {
        var module = LLVM.ModuleCreateWithName("KaleidoscopeJIT".ToSByte());
        var builder = LLVM.CreateBuilder();

        var eval = new KaleidoscopeVisitor(module, builder);
        eval.Visit(tree);
        
        LLVM.DumpModule(module);
        var filePath = Directory.GetCurrentDirectory() + "/output.ll";
        LLVM.WriteBitcodeToFile(module, filePath.ToSByte());
    }
}
catch (Exception ex)
{
    Console.WriteLine("Errors" + ex);
}