using System.Runtime.CompilerServices;
using Xspec.Continuations;
using Xspec.Internal.TestData.Generation.Strategies;

namespace Xspec.Internal.Pipelines;

/// <summary>
/// The internal implementation of the Using&lt;TTarget&gt;() continuation.
/// Defaults to registering the target as a concrete class for subject construction at arrange time,
/// unless From replaces that meaning with a type conversion registration.
/// </summary>
internal class UsingContinuation<TSUT, TResult, TTarget> :
    UsingTestPipeline<TSUT, TResult>,
    IUsingContinuation<TSUT, TResult, TTarget>
{
    private readonly For _scope;
    private bool _isConverted;

    internal UsingContinuation(Spec<TSUT, TResult> parent, For scope) : base(parent)
        => _scope = scope;

    internal bool IsConverted => _isConverted;

    internal void ResolveDefault()
    {
        if (!_isConverted)
            Parent.Pipeline.Using(Parent.Pipeline.InstantiateNew<TTarget>, _scope, string.Empty);
    }

    /// <inheritdoc />
    public IUsingFromContinuation<TSUT, TResult, TSource> From<TSource>()
        => RegisterConversion<TSource>(null);

    /// <inheritdoc />
    public IUsingFromContinuation<TSUT, TResult, TSource> From<TSource>(Func<TSource, TTarget> convert)
        => RegisterConversion(convert ?? throw new SetupFailed("Convert cannot be null"));

    /// <inheritdoc />
    public IUsingTestPipeline<TSUT, TResult> From<TSource>(
        Func<TSource> generate,
        [CallerArgumentExpression(nameof(generate))] string? generateExpr = null)
    {
        if (generate is null)
            throw new SetupFailed("Generate cannot be null");
        VerifyNotConverted();
        var holder = new SequenceHolder();
        Parent.Pipeline.Register<TTarget, TSource>(null, _scope, holder);
        Parent.Pipeline.SetSequence(holder, () => generate());
        _isConverted = true;
        Parent.Pipeline.Specification.AddUsingFactory<TTarget>(_scope, generateExpr!);
        return this;
    }

    private IUsingFromContinuation<TSUT, TResult, TSource> RegisterConversion<TSource>(Func<TSource, TTarget>? convert)
    {
        VerifyNotConverted();
        var holder = new SequenceHolder();
        var from = new FromContinuation<TSUT, TResult, TSource>(Parent, holder);
        Parent.Pipeline.Register(convert, _scope, holder);
        _isConverted = true;
        Parent.Pipeline.Specification.AddUsingConversion<TTarget, TSource>(_scope, from.DescribeSequence);
        return from;
    }

    private void VerifyNotConverted()
    {
        if (_isConverted)
            throw new SetupFailed("From can only be applied once per Using");
    }
}
