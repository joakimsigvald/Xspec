using System.Runtime.CompilerServices;
using Xspec.Continuations;
using Xspec.Internal.Pipelines;

namespace Xspec;

public abstract partial class Spec<TSUT, TResult> : ITestPipeline<TSUT, TResult>
{
    /// <summary>
    /// Use a concrete class for auto-mocking of subject
    /// </summary>
    /// <typeparam name="TConcrete"></typeparam>
    /// <returns></returns>
    public IUsingTestPipeline<TSUT, TResult> Using<TConcrete>()
    {
        _pipeline.Using(_pipeline.Instantiate<TConcrete>, For.Subject, typeof(TConcrete).Name);
        return new UsingTestPipeline<TSUT, TResult>(this);
    }

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

    /// <summary>
    /// Instructs the test pipeline to use the specified instance when resolving dependencies or generating test data.
    /// </summary>
    /// <typeparam name="TValue">The type of the value being provided.</typeparam>
    /// <param name="value">The specific instance to use.</param>
    /// <param name="scope">Determines whether the value is used for Subject Under Test construction (Subject), ambient test data (Input), or both (All). Defaults to All.</param>
    /// <param name="valueExpr">Automatically populated by the compiler to capture the argument expression.</param>
    /// <returns>A continuation to provide further infrastructure and test data arrangement.</returns>
    public IUsingTestPipeline<TSUT, TResult> Using<TValue>(
        TValue value,
        For scope = For.All,
        [CallerArgumentExpression(nameof(value))] string? valueExpr = null)
    {
        _pipeline.Using(value, scope, valueExpr!);
        return new UsingTestPipeline<TSUT, TResult>(this);
    }

    /// <summary>
    /// Instructs the test pipeline to use a factory method to resolve the value when generating test data or resolving dependencies.
    /// </summary>
    /// <typeparam name="TValue">The type of the value being provided.</typeparam>
    /// <param name="factory">A function that creates the value.</param>
    /// <param name="scope">Determines whether the factory is used for Subject Under Test construction (Subject), ambient test data (Input), or both (All). Defaults to All.</param>
    /// <param name="factoryExpr">Automatically populated by the compiler.</param>
    /// <returns>A continuation to provide further infrastructure and test data arrangement.</returns>
    public IUsingTestPipeline<TSUT, TResult> Using<TValue>(
        Func<TValue> factory,
        For scope = For.All,
        [CallerArgumentExpression(nameof(factory))] string? factoryExpr = null)
    {
        _pipeline.AppendUsing(() => _pipeline.Using(factory, scope, factoryExpr!));
        return new UsingTestPipeline<TSUT, TResult>(this);
    }

    /// <summary>
    /// Instructs the test pipeline to use the value associated with the specified tag when resolving dependencies or generating test data.
    /// </summary>
    /// <typeparam name="TValue">The type of the value associated with the tag.</typeparam>
    /// <param name="tag">The tag used to identify the specific value instance.</param>
    /// <param name="scope">Determines whether the tag's value is used for Subject Under Test construction (Subject), ambient test data (Input), or both (All). Defaults to All.</param>
    /// <param name="tagExpr">Automatically populated by the compiler to capture the argument expression.</param>
    /// <returns>A continuation to provide further infrastructure and test data arrangement.</returns>
    public IUsingTestPipeline<TSUT, TResult> Using<TValue>(
        Tag<TValue> tag,
        For scope = For.All,
        [CallerArgumentExpression(nameof(tag))] string? tagExpr = null)
    {
        _pipeline.AppendUsing(() => _pipeline.Using(() => The(tag), scope, tagExpr!));
        return new UsingTestPipeline<TSUT, TResult>(this);
    }
}