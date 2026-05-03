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
    /// <typeparam name="TSubject"></typeparam>
    /// <param name="subject"></param>
    /// <returns>the given subject</returns>
    public TSubject Then<TSubject>(TSubject subject)
    {
        Pipeline.Then();
        return subject;
    }

    /// <summary>
    /// Run the test-pipeline and verify mock invocation.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="expression"></param>
    /// <param name="expressionExpr"></param>
    /// <returns></returns>
    public IAndVerify<TResult> Then<TService>(
        Expression<Action<TService>> expression,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) 
        where TService : class
        => Pipeline.Then(expression, expressionExpr!);

    /// <summary>
    /// Run the test-pipeline and verify mock invocation.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <param name="expressionExpr"></param>
    /// <returns></returns>
    public IAndVerify<TResult> Then<TService>(
        Expression<Action<TService>> expression, Times times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) where TService : class
        => Pipeline.Then(expression, times, expressionExpr!);

    /// <summary>
    /// Run the test-pipeline and verify mock invocation.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <param name="expressionExpr"></param>
    /// <returns></returns>
    public IAndVerify<TResult> Then<TService>(
        Expression<Action<TService>> expression, Func<Times> times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) where TService : class
        => Pipeline.Then(expression, times, expressionExpr!);

    /// <summary>
    /// Run the test-pipeline and verify mock invocation.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TReturns"></typeparam>
    /// <param name="expression"></param>
    /// <param name="expressionExpr"></param>
    /// <returns></returns>
    public IAndVerify<TResult> Then<TService, TReturns>(
        Expression<Func<TService, TReturns>> expression,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) where TService : class
        => Pipeline.Then(expression, expressionExpr!);

    /// <summary>
    /// Run the test-pipeline and verify mock invocation.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TReturns"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <param name="expressionExpr"></param>
    /// <returns></returns>
    public IAndVerify<TResult> Then<TService, TReturns>(
        Expression<Func<TService, TReturns>> expression, Times times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class
        => Pipeline.Then(expression, times, expressionExpr!);

    /// <summary>
    /// Run the test-pipeline and verify mock invocation.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TReturns"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <param name="expressionExpr"></param>
    /// <returns></returns>
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