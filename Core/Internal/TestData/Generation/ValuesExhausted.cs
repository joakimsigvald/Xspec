namespace Xspec.Internal.TestData.Generation;

/// <summary>
/// Exception thrown when a value space has no more values to yield:
/// a numeric sequence would repeat a value (generated sequence values are guaranteed to be unique,
/// so generation fails rather than yielding a duplicate), or an explicit value list is depleted.
/// </summary>
/// <param name="sourceType">The type whose values are exhausted</param>
public class ValuesExhausted(Type sourceType)
    : Exception($"The sequence of '{sourceType.Name}' values is exhausted.");
