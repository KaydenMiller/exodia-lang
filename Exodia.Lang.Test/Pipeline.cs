using Antlr4.Runtime;
using LLVMSharp.Interop;

namespace Exodia.Lang.Test;

/// <summary>
/// Test-facing view of the Exodia compilation pipeline, mirroring <c>Program.cs</c>.
/// Each stage builds on the previous one so a test can stop at whichever stage it
/// wants to assert against: parse tree, lowered AST, or emitted LLVM IR text.
/// </summary>
public static class Pipeline
{
    /// <summary>Stage 0: source text -> a configured parser (before <c>program()</c> is called).</summary>
    public static ExodiaParser Parser(string source)
    {
        var input = new AntlrInputStream(source);
        var lexer = new ExodiaLexer(input);
        var tokens = new CommonTokenStream(lexer);
        return new ExodiaParser(tokens);
    }

    /// <summary>Stage 1: source text -> parse tree (the ANTLR <c>program</c> rule).</summary>
    public static ExodiaParser.ProgramContext ParseTree(string source) =>
        Parser(source).program();

    /// <summary>Stage 2: source text -> lowered AST.</summary>
    public static ProgramNode LowerAst(string source) =>
        new AstLowering().LowerProgram(ParseTree(source));

    /// <summary>
    /// Stage 3: source text -> a codegen'd LLVM module.
    /// Each compile gets its own LLVM context so struct type names don't accumulate across
    /// compilations in a shared (global) context, which would rename the Nth "Circle" to
    /// "Circle.N" and break codegen's name-keyed lookups. Safe now that the codegen routes
    /// every primitive through the module's context (no global-context leaks).
    /// </summary>
    public static LLVMModuleRef CompileToModule(string source, string moduleName = "test")
    {
        var context = LLVMContextRef.Create();
        var module = context.CreateModuleWithName(moduleName);
        LowerAst(source).Accept(new AstVisitor(module));
        return module;
    }

    /// <summary>Stage 3, as text: the emitted LLVM IR (what <c>Program.cs</c> prints).</summary>
    public static string Compile(string source, string moduleName = "test") =>
        CompileToModule(source, moduleName).PrintToString();
}
