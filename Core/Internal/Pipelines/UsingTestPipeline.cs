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
}