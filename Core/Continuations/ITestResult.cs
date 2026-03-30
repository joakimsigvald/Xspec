using System.Runtime.CompilerServices;

namespace Xspec.Continuations;

/// <summary>
/// An object containing the result of a test-run
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface ITestResult<TResult>
{
    /// <summary>
    /// The return value of a non-throwing test-run
    /// </summary>
    TResult Result { get; }

    /// <summary>
    /// Asserts that the test-run threw an error of the given type
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    IAndThen<TResult> Throws<TError>();

    /// <summary>
    /// Asserts that the test-run threw an error that is equal to the return value of the given function
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    IAndThen<TResult> Throws<TError>(
        Func<TError> expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null) 
        where TError : Exception;

    /// <summary>
    /// Asserts that the test-run threw an error of the given type, and satisfy the given assertions
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <param name="assert"></param>
    /// <returns></returns>
    IAndThen<TResult> Throws<TError>(Action<TError> assert);

    /// <summary>
    /// Asserts that the test-run threw an error that satisfies the given predicate
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <param name="condition"></param>
    /// <param name="conditionExpr"></param>
    /// <returns></returns>
    IAndThen<TResult> Throws<TError>(
        Func<TError, bool> condition, 
        [CallerArgumentExpression(nameof(condition))] string? conditionExpr = null);

    /// <summary>
    /// Asserts that the test-run threw an error
    /// </summary>
    /// <returns></returns>
    IAndThen<TResult> Throws();

    /// <summary>
    /// Asserts that the test-run did not throw an error of the given type
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    IAndThen<TResult> DoesNotThrow<TError>();

    /// <summary>
    /// Asserts that the test-run did not throw an error
    /// </summary>
    /// <returns></returns>
    IAndThen<TResult> DoesNotThrow();
}