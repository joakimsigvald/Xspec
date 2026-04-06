using Xspec.Continuations;
using Xspec.Internal.Pipelines;

namespace Xspec;

public abstract partial class Spec<TSUT, TResult> : ITestPipeline<TSUT, TResult>
{
    /// <summary>
    /// Registers a type conversion strategy. Whenever the target type is requested, the generator will first generate the source type and cast it.
    /// </summary>
    /// <typeparam name="TTarget">The type being requested by the pipeline.</typeparam>
    /// <typeparam name="TSource">The underlying primitive or source type to generate first.</typeparam>
    /// <returns>A continuation to provide further infrastructure and test data arrangement.</returns>
    public IUsingTestPipeline<TSUT, TResult> Using<TTarget, TSource>()
    {
        _pipeline.Register<TTarget, TSource>();
        return new UsingTestPipeline<TSUT, TResult>(this);
    }

    /// <summary>
    /// Registers a type conversion strategy with a specific conversion function. Whenever the target type is requested, the generator will generate the source type and apply the conversion.
    /// </summary>
    /// <typeparam name="TTarget">The type being requested by the pipeline.</typeparam>
    /// <typeparam name="TSource">The underlying primitive or source type to generate first.</typeparam>
    /// <param name="convert">The function used to convert the source type into the target type.</param>
    /// <returns>A continuation to provide further infrastructure and test data arrangement.</returns>
    public IUsingTestPipeline<TSUT, TResult> Using<TTarget, TSource>(Func<TSource, TTarget> convert)
    {
        _pipeline.Register(convert);
        return new UsingTestPipeline<TSUT, TResult>(this);
    }
}