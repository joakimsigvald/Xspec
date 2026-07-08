namespace Xspec.Continuations;

/// <summary>
/// A continuation returned by Using&lt;TTarget&gt;(), allowing the source of the target type's values to be specified with From.
/// If From is not called, the target type is instead registered as a concrete class to instantiate when the subject under test requires an abstraction it implements.
/// </summary>
public interface IUsingContinuation<TSUT, TResult, TTarget> : IUsingTestPipeline<TSUT, TResult>
{
    /// <summary>
    /// Registers a type conversion strategy. Whenever the target type is requested, the generator will first generate the source type and cast it.
    /// </summary>
    /// <typeparam name="TSource">The underlying primitive or source type to generate first.</typeparam>
    /// <returns>A continuation to constrain the generated source values with StartingAt or Spaced, or provide further arrangement.</returns>
    IUsingFromContinuation<TSUT, TResult, TSource> From<TSource>();

    /// <summary>
    /// Registers a type conversion strategy with a specific conversion function. Whenever the target type is requested, the generator will generate the source type and apply the conversion.
    /// </summary>
    /// <typeparam name="TSource">The underlying primitive or source type to generate first.</typeparam>
    /// <param name="convert">The function used to convert the source type into the target type.</param>
    /// <returns>A continuation to constrain the generated source values with StartingAt or Spaced, or provide further arrangement.</returns>
    IUsingFromContinuation<TSUT, TResult, TSource> From<TSource>(Func<TSource, TTarget> convert);
}
