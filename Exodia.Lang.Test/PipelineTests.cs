namespace Exodia.Lang.Test;

/// <summary>
/// Smoke tests that exercise every pipeline stage and the fixture loader. These double
/// as proof that the whole test harness builds and runs against the real compiler.
/// </summary>
public class PipelineTests
{
    private const string MinimalProgram = "fn main(): int32 { return 0; }";

    [Test]
    [DisplayName("Parses a minimal function with no syntax errors")]
    public async Task ParsesMinimalProgram()
    {
        var parser = Pipeline.Parser(MinimalProgram);
        _ = parser.program();
        await Assert.That(parser.NumberOfSyntaxErrors).IsEqualTo(0);
    }

    [Test]
    [DisplayName("Lowers a minimal program to a ProgramNode")]
    public async Task LowersMinimalProgram()
    {
        var ast = Pipeline.LowerAst(MinimalProgram);
        await Assert.That(ast).IsNotNull();
    }

    [Test]
    [DisplayName("Compiles a minimal program to LLVM IR")]
    public async Task CompilesMinimalProgram()
    {
        var ir = Pipeline.Compile(MinimalProgram);
        await Assert.That(ir).Contains("define");
    }
}
