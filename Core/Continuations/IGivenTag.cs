using System.Runtime.CompilerServices;

namespace Xspec.Continuations;

/// <summary>
/// Continuation for setting up an expectation for a tag, such as associating it with a value
/// </summary>
/// <typeparam name="TSUT"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface IGivenTag<TSUT, TResult, TValue>
{
    /// <summary>
    /// Associate a tag with a value, which can be referenced in the pipeline using 'The([tag])'
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueExpr"></param>
    /// <returns>The pipeline, so that further setup can be provided</returns>
    IGivenTestPipeline<TSUT, TResult> Is(
        TValue value,
        [CallerArgumentExpression(nameof(value))] string? valueExpr = null);

    /// <summary>
    /// Apply setup to the value associated with the tag
    /// </summary>
    /// <param name="setup"></param>
    /// <param name="setupExpr"></param>
    /// <returns>The pipeline, so that further setup can be provided</returns>
    IGivenTestPipeline<TSUT, TResult> Has(
        Action<TValue> setup,
        [CallerArgumentExpression(nameof(setup))] string? setupExpr = null);

    /// <summary>
    /// Apply transform to the value associated with the tag
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="transformExpr"></param>
    /// <returns>The pipeline, so that further setup can be provided</returns>
    IGivenTestPipeline<TSUT, TResult> Has(
        Func<TValue, TValue> transform,
        [CallerArgumentExpression(nameof(transform))] string? transformExpr = null);
}