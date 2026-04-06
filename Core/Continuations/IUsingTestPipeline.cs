using System.Runtime.CompilerServices;

namespace Xspec.Continuations;

/// <summary>
/// A continuation to provide further infrastructure and test data arrangement.
/// </summary>
public interface IUsingTestPipeline<TSUT, TResult> : ITestPipeline<TSUT, TResult>
{
    /// <summary>
    /// Registers a type conversion strategy. Whenever the target type is requested, the generator will first generate the source type and cast it.
    /// </summary>
    /// <typeparam name="TTarget">The type being requested by the pipeline.</typeparam>
    /// <typeparam name="TSource">The underlying primitive or source type to generate first.</typeparam>
    /// <returns>A continuation to provide further infrastructure and test data arrangement.</returns>
    IUsingTestPipeline<TSUT, TResult> And<TTarget, TSource>();

    /// <summary>
    /// Registers a type conversion strategy with a specific conversion function. Whenever the target type is requested, the generator will generate the source type and apply the conversion.
    /// </summary>
    /// <typeparam name="TTarget">The type being requested by the pipeline.</typeparam>
    /// <typeparam name="TSource">The underlying primitive or source type to generate first.</typeparam>
    /// <param name="convert">The function used to convert the source type into the target type.</param>
    /// <returns>A continuation to provide further infrastructure and test data arrangement.</returns>
    IUsingTestPipeline<TSUT, TResult> And<TTarget, TSource>(Func<TSource, TTarget> convert);

    /// <summary>
    /// Instructs the test pipeline to use the specified instance when resolving dependencies or generating test data.
    /// </summary>
    /// <typeparam name="TValue">The type of the value being provided.</typeparam>
    /// <param name="value">The specific instance to use.</param>
    /// <param name="scope">Determines whether the value is used for Subject Under Test construction (Subject), ambient test data (Input), or both (All). Defaults to All.</param>
    /// <param name="valueExpr">Automatically populated by the compiler to capture the argument expression.</param>
    /// <returns>A continuation to provide further infrastructure and test data arrangement.</returns>
    IUsingTestPipeline<TSUT, TResult> And<TValue>(
        TValue value,
        Scope scope = Scope.All,
        [CallerArgumentExpression(nameof(value))] string? valueExpr = null);
}