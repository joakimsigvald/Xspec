using Moq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xspec.Continuations;

namespace Xspec;

public abstract partial class Spec<TSUT, TResult> : ITestPipeline<TSUT, TResult>
{
    /// <summary>
    /// Run the test-pipeline and return the result
    /// </summary>
    /// <returns>The test result</returns>
    public ITestResultWithSUT<TSUT, TResult> Then() => Pipeline.Then();

    /// <summary>
    /// Run the test-pipeline and return a given subject to be used in chained assertions.
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject to return</typeparam>
    /// <param name="subject">The subject to return for chained assertions</param>
    /// <returns>the given subject</returns>
    public TSubject Then<TSubject>(TSubject subject)
    {
        Pipeline.Then();
        return subject;
    }

    /// <summary>
    /// Run the test-pipeline and verify that the given mock invocation was made.
    /// </summary>
    /// <typeparam name="TService">The mocked type to verify an invocation on</typeparam>
    /// <param name="expression">An expression specifying the method invocation to verify</param>
    /// <param name="expressionExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>A continuation for further verification or assertions of the test result</returns>
    public IAndVerify<TResult> Then<TService>(
        Expression<Action<TService>> expression,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class
        => Pipeline.Then(expression, expressionExpr!);

    /// <summary>
    /// Run the test-pipeline and verify that the given mock invocation was made the given number of times.
    /// </summary>
    /// <typeparam name="TService">The mocked type to verify an invocation on</typeparam>
    /// <param name="expression">An expression specifying the method invocation to verify</param>
    /// <param name="times">The number of times the invocation is expected to have been made</param>
    /// <param name="expressionExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>A continuation for further verification or assertions of the test result</returns>
    public IAndVerify<TResult> Then<TService>(
        Expression<Action<TService>> expression, Times times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) where TService : class
        => Pipeline.Then(expression, times, expressionExpr!);

    /// <summary>
    /// Run the test-pipeline and verify that the given mock invocation was made the number of times given by a function.
    /// </summary>
    /// <typeparam name="TService">The mocked type to verify an invocation on</typeparam>
    /// <param name="expression">An expression specifying the method invocation to verify</param>
    /// <param name="times">A function providing the number of times the invocation is expected to have been made</param>
    /// <param name="expressionExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>A continuation for further verification or assertions of the test result</returns>
    public IAndVerify<TResult> Then<TService>(
        Expression<Action<TService>> expression, Func<Times> times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) where TService : class
        => Pipeline.Then(expression, times, expressionExpr!);

    /// <summary>
    /// Run the test-pipeline and verify that the given value-returning mock invocation was made.
    /// </summary>
    /// <typeparam name="TService">The mocked type to verify an invocation on</typeparam>
    /// <typeparam name="TReturns">The return type of the mocked invocation</typeparam>
    /// <param name="expression">An expression specifying the method invocation to verify</param>
    /// <param name="expressionExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>A continuation for further verification or assertions of the test result</returns>
    public IAndVerify<TResult> Then<TService, TReturns>(
        Expression<Func<TService, TReturns>> expression,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) where TService : class
        => Pipeline.Then(expression, expressionExpr!);

    /// <summary>
    /// Run the test-pipeline and verify that the given value-returning mock invocation was made the given number of times.
    /// </summary>
    /// <typeparam name="TService">The mocked type to verify an invocation on</typeparam>
    /// <typeparam name="TReturns">The return type of the mocked invocation</typeparam>
    /// <param name="expression">An expression specifying the method invocation to verify</param>
    /// <param name="times">The number of times the invocation is expected to have been made</param>
    /// <param name="expressionExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>A continuation for further verification or assertions of the test result</returns>
    public IAndVerify<TResult> Then<TService, TReturns>(
        Expression<Func<TService, TReturns>> expression, Times times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class
        => Pipeline.Then(expression, times, expressionExpr!);

    /// <summary>
    /// Run the test-pipeline and verify that the given value-returning mock invocation was made the number of times given by a function.
    /// </summary>
    /// <typeparam name="TService">The mocked type to verify an invocation on</typeparam>
    /// <typeparam name="TReturns">The return type of the mocked invocation</typeparam>
    /// <param name="expression">An expression specifying the method invocation to verify</param>
    /// <param name="times">A function providing the number of times the invocation is expected to have been made</param>
    /// <param name="expressionExpr">Captured automatically by the compiler — do not provide</param>
    /// <returns>A continuation for further verification or assertions of the test result</returns>
    public IAndVerify<TResult> Then<TService, TReturns>(
        Expression<Func<TService, TReturns>> expression, Func<Times> times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class
        => Pipeline.Then(expression, times, expressionExpr!);

    /// <summary>
    /// Contains the returned value after calling method-under-test
    /// </summary>
    protected TResult Result => Pipeline.Then().Result;
}