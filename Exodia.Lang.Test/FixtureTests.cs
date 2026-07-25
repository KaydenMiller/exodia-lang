using LLVMSharp.Interop;

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
    [DisplayName("Emits well-formed IR for $fixture")]
    public async Task EmitsWellFormedIr(string fixture)
    {
        var module = Pipeline.CompileToModule(Fixtures.Load(fixture));

        // Surface the emitted IR in the test's captured output so it's viewable per-test
        // (Rider shows it for passing tests; the CLI runner shows it on failure). Set
        // EXODIA_DUMP_IR=1 to also dump .ll files for CLI-side inspection of passing runs.
        var ir = module.PrintToString();
        TestContext.Current!.Output.WriteLine(ir);
        IrArtifacts.Write(fixture, ir);

        var ok = module.TryVerify(LLVMVerifierFailureAction.LLVMReturnStatusAction, out var message);
        await Assert.That(ok).IsTrue().Because($"LLVM verifier rejected the IR: {message}");
    }
}
