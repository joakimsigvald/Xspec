using Xspec.Continuations;

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
    public IUsingTestPipeline<TSUT, TResult> From<TSource>()
        => RegisterConversion<TSource>(null);

    /// <inheritdoc />
    public IUsingTestPipeline<TSUT, TResult> From<TSource>(Func<TSource, TTarget> convert)
        => RegisterConversion(convert ?? throw new SetupFailed("Convert cannot be null"));

    private IUsingTestPipeline<TSUT, TResult> RegisterConversion<TSource>(Func<TSource, TTarget>? convert)
    {
        if (_isConverted)
            throw new SetupFailed("From can only be applied once per Using");
        Parent.Pipeline.Register(convert, _scope);
        _isConverted = true;
        Parent.Pipeline.Specification.AddUsingConversion<TTarget, TSource>(_scope);
        return this;
    }
}
