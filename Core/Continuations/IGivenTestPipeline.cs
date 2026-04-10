using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Xspec.Continuations;

/// <summary>
/// A continuation to provide further arrangement
/// </summary>
public interface IGivenTestPipeline<TSUT, TResult> : ITestPipeline<TSUT, TResult>
{
    /// <summary>
    /// Access the mock of the given type to provide mock-setup
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns>A continuation to provide mock-setup for the given type</returns>
    IGivenServiceContinuation<TSUT, TResult, TService> And<TService>() where TService : class;

    /// <summary>
    /// A continuation to provide further arrangement to the test
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Special convension of binding words")]
    IGivenContinuation<TSUT, TResult> and { get; }

    /// <summary>
    /// Transform any value and use the transformed value as default
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <param name="transformExpr"></param>
    /// <returns></returns>
    IGivenTestPipeline<TSUT, TResult> And<TValue>(
        Func<TValue, TValue> transform,
        [CallerArgumentExpression(nameof(transform))] string? transformExpr = null);

    /// <summary>
    /// Provide any arrangement to the test, which will be applied during test execution in reverse order of where in the test-pipeline it was provided
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <param name="setupExpr"></param>
    /// <returns></returns>
    IGivenTestPipeline<TSUT, TResult> And<TValue>(
        Action<TValue> setup,
        [CallerArgumentExpression(nameof(setup))] string? setupExpr = null) where TValue : class;

    /// <summary>
    /// Provide a tag to setup some expectation, such as associating it with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of value the tag is associated with</typeparam>
    /// <param name="tag">The tag</param>
    /// <param name="tagExpr">Leave empty. Provided by the compiler</param>
    /// <returns></returns>
    IGivenTag<TSUT, TResult, TValue> And<TValue>(
        Tag<TValue> tag,
        [CallerArgumentExpression(nameof(tag))] string? tagExpr = null);
}