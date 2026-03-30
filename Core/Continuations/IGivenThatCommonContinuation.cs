using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Xspec.Continuations;

/// <summary>
/// Provide a callback to tap the mocked function call with no input arguments
/// </summary>
/// <typeparam name="TSUT"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TService"></typeparam>
/// <typeparam name="TReturns"></typeparam>
public interface IGivenThatCommonContinuation<TSUT, TResult, TService, TReturns>
    where TService : class
{
    /// <summary>
    /// After using Tap to inspect or use incoming parameters of a mocked method invocation, 
    /// call Returns (with or without return value) to complete the setup of the mock.
    /// Otherwise the mocked behavior of this method will not be applied when running the test pipeline.
    /// </summary>
    /// <returns></returns>
    IGivenThatReturnsContinuation<TSUT, TResult, TService, TReturns> Returns();

    /// <summary>
    /// Mock the return-value of a method invocation
    /// </summary>
    /// <param name="returns">the return value</param>
    /// <param name="returnsExpr">Provided by the compiler to generate specification output</param>
    /// <returns></returns>
    IGivenThatReturnsContinuation<TSUT, TResult, TService, TReturns> Returns(
        [NotNull] Func<TReturns?> returns, [CallerArgumentExpression(nameof(returns))] string? returnsExpr = null);

    /// <summary>
    /// Mock the return-value of a method invocation
    /// </summary>
    /// <param name="tag">the return value</param>
    /// <param name="tagExpr">Provided by the compiler to generate specification output</param>
    /// <returns></returns>
    IGivenThatReturnsContinuation<TSUT, TResult, TService, TReturns> Returns(
        [NotNull] Tag<TReturns?> tag, [CallerArgumentExpression(nameof(tag))] string? tagExpr = null);

    /// <summary>
    /// Mock the return-value as default
    /// </summary>
    /// <returns></returns>
    IGivenThatReturnsContinuation<TSUT, TResult, TService, TReturns> ReturnsDefault();

    /// <summary>
    /// Provide an exception-type to mock
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    /// <returns></returns>
    IGivenThatReturnsContinuation<TSUT, TResult, TService, TReturns> Throws<TException>() where TException : Exception, new();

    /// <summary>
    /// Provide an exception to mock
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    IGivenThatReturnsContinuation<TSUT, TResult, TService, TReturns> Throws(
        Func<Exception> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null);
}