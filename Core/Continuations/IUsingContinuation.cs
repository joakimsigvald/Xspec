using System.Runtime.CompilerServices;

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

    /// <summary>
    /// Registers a generator function describing the value space of the target type.
    /// Whenever the target type is requested, the generator produces the next source value, which is converted to the target type if necessary.
    /// The values are used exactly as produced: the user defines the value space, so duplicates are allowed.
    /// </summary>
    /// <typeparam name="TSource">The type of the generated values.</typeparam>
    /// <param name="generate">A function producing the next value of the space, invoked once per generated value.</param>
    /// <param name="generateExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>A continuation to provide further infrastructure and test data arrangement.</returns>
    IUsingTestPipeline<TSUT, TResult> From<TSource>(
        Func<TSource> generate,
        [CallerArgumentExpression(nameof(generate))] string? generateExpr = null);
}
