namespace Xspec.Continuations;

/// <summary>
/// A continuation returned by From&lt;TSource&gt;(), allowing the generated source values to be constrained
/// with the extension methods StartingAt and Spaced (for numeric source types), or provide further arrangement.
/// </summary>
public interface IUsingFromContinuation<TSUT, TResult, TSource> : IUsingTestPipeline<TSUT, TResult>
{
}
