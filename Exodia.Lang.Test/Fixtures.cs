namespace Exodia.Lang.Test;

/// <summary>
/// Loads Exodia source fixtures (<c>.ex</c> files) that are copied next to the test
/// binary via the <c>Fixtures\**\*.ex</c> item in the csproj. Paths are given relative
/// to the <c>Fixtures/</c> root, e.g. <c>"codegen/return-literal.ex"</c>.
/// <para>
/// <see cref="InDirectory"/> is the seam for upcoming multi-file / library tests: a
/// "library" is just a directory of <c>.ex</c> files under <c>Fixtures/</c>.
/// </para>
/// </summary>
public static class Fixtures
{
    private static readonly string Root = System.IO.Path.Combine(
        System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!,
        "Fixtures");

    /// <summary>Absolute path to a fixture, given its path relative to <c>Fixtures/</c>.</summary>
    public static string FullPath(string relativePath) =>
        System.IO.Path.Combine(Root, relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));

    /// <summary>Reads a fixture's source text, given its path relative to <c>Fixtures/</c>.</summary>
    public static string Load(string relativePath) =>
        File.ReadAllText(FullPath(relativePath));

    /// <summary>
    /// Enumerates the <c>.ex</c> fixtures under a directory (relative to <c>Fixtures/</c>),
    /// returning each one's path relative to <c>Fixtures/</c> (forward-slashed) for use with
    /// <see cref="Load"/> or as test <c>[Arguments]</c>. Non-recursive.
    /// </summary>
    public static IEnumerable<string> InDirectory(string relativeDir)
    {
        var dir = FullPath(relativeDir);
        if (!Directory.Exists(dir))
            return Enumerable.Empty<string>();

        return Directory.EnumerateFiles(dir, "*.ex")
            .Select(full => System.IO.Path.GetRelativePath(Root, full).Replace(System.IO.Path.DirectorySeparatorChar, '/'))
            .OrderBy(p => p, StringComparer.Ordinal);
    }
}
