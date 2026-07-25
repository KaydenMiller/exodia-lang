namespace Exodia.Lang.Test;

/// <summary>
/// Opt-in dump of each compiled fixture's LLVM IR to a durable <c>.ll</c> file, so the IR is
/// inspectable from the command line (where the runner only surfaces captured output on
/// failure). Off by default — set <c>EXODIA_DUMP_IR=1</c> to enable. Files land under
/// <c>&lt;test-bin&gt;/TestResults/ir/</c>, mirroring the fixture's relative path
/// (e.g. <c>codegen/return-literal.ll</c>).
/// </summary>
public static class IrArtifacts
{
    /// <summary>True when <c>EXODIA_DUMP_IR</c> is set (any non-empty value).</summary>
    public static bool Enabled =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EXODIA_DUMP_IR"));

    private static readonly string Dir = Path.Combine(
        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!,
        "TestResults", "ir");

    /// <summary>Writes <paramref name="ir"/> for a fixture when enabled; returns the path, or null if disabled.</summary>
    public static string? Write(string fixtureRelPath, string ir)
    {
        if (!Enabled) return null;
        var path = Path.Combine(Dir, Path.ChangeExtension(fixtureRelPath, ".ll"));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, ir);
        return path;
    }
}
