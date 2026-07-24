namespace Exodia.Lang;

/// <summary>
/// Identifies on overloaded callable: who owns it, its name, and how many parameters
/// </summary>
/// <param name="Owner">"" for free functions; struct name for methods/constructors</param>
/// <param name="Name"></param>
/// <param name="Arity"></param>
public sealed record CallableKey(
    string Owner,
    string Name,
    int Arity
);