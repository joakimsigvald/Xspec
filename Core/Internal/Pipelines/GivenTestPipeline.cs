using System.Runtime.CompilerServices;
using Xspec.Continuations;

namespace Xspec.Internal.Pipelines;

internal class GivenTestPipeline<TSUT, TResult>
    : TestPipeline<TSUT, TResult, Spec<TSUT, TResult>>, IGivenTestPipeline<TSUT, TResult>
{
    internal GivenTestPipeline(Spec<TSUT, TResult> parent) : base(parent) { }

    public ITestPipeline<TSUT, TResult> When(
        Action<TSUT> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null)
        => Parent.When(act, actExpr!);

    public ITestPipeline<TSUT, TResult> When(
        Action act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null)
        => Parent.When(act, actExpr!);

    public ITestPipeline<TSUT, TResult> When(
        Func<TSUT, TResult> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null)
        => Parent.When(act, actExpr!);

    public ITestPipeline<TSUT, TResult> When(
        Func<TResult> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null)
        => Parent.When(act, actExpr!);

    public ITestPipeline<TSUT, TResult> When(
        Func<TSUT, Task> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null)
        => Parent.When(act, actExpr!);

    public ITestPipeline<TSUT, TResult> When(
        Func<Task> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null)
        => Parent.When(act, actExpr!);

    public ITestPipeline<TSUT, TResult> When(
        Func<TSUT, Task<TResult>> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null)
        => Parent.When(act, actExpr!);

    public ITestPipeline<TSUT, TResult> When(
        Func<Task<TResult>> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null)
        => Parent.When(act, actExpr!);

    public ITestPipeline<TSUT, TResult> After(
        Action<TSUT> setUp, 
        Func<int>? delayBeforeNextMs = null,
        [CallerArgumentExpression(nameof(setUp))] string? setUpExpr = null,
        [CallerArgumentExpression(nameof(delayBeforeNextMs))] string? delayExpr = null) 
        => Parent.After(setUp, delayBeforeNextMs, setUpExpr!, delayExpr!);

    public ITestPipeline<TSUT, TResult> After(
        Func<TSUT, Task> setUp, 
        Func<int>? delayBeforeNextMs = null,
        [CallerArgumentExpression(nameof(setUp))] string? setUpExpr = null,
        [CallerArgumentExpression(nameof(delayBeforeNextMs))] string? delayExpr = null) 
        => Parent.After(setUp, delayBeforeNextMs, setUpExpr!, delayExpr!);

    public ITestPipeline<TSUT, TResult> Before(
        Action<TSUT> tearDown, [CallerArgumentExpression(nameof(tearDown))] string? tearDownExpr = null)
        => Parent.Before(tearDown, tearDownExpr!);

    public ITestPipeline<TSUT, TResult> Before(
        Func<TSUT, Task> tearDown, [CallerArgumentExpression(nameof(tearDown))] string? tearDownExpr = null)
        => Parent.Before(tearDown, tearDownExpr!);

    public IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        Action<TValue> setup,
        [CallerArgumentExpression(nameof(setup))] string? setupExpr = null) where TValue : class
        => Parent.Given(setup, setupExpr!);

    public IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        Func<TValue, TValue> transform,
        [CallerArgumentExpression(nameof(transform))] string? transformExpr = null)
        => Parent.Given(transform, transformExpr!);

    public IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        TValue defaultValue,
        [CallerArgumentExpression(nameof(defaultValue))] string? defaultValueExpr = null)
        => Parent.Given(defaultValue, defaultValueExpr!);

    public IGivenServiceContinuation<TSUT, TResult, TService> Given<TService>() where TService : class
        => Parent.Given<TService>();

    public IGivenContinuation<TSUT, TResult> Given() => Parent.Given();

    public IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        Func<TValue> defaultValue,
        [CallerArgumentExpression(nameof(defaultValue))] string? defaultValueExpr = null) 
        => Parent.Given(defaultValue, defaultValueExpr!);

    public IGivenTag<TSUT, TResult, TValue> Given<TValue>(
        Tag<TValue> tag,
        [CallerArgumentExpression(nameof(tag))] string? tagExpr = null)
        => Parent.Given(tag, tagExpr!);

    public IGivenTestPipeline<TSUT, TResult> And<TValue>(
        Action<TValue> setup,
        [CallerArgumentExpression(nameof(setup))] string? setupExpr = null) where TValue : class
        => Given(setup, setupExpr);

    public IGivenTag<TSUT, TResult, TValue> And<TValue>(
        Tag<TValue> tag,
        [CallerArgumentExpression(nameof(tag))] string? tagExpr = null)
        => new GivenTag<TSUT, TResult, TValue>(Parent, tag, tagExpr!);

    public IGivenTestPipeline<TSUT, TResult> And<TValue>(
        Func<TValue, TValue> setup,
        [CallerArgumentExpression(nameof(setup))] string? setupExpr = null)
        => Given(setup, setupExpr);

    public IGivenTestPipeline<TSUT, TResult> And<TValue>(
        Func<TValue> defaultValue,
        [CallerArgumentExpression(nameof(defaultValue))] string? defaultValueExpr = null)
        => Given(defaultValue, defaultValueExpr);

    public IGivenTestPipeline<TSUT, TResult> And<TValue>(
        TValue defaultValue, 
        [CallerArgumentExpression(nameof(defaultValue))] string? defaultValueExpr = null) 
        => Given(defaultValue, defaultValueExpr);

    public IGivenServiceContinuation<TSUT, TResult, TService> And<TService>() where TService : class => Given<TService>();
    public IGivenContinuation<TSUT, TResult> And() => Given();
    public IGivenContinuation<TSUT, TResult> and => Given();
}