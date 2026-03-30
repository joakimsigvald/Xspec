using Moq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Xspec.Continuations;

/// <summary>
/// A continuation to apply additional assertions on the test result
/// </summary>
public interface IAndVerify<TResult> : IAndThen<TResult>
{
    /// <summary>
    /// Assert that a mock invocation satisfy the given expression
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <param name="expression"></param>
    /// <param name="expressionExpr"></param>
    /// <returns></returns>
    public IAndVerify<TResult> And<TObject>(
        Expression<Action<TObject>> expression,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) 
        where TObject : class;

    /// <summary>
    /// Assert that a mock invocation satisfying the given expression was made the given number of times
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <param name="expressionExpr"></param>
    /// <returns></returns>
    public IAndVerify<TResult> And<TObject>(
        Expression<Action<TObject>> expression, Times times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) 
        where TObject : class;

    /// <summary>
    /// Assert that a mock invocation satisfying the given expression was made the given number of times
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <param name="expressionExpr"></param>
    /// <returns></returns>
    public IAndVerify<TResult> And<TObject>(
        Expression<Action<TObject>> expression, Func<Times> times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) 
        where TObject : class;

    /// <summary>
    /// Assert that a mock invocation satisfy the given expression
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TReturns"></typeparam>
    /// <param name="expression"></param>
    /// <param name="expressionExpr"></param>
    /// <returns></returns>
    public IAndVerify<TResult> And<TObject, TReturns>(
        Expression<Func<TObject, TReturns>> expression,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) 
        where TObject : class;

    /// <summary>
    /// Assert that a mock invocation satisfying the given expression was made the given number of times
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TReturns"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <param name="expressionExpr"></param>
    /// <returns></returns>
    public IAndVerify<TResult> And<TObject, TReturns>(
        Expression<Func<TObject, TReturns>> expression, Times times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TObject : class;

    /// <summary>
    /// Assert that a mock invocation satisfying the given expression was made the given number of times
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TReturns"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <param name="expressionExpr"></param>
    /// <returns></returns>
    public IAndVerify<TResult> And<TObject, TReturns>(
        Expression<Func<TObject, TReturns>> expression, Func<Times> times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TObject : class;
}