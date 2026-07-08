namespace Xspec.Internal.TestData.Generation;

/// <summary>
/// Exception thrown when a generated sequence would repeat a value.
/// Generated values are guaranteed to be unique, so generation fails rather than yielding a duplicate.
/// </summary>
/// <param name="sourceType">The type whose unique values are exhausted</param>
public class ValuesExhausted(Type sourceType)
    : Exception($"The sequence of unique '{sourceType.Name}' values is exhausted.");
