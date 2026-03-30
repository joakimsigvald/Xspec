namespace Xspec;

/// <summary>
/// This type's sole purpose is to help the compiler distinguish between overloaded extension methods, 
/// by adding it as nullable to the argument list. As the name implies, this class should be ignored.
/// </summary>
public readonly struct Ignore
{
    /// <summary>
    /// The Singleton instance of Ignore
    /// </summary>
    public readonly static Ignore Me = default;

    /// <summary>
    /// Empty string
    /// </summary>
    /// <returns></returns>
    public override string ToString() => string.Empty;
}