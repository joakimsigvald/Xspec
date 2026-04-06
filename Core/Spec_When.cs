using System.Runtime.CompilerServices;
using Xspec.Continuations;

namespace Xspec;

public abstract partial class Spec<TSUT, TResult> : ITestPipeline<TSUT, TResult>
{
    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="expr"></param>
    /// <returns></returns>
    public ITestPipeline<TSUT, TResult> When(
        Action act,
        [CallerArgumentExpression(nameof(act))] string? expr = null)
        => SetAction(act, expr!);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="expr"></param>
    /// <returns></returns>
    public ITestPipeline<TSUT, TResult> When(
        Action<TSUT> act,
        [CallerArgumentExpression(nameof(act))] string? expr = null)
        => SetAction(act, expr!);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="expr"></param>
    /// <returns></returns>
    public ITestPipeline<TSUT, TResult> When(
    Func<TSUT, TResult> act,
        [CallerArgumentExpression(nameof(act))] string? expr = null)
        => SetAction(act, expr!);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="expr"></param>
    /// <returns></returns>
    public ITestPipeline<TSUT, TResult> When(
    Func<TResult> act, [CallerArgumentExpression(nameof(act))] string? expr = null)
        => SetAction(act, expr!);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="expr"></param>
    /// <returns></returns>
    public ITestPipeline<TSUT, TResult> When(
        Func<TSUT, Task> act, [CallerArgumentExpression(nameof(act))] string? expr = null)
        => SetAction(act, expr!);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="expr"></param>
    /// <returns></returns>
    public ITestPipeline<TSUT, TResult> When(
        Func<Task> act, [CallerArgumentExpression(nameof(act))] string? expr = null) 
        => SetAction(act, expr!);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="expr"></param>
    /// <returns></returns>
    public ITestPipeline<TSUT, TResult> When(
        Func<TSUT, Task<TResult>> act, [CallerArgumentExpression(nameof(act))] string? expr = null) 
        => SetAction(act, expr!);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="expr"></param>
    /// <returns></returns>
    public ITestPipeline<TSUT, TResult> When(
        Func<Task<TResult>> act, [CallerArgumentExpression(nameof(act))] string? expr = null)
        => SetAction(act, expr!);

    /// <summary>
    /// Provide the tearDown to the test-pipeline
    /// </summary>
    /// <param name="tearDown"></param>
    /// <param name="expr"></param>
    /// <returns></returns>
    public ITestPipeline<TSUT, TResult> Before(
        Action<TSUT> tearDown, [CallerArgumentExpression(nameof(tearDown))] string? expr = null)
        => SetTearDown(tearDown, expr!);

    /// <summary>
    /// Provide the tearDown to the test-pipeline
    /// </summary>
    /// <param name="tearDown"></param>
    /// <param name="expr"></param>
    /// <returns></returns>
    public ITestPipeline<TSUT, TResult> Before(
        Func<TSUT, Task> tearDown, [CallerArgumentExpression(nameof(tearDown))] string? expr = null)
        => SetTearDown(tearDown, expr!);

    /// <summary>
    /// Provide the setUp to the test-pipeline
    /// </summary>
    /// <param name="setUp"></param>
    /// <param name="delayMs">Delay between this method invocation and the next in the pipeline</param>
    /// <param name="expr">Provided by the compiler</param>
    /// <param name="delayExpr">Provided by the compiler</param>
    /// <returns></returns>
    public ITestPipeline<TSUT, TResult> After(
        Action<TSUT> setUp, 
        Func<int>? delayMs = null,
        [CallerArgumentExpression(nameof(setUp))] string? expr = null,
        [CallerArgumentExpression(nameof(delayMs))] string? delayExpr = null)
    {
        AddDelay(delayMs, delayExpr!);
        return PrependSetUp(setUp, expr!);
    }

    /// <summary>
    /// Provide the setUp to the test-pipeline
    /// </summary>
    /// <param name="setUp"></param>
    /// <param name="delayMs">Delay between this method invocation and the next in the pipeline</param>
    /// <param name="expr">Provided by the compiler</param>
    /// <param name="delayExpr">Provided by the compiler</param>
    /// <returns></returns>
    public ITestPipeline<TSUT, TResult> After(
        Func<TSUT, Task> setUp, Func<int>? delayMs = null,
        [CallerArgumentExpression(nameof(setUp))] string? expr = null,
        [CallerArgumentExpression(nameof(delayMs))] string? delayExpr = null)
    {
        AddDelay(delayMs, delayExpr!);
        return PrependSetUp(setUp, expr!);
    }

    private void AddDelay(Func<int>? delayMs, string delayExpr)
    {
        if (delayMs is null)
            return;
            PrependSetUp((Func<TSUT, Task>)(_ => Task.Delay(delayMs())), $"wait {delayExpr} ms");
    }

    private Spec<TSUT, TResult> SetAction(Delegate act, string expr)
    {
        _pipeline.SetAction(act, expr);
        return this;
    }

    private Spec<TSUT, TResult> SetTearDown(Delegate act, string expr)
    {
        _pipeline.SetTearDown(act, expr);
        return this;
    }

    private Spec<TSUT, TResult> PrependSetUp(Delegate act, string expr)
    {
        _pipeline.PrependSetUp(act, expr);
        return this;
    }
}