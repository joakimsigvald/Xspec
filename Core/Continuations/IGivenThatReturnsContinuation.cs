using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Xspec.Continuations;

/// <summary>
/// Provide a callback to tap the mocked function call with no input arguments
/// </summary>
/// <typeparam name="TSUT"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TService"></typeparam>
/// <typeparam name="TReturns"></typeparam>
public interface IGivenThatReturnsContinuation<TSUT, TResult, TService, TReturns> : IGivenTestPipeline<TSUT, TResult>
    where TService : class
{
    /// <summary>
    /// Setup mock to return a value as default for any invocation where no specific mock-setup has been provided
    /// </summary>
    /// <typeparam name="TReturns2"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    IGivenTestPipeline<TSUT, TResult> AndReturnsDefault<TReturns2>(Func<TReturns2> value);

    /// <summary>
    /// Mock the method invocation
    /// </summary>
    /// <typeparam name="TReturns2"></typeparam>
    /// <param name="call"></param>
    /// <param name="callExpr"></param>
    /// <returns>Continuation for providing method invocation result to mock</returns>
    IGivenThatContinuation<TSUT, TResult, TService, TReturns2> AndThat<TReturns2>(
        Expression<Func<TService, TReturns2>> call,
        [CallerArgumentExpression(nameof(call))] string? callExpr = null);

    /// <summary>
    /// Provide async method invocation to mock
    /// </summary>
    /// <typeparam name="TReturns2"></typeparam>
    /// <param name="call"></param>
    /// <param name="callExpr"></param>
    /// <returns>Continuation for providing method invocation result to mock</returns>
    IGivenThatContinuation<TSUT, TResult, TService, TReturns2> AndThat<TReturns2>(
        Expression<Func<TService, Task<TReturns2>>> call,
        [CallerArgumentExpression(nameof(call))] string? callExpr = null);

    /// <summary>
    /// Returns a continuation for providing the next mocked result of a sequence of method invocations.
    /// </summary>
    /// <returns></returns>
    IGivenThatCommonContinuation<TSUT, TResult, TService, TReturns> AndNext();
}