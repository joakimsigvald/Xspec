using System.Runtime.CompilerServices;
using Xspec.Continuations;

namespace Xspec.Internal.Pipelines;

/// <summary>
/// The internal implementation of the test pipeline continuation for infrastructure and data generation arrangement.
/// Delegates subsequent setup calls back to the parent specification.
/// </summary>
internal class UsingTestPipeline<TSUT, TResult> :
    TestPipeline<TSUT, TResult, Spec<TSUT, TResult>>,
    IUsingTestPipeline<TSUT, TResult>
{
    internal UsingTestPipeline(Spec<TSUT, TResult> parent) : base(parent)
    {
    }

    /// <inheritdoc />
    public IUsingTestPipeline<TSUT, TResult> And<TTarget, TSource>()
        => Parent.Using<TTarget, TSource>();

    /// <inheritdoc />
    public IUsingTestPipeline<TSUT, TResult> And<TTarget, TSource>(Func<TSource, TTarget> convert)
        => Parent.Using(convert);

    /// <inheritdoc />
    public IUsingTestPipeline<TSUT, TResult> And<TValue>(
        TValue value,
        For scope = For.All,
        [CallerArgumentExpression(nameof(value))] string? valueExpr = null)
        => Parent.Using(value, scope, valueExpr!);

    /// <inheritdoc />
    public IUsingTestPipeline<TSUT, TResult> And<TValue>(
        Func<TValue> factory,
        For scope = For.All,
        [CallerArgumentExpression(nameof(factory))] string? factoryExpr = null)
        => Parent.Using(factory, scope, factoryExpr!);
}