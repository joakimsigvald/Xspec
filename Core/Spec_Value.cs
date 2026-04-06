using Xspec.Continuations;

namespace Xspec;

public abstract partial class Spec<TSUT, TResult> : ITestPipeline<TSUT, TResult>
{

    /// <summary>
    /// Yields a value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue A<TValue>() => _pipeline.Mention<TValue>();

    /// <summary>
    /// Yields a value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue An<TValue>() => _pipeline.Mention<TValue>();

    /// <summary>
    /// Reference an auto-generated, or previously provided value of the given type. 
    /// Using `The` is synonymous to `A` or `An`, but suggest that this value has been provided or referenced earlier in the test pipeline. 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue The<TValue>() => _pipeline.Mention<TValue>();

    /// <summary>
    /// Reference an auto-generated, or previously provided value by the given tag
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="tag">The tag is used to distinguish between different values. Each tag instance corresponds to one value</param>
    /// <returns>The value associated to the tag</returns>
    protected internal TValue The<TValue>(Tag<TValue> tag) => _pipeline.Mention(tag, tag.Name);

    /// <summary>
    /// Yields a value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue AFirst<TValue>() => _pipeline.Mention<TValue>();

    /// <summary>
    /// Yields a value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue TheFirst<TValue>() => _pipeline.Mention<TValue>();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue A<TValue>(Action<TValue> setup) 
        => _pipeline.Apply(setup, 0);

    /// <summary>
    /// Yields a customized value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue An<TValue>(Action<TValue> setup)
        => _pipeline.Apply(setup, 0);

    /// <summary>
    /// Yields a customized value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue AFirst<TValue>(Action<TValue> setup)
        => _pipeline.Apply(setup, 0);

    /// <summary>
    /// Yields a customized value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue A<TValue>(Func<TValue, TValue> transform) => _pipeline.Apply(transform, 0);

    /// <summary>
    /// Yields a customized value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue An<TValue>(Func<TValue, TValue> transform) => _pipeline.Apply(transform, 0);

    /// <summary>
    /// Yields a customized value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue AFirst<TValue>(Func<TValue, TValue> transform) => _pipeline.Apply(transform, 0);

    /// <summary>
    /// Provide a specific value for the first instance of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    protected internal TValue AFirst<TValue>(TValue value) => _pipeline.Assign(0, value);

    /// <summary>
    /// Provide a specific value of the given type, that can be referenced at different points of the test, with the keywords, A, An or The
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    protected internal TValue A<TValue>(TValue value) => _pipeline.Assign(0, value);

    /// <summary>
    /// Yields a second value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue TheSecond<TValue>() => _pipeline.Mention<TValue>(1);

    /// <summary>
    /// Yields a second value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue ASecond<TValue>() => _pipeline.Mention<TValue>(1);

    /// <summary>
    /// Yields a customized second value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue ASecond<TValue>(Action<TValue> setup) => _pipeline.Apply(setup, 1);

    /// <summary>
    /// Provide transform for a second value of a given type, that can be mentioned in the test pipeline as ASecond or TheSecond.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue ASecond<TValue>(Func<TValue, TValue> transform) => _pipeline.Apply(transform, 1);

    /// <summary>
    /// Provide a specific value for the second instance of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    protected internal TValue ASecond<TValue>(TValue value) => _pipeline.Assign(1, value);

    /// <summary>
    /// Yields a third value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue TheThird<TValue>() => _pipeline.Mention<TValue>(2);

    /// <summary>
    /// Yields a third value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue AThird<TValue>() => _pipeline.Mention<TValue>(2);

    /// <summary>
    /// Yields a customized third value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue AThird<TValue>(Action<TValue> setup) => _pipeline.Apply(setup, 2);

    /// <summary>
    /// Provide transform for a third value of a given type, that can be mentioned in the test pipeline as AThird or TheThird.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue AThird<TValue>(Func<TValue, TValue> transform) => _pipeline.Apply(transform, 2);

    /// <summary>
    /// Provide a specific value for the third instance of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    protected internal TValue AThird<TValue>(TValue value) => _pipeline.Assign(2, value);

    /// <summary>
    /// Yields a fourth value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue TheFourth<TValue>() => _pipeline.Mention<TValue>(3);

    /// <summary>
    /// Yields a fourth value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue AFourth<TValue>() => _pipeline.Mention<TValue>(3);

    /// <summary>
    /// Yields a customized fourth value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue AFourth<TValue>(Action<TValue> setup) => _pipeline.Apply(setup, 3);

    /// <summary>
    /// Provide transform for a fourth value of a given type, that can be mentioned in the test pipeline as AFourth or TheFourth.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue AFourth<TValue>(Func<TValue, TValue> transform) => _pipeline.Apply(transform, 3);

    /// <summary>
    /// Provide a specific value for the fourth instance of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    protected internal TValue AFourth<TValue>(TValue value) => _pipeline.Assign(3, value);

    /// <summary>
    /// Yields a fifth value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue TheFifth<TValue>() => _pipeline.Mention<TValue>(4);

    /// <summary>
    /// Yields a fifth value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue AFifth<TValue>() => _pipeline.Mention<TValue>(4);

    /// <summary>
    /// Yields a customized fifth value of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue AFifth<TValue>(Action<TValue> setup) => _pipeline.Apply(setup, 4);

    /// <summary>
    /// Provide a specific value for the fifth instance of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    protected internal TValue AFifth<TValue>(TValue value) => _pipeline.Assign(4, value);

    /// <summary>
    /// Provide transform for a fifth value of a given type, that can be mentioned in the test pipeline as AFifth or TheFifth.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue AFifth<TValue>(Func<TValue, TValue> transform) => _pipeline.Apply(transform, 4);

    /// <summary>
    /// Yields a value of the given type that cannot be retrieved again
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue Any<TValue>() => _pipeline.Mention<TValue>(null);

    /// <summary>
    /// Yields a customized value of the given type that cannot be retrieved again
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue Any<TValue>(Action<TValue> setup) => _pipeline.Create(setup);

    /// <summary>
    /// Yields a value of the given type that cannot be retrieved again
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue Another<TValue>() => _pipeline.Mention<TValue>(null);

    /// <summary>
    /// Yields a customized value of the given type that cannot be retrieved again
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue Another<TValue>(Action<TValue> setup) => _pipeline.Create(setup);

    internal TValue Assign<TValue>(Tag<TValue> tag, TValue value, string tagName) => _pipeline.Assign(tag, value, tagName);

    internal TValue Apply<TValue>(Tag<TValue> tag, Action<TValue> setup, string tagName) => _pipeline.Apply(tag, setup, tagName);

    internal TValue Apply<TValue>(Tag<TValue> tag, Func<TValue, TValue> transform, string tagName) => _pipeline.Apply(tag, transform, tagName);
}