namespace Exodia.Lang.Test;

/// <summary>
/// Drives every <c>.ex</c> fixture through the pipeline, one generated test case per file.
/// The fixture list is discovered from disk (the copied <c>Fixtures/</c> tree), so adding a
/// file adds a test case with no code change.
/// </summary>
public class FixtureTests
{
    /// <summary>Every fixture, both the parse-level ones at the root and the codegen ones.</summary>
    public static IEnumerable<string> AllFixtures() =>
        Fixtures.InDirectory("").Concat(Fixtures.InDirectory("codegen"));

    /// <summary>Fixtures under <c>codegen/</c>, which are expected to lower and emit IR.</summary>
    public static IEnumerable<string> CodegenFixtures() =>
        Fixtures.InDirectory("codegen");

    [Test]
    [MethodDataSource(nameof(AllFixtures))]
    [DisplayName("Parses $fixture without syntax errors")]
    public async Task ParsesWithoutSyntaxErrors(string fixture)
    {
        var parser = Pipeline.Parser(Fixtures.Load(fixture));
        _ = parser.program();
        await Assert.That(parser.NumberOfSyntaxErrors).IsEqualTo(0);
    }

    [Test]
    [MethodDataSource(nameof(CodegenFixtures))]
    [DisplayName("Compiles $fixture to LLVM IR")]
    public async Task CompilesToIr(string fixture)
    {
        var ir = Pipeline.Compile(Fixtures.Load(fixture));
        await Assert.That(ir).Contains("define");
    }
}
