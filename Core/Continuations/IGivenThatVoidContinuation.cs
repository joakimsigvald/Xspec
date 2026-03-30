using System.Runtime.CompilerServices;

namespace Xspec.Continuations;

/// <summary>
/// A continuation to mock the result of a method invocation
/// </summary>
/// <typeparam name="TSUT"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TService"></typeparam>
public interface IGivenThatVoidContinuation<TSUT, TResult, TService>
    : IGivenThatCommonContinuation<TSUT, TResult, TService, Void>
    where TService : class
{
    /// <summary>
    /// Provide a callback to tap the mocked function call with no input arguments
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="callbackExpr"></param>
    /// <returns></returns>
    IGivenThatCommonContinuation<TSUT, TResult, TService, Void> Tap(
        Action callback,
        [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null);

    /// <summary>
    /// Provide a callback to tap the mocked function call with one input argument
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <param name="callback"></param>
    /// <param name="callbackExpr"></param>
    /// <returns></returns>
    IGivenThatCommonContinuation<TSUT, TResult, TService, Void> Tap<TArg>(
        Action<TArg> callback,
        [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null);

    /// <summary>
    /// Provide a callback to tap the mocked function call with two input arguments
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <param name="callback"></param>
    /// <param name="callbackExpr"></param>
    /// <returns></returns>
    IGivenThatCommonContinuation<TSUT, TResult, TService, Void> Tap<TArg1, TArg2>(
        Action<TArg1, TArg2> callback,
        [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null);

    /// <summary>
    /// Provide a callback to tap the mocked function call with three input arguments
    /// </summary>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TArg3"></typeparam>
    /// <param name="callback"></param>
    /// <param name="callbackExpr"></param>
    /// <returns></returns>
    IGivenThatCommonContinuation<TSUT, TResult, TService, Void> Tap<TArg1, TArg2, TArg3>(
        Action<TArg1, TArg2, TArg3> callback,
        [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null);

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
    IGivenThatCommonContinuation<TSUT, TResult, TService, Void> Tap<TArg1, TArg2, TArg3, TArg4>(
        Action<TArg1, TArg2, TArg3, TArg4> callback,
        [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null);

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
    IGivenThatCommonContinuation<TSUT, TResult, TService, Void> Tap<TArg1, TArg2, TArg3, TArg4, TArg5>(
        Action<TArg1, TArg2, TArg3, TArg4, TArg5> callback,
        [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null);
}