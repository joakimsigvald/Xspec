using System.Runtime.CompilerServices;

namespace Xspec.Continuations;

/// <summary>
/// A continuation to mock the result of a method invocation
/// </summary>
/// <typeparam name="TSUT"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TService"></typeparam>
/// <typeparam name="TReturns"></typeparam>
public interface IGivenThatContinuation<TSUT, TResult, TService, TReturns>
    : IGivenThatCommonContinuation<TSUT, TResult, TService, TReturns>
    where TService : class
{
    /// <summary>
    /// Provide a callback to tap the mocked function call with no input arguments
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="callbackExpr"></param>
    /// <returns></returns>
    IGivenThatCommonContinuation<TSUT, TResult, TService, TReturns> Tap(
        Action callback,
        [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null);

    /// <summary>
    /// Provide a callback to tap the mocked function call with one input argument
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <param name="callback"></param>
    /// <param name="callbackExpr"></param>
    /// <returns></returns>
    IGivenThatCommonContinuation<TSUT, TResult, TService, TReturns> Tap<TArg>(
        Action<TArg> callback,
        [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null);

    /// <summary>
    /// Mock the return-value given one input parameter
    /// </summary>
    /// <param name="returns"></param>
    /// <param name="returnsExpr"></param>
    /// <typeparam name="TArg"></typeparam>
    /// <returns></returns>
    IGivenThatReturnsContinuation<TSUT, TResult, TService, TReturns> Returns<TArg>(
        Func<TArg, TReturns> returns,
        [CallerArgumentExpression(nameof(returns))] string? returnsExpr = null);

    /// <summary>
    /// Provide a callback to tap the mocked function call with two input arguments
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <param name="callback"></param>
    /// <param name="callbackExpr"></param>
    /// <returns></returns>
    IGivenThatCommonContinuation<TSUT, TResult, TService, TReturns> Tap<TArg1, TArg2>(
        Action<TArg1, TArg2> callback,
        [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null);

    /// <summary>
    /// Mock the return-value given two input parameters
    /// </summary>
    /// <param name="returns"></param>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <returns></returns>
    IGivenThatReturnsContinuation<TSUT, TResult, TService, TReturns> Returns<TArg1, TArg2>(
        Func<TArg1, TArg2, TReturns> returns);

    /// <summary>
    /// Provide a callback to tap the mocked function call with three input arguments
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TArg3"></typeparam>
    /// <param name="callback"></param>
    /// <param name="callbackExpr"></param>
    /// <returns></returns>
    IGivenThatCommonContinuation<TSUT, TResult, TService, TReturns> Tap<TArg1, TArg2, TArg3>(
        Action<TArg1, TArg2, TArg3> callback,
        [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null);

    /// <summary>
    /// Mock the return-value given three input parameters
    /// </summary>
    /// <param name="returns"></param>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TArg3"></typeparam>
    /// <returns></returns>
    IGivenThatReturnsContinuation<TSUT, TResult, TService, TReturns> Returns<TArg1, TArg2, TArg3>(
        Func<TArg1, TArg2, TArg3, TReturns> returns);

    /// <summary>
    /// Provide a callback to tap the mocked function call with four input arguments
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TArg3"></typeparam>
    /// <typeparam name="TArg4"></typeparam>
    /// <param name="callback"></param>
    /// <param name="callbackExpr"></param>
    /// <returns></returns>
    IGivenThatCommonContinuation<TSUT, TResult, TService, TReturns> Tap<TArg1, TArg2, TArg3, TArg4>(
        Action<TArg1, TArg2, TArg3, TArg4> callback,
        [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null);

    /// <summary>
    /// Mock the return-value given four input parameters
    /// </summary>
    /// <param name="returns"></param>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TArg3"></typeparam>
    /// <typeparam name="TArg4"></typeparam>
    /// <returns></returns>
    IGivenThatReturnsContinuation<TSUT, TResult, TService, TReturns> Returns<TArg1, TArg2, TArg3, TArg4>(
        Func<TArg1, TArg2, TArg3, TArg4, TReturns> returns);

    /// <summary>
    /// Provide a callback to tap the mocked function call with five input arguments
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TArg3"></typeparam>
    /// <typeparam name="TArg4"></typeparam>
    /// <typeparam name="TArg5"></typeparam>
    /// <param name="callback"></param>
    /// <param name="callbackExpr"></param>
    /// <returns></returns>
    IGivenThatCommonContinuation<TSUT, TResult, TService, TReturns> Tap<TArg1, TArg2, TArg3, TArg4, TArg5>(
        Action<TArg1, TArg2, TArg3, TArg4, TArg5> callback,
        [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null);

    /// <summary>
    /// Mock the return-value given five input parameters
    /// </summary>
    /// <param name="returns"></param>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TArg3"></typeparam>
    /// <typeparam name="TArg4"></typeparam>
    /// <typeparam name="TArg5"></typeparam>
    /// <returns></returns>
    IGivenThatReturnsContinuation<TSUT, TResult, TService, TReturns> Returns<TArg1, TArg2, TArg3, TArg4, TArg5>(
        Func<TArg1, TArg2, TArg3, TArg4, TArg5, TReturns> returns);

    /// <summary>
    /// Returns a continuation for providing the first mocked result of a sequence of method invocations.
    /// </summary>
    /// <returns></returns>
    IGivenThatCommonContinuation<TSUT, TResult, TService, TReturns> First();
}