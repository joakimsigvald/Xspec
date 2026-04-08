using Moq;
using System.Runtime.CompilerServices;
using Xspec.Continuations;
using Xspec.Internal.Pipelines;
using Xspec.Internal.Specification;
using Xspec.Internal.TestData;

namespace Xspec;

public abstract partial class Spec<TSUT, TResult> : ITestPipeline<TSUT, TResult>
{
    /// <summary>
    /// Provide any arrangement to the test, which will be applied during test execution in reverse order of where in the test-pipeline it was provided
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <param name="setupExpr"></param>
    /// <returns></returns>
    public IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        Action<TValue> setup,
        [CallerArgumentExpression(nameof(setup))] string? setupExpr = null)
        where TValue : class
    {
        _pipeline.SetDefault(setup, setupExpr!);
        return new GivenTestPipeline<TSUT, TResult>(this);
    }

    /// <summary>
    /// Transform any value and use the transformed value as default
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <param name="transformExpr"></param>
    /// <returns></returns>
    public IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        Func<TValue, TValue> transform,
        [CallerArgumentExpression(nameof(transform))] string? transformExpr = null)
    {
        _pipeline.SetDefault(transform, transformExpr!);
        return new GivenTestPipeline<TSUT, TResult>(this);
    }

    /// <summary>
    /// Provide a tag to setup some expectation, such as associating it with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of value the tag is associated with</typeparam>
    /// <param name="tag">The tag</param>
    /// <param name="tagExpr">Leave empty. Provided by the compiler</param>
    /// <returns></returns>
    public IGivenTag<TSUT, TResult, TValue> Given<TValue>(
        Tag<TValue> tag,
        [CallerArgumentExpression(nameof(tag))] string? tagExpr = null)
        => new GivenTag<TSUT, TResult, TValue>(this, tag, tagExpr!);

    /// <summary>
    /// Provide a default value, that will be applied in all mocks and auto-generated test-data, where no specific value or setup is given.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="defaultValue"></param>
    /// <param name="defaultValueExpr"></param>
    /// <returns></returns>
    [Obsolete("Use `Using` instead")]
    public IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        TValue defaultValue,
        [CallerArgumentExpression(nameof(defaultValue))] string? defaultValueExpr = null)
        => GivenDefault(defaultValue, For.All, defaultValueExpr!);

    /// <summary>
    /// Provide an array of default values, that will be applied in all mocks and auto-generated test-data, where no specific value or setup is given.
    /// It is also mentioned by position so the values can be retrieved by A, ASecond, AThird etc.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="defaultValues"></param>
    /// <param name="defaultValuesExpr"></param>
    /// <returns></returns>
    public IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        TValue[] defaultValues,
        [CallerArgumentExpression(nameof(defaultValues))] string? defaultValuesExpr = null)
    {
        _pipeline.SetDefault(defaultValues, For.All, defaultValuesExpr!);
        defaultValues?.Take(5).Select((value, i) => _pipeline.Assign(i, value)).ToArray();
        return new GivenTestPipeline<TSUT, TResult>(this);
    }

    /// <summary>
    /// A continuation for providing mock-setup for the given type
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    /// <exception cref="SetupFailed"></exception>
    public IGivenServiceContinuation<TSUT, TResult, TService> Given<TService>() where TService : class
        => new GivenServiceContinuation<TSUT, TResult, TService>(this);

    /// <summary>
    /// Return continuation for providing any setup as an action
    /// </summary>
    /// <returns></returns>
    /// <exception cref="SetupFailed"></exception>
    public IGivenContinuation<TSUT, TResult> Given()
        => new GivenContinuation<TSUT, TResult>(this);

    /// <summary>
    /// Provide a default value as a lambda, to be evaluated during test execution AFTER any subsequently added arrangement.
    /// Providing a default value as a lambda, to defer execution, is useful when the default value is created based on test data that is specified later in the test-pipeline.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="defaultValue"></param>
    /// <param name="defaultValueExpr"></param>
    /// <returns></returns>
    [Obsolete("Use `Using` instead")]
    public IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        Func<TValue> defaultValue,
        [CallerArgumentExpression(nameof(defaultValue))] string? defaultValueExpr = null)
        => GivenDefault(defaultValue, For.All, defaultValueExpr!);

    //TODO: Fix
    internal IGivenTestPipeline<TSUT, TResult> Using<TConcrete>()
      => GivenDefault(_pipeline.Instantiate<TConcrete>, For.Subject, typeof(TConcrete).Name);

    internal IGivenTestPipeline<TSUT, TResult> GivenDefault<TValue>(
        TValue defaultValue, For scope, string defaultValueExpr)
    {
        _pipeline.SetDefault(defaultValue, scope, defaultValueExpr);
        return new GivenTestPipeline<TSUT, TResult>(this);
    }

    internal IGivenTestPipeline<TSUT, TResult> GivenDefault<TValue>(
        Func<TValue> value, For scope, string defaultValueExpr)
        => PrependGiven(() => _pipeline.SetDefault(value(), scope, defaultValueExpr));

    //TODO: Remove
    internal IGivenTestPipeline<TSUT, TResult> GivenUnique<TValue>()
        => Continue();

    internal IGivenTestPipeline<TSUT, TResult> Apply<TValue>(
        Action setup,
        string setupExpr,
        bool isCustomExpression = false,
        [CallerMemberName] string? article = null)
        => PrependGiven(() =>
        {
            SpecificationGenerator.AddGiven<TValue>(setupExpr, isCustomExpression, article);
            setup();
        });

    internal IGivenTestPipeline<TSUT, TResult> ApplyMany<TValue>(
        Action setup, [CallerMemberName] string? count = null)
        => PrependGiven(() =>
        {
            SpecificationGenerator.AddGivenCount<TValue>(count!);
            setup();
        });

    internal Mock<TService> GetMock<TService>() where TService : class
        => _pipeline.GetMock<TService>();

    internal void SetupThrows<TService>(Func<Exception> expected)
        => _pipeline.SetupThrows<TService>(expected);

    internal IGivenTestPipeline<TSUT, TResult> AppendGiven(Action given)
    {
        _pipeline.AppendGiven(given);
        return Continue();
    }

    private GivenTestPipeline<TSUT, TResult> PrependGiven(Action given)
    {
        _pipeline.PrependGiven(given);
        return Continue();
    }

    private GivenTestPipeline<TSUT, TResult> Continue() => new(this);
}