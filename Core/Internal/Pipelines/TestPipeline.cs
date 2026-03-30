using Moq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xspec.Continuations;

namespace Xspec.Internal.Pipelines;

internal abstract class TestPipeline<TSUT, TResult, TParent>(TParent parent) where TParent : Spec<TSUT, TResult>
{
    protected readonly TParent Parent = parent;

    public ITestResultWithSUT<TSUT, TResult> Then() => Parent.Then();
    public TSubject Then<TSubject>(TSubject subject) => Parent.Then(subject);

    public IAndVerify<TResult> Then<TService>(
        Expression<Action<TService>> expression,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) 
        where TService : class
        => Parent.Then(expression, expressionExpr!);

    public IAndVerify<TResult> Then<TService>(
        Expression<Action<TService>> expression, Times times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class
        => Parent.Then(expression, times, expressionExpr!);

    public IAndVerify<TResult> Then<TService>(
        Expression<Action<TService>> expression, Func<Times> times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null) 
        where TService : class
        => Parent.Then(expression, times, expressionExpr!);

    public IAndVerify<TResult> Then<TService, TReturns>(
        Expression<Func<TService, TReturns>> expression,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class
        => Parent.Then(expression, expressionExpr!);

    public IAndVerify<TResult> Then<TService, TReturns>(
        Expression<Func<TService, TReturns>> expression, Times times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class
        => Parent.Then(expression, times, expressionExpr!);

    public IAndVerify<TResult> Then<TService, TReturns>(
        Expression<Func<TService, TReturns>> expression, Func<Times> times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class
        => Parent.Then(expression, times, expressionExpr!);
}