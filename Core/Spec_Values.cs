using Xspec.Continuations;

namespace Xspec;

public abstract partial class Spec<TSUT, TResult> : ITestPipeline<TSUT, TResult>
{
    /// <summary>
    /// Yields an array with one element of the given type. The same array will be returned if called several times.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue[] One<TValue>() => _pipeline.MentionMany<TValue>(1);

    /// <summary>
    /// Yields an array with the provided element. Calling One() with the same type parameter again will yield the same array.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue[] One<TValue>(TValue value)
    {
        _pipeline.Assign(0, value);
        return _pipeline.MentionMany<TValue>(1);
    }

    /// <summary>
    /// Yields an array with the provided elements. Only up to five elements will be included in the returned array.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue[] Some<TValue>(IEnumerable<TValue> values)
    {
        var arr = values.Take(5).ToArray();
        for (var i = 0; i < arr.Length; i++)
            _pipeline.Assign(i, arr[i]);
        return _pipeline.AssignMany(arr);
    }

    /// <summary>
    /// Yields an array with one element of the given type, after setup has been applied to the element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue[] One<TValue>(Action<TValue> setup) => _pipeline.ApplyMany<TValue>(setup, 1);

    /// <summary>
    /// Yields an array with one element of the given type, after transform has been applied to the element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue[] One<TValue>(Func<TValue, TValue> transform) => _pipeline.ApplyMany<TValue>(transform, 1);

    /// <summary>
    /// Yields an array with two elements of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue[] Two<TValue>() => _pipeline.MentionMany<TValue>(2);

    /// <summary>
    /// Yields an array with two elements of the given type, with setup applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue[] Two<TValue>(Action<TValue> setup) => _pipeline.ApplyMany<TValue>(setup, 2);

    /// <summary>
    /// Yields an array with two elements of the given type, with setup applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue[] Two<TValue>(Action<TValue, int> setup) => _pipeline.ApplyMany<TValue>(setup, 2);

    /// <summary>
    /// Yields an array with two elements of the given type, with transform applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue[] Two<TValue>(Func<TValue, TValue> transform) => _pipeline.ApplyMany<TValue>(transform, 2);

    /// <summary>
    /// Yields an array with two elements of the given type, with transform applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue[] Two<TValue>(Func<TValue, int, TValue> transform) 
        => _pipeline.ApplyMany<TValue>(transform, 2);

    /// <summary>
    /// Yields an array with three elements of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue[] Three<TValue>() => _pipeline.MentionMany<TValue>(3);

    /// <summary>
    /// Yields an array with three elements of the given type, with setup applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue[] Three<TValue>(Action<TValue> setup) => _pipeline.ApplyMany<TValue>(setup, 3);

    /// <summary>
    /// Yields an array with three elements of the given type, with setup applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue[] Three<TValue>(Action<TValue, int> setup) => _pipeline.ApplyMany<TValue>(setup, 3);

    /// <summary>
    /// Yields an array with three elements of the given type, with transform applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue[] Three<TValue>(Func<TValue, TValue> transform) => _pipeline.ApplyMany<TValue>(transform, 3);

    /// <summary>
    /// Yields an array with three elements of the given type, with transform applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue[] Three<TValue>(Func<TValue, int, TValue> transform) 
        => _pipeline.ApplyMany<TValue>(transform, 3);

    /// <summary>
    /// Yields an array with four elements of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue[] Four<TValue>() => _pipeline.MentionMany<TValue>(4);

    /// <summary>
    /// Yields an array with four elements of the given type, with setup applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue[] Four<TValue>(Action<TValue> setup) => _pipeline.ApplyMany<TValue>(setup, 4);

    /// <summary>
    /// Yields an array with four elements of the given type, with setup applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue[] Four<TValue>(Action<TValue, int> setup) => _pipeline.ApplyMany<TValue>(setup, 4);

    /// <summary>
    /// Yields an array with four elements of the given type, with transform applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue[] Four<TValue>(Func<TValue, TValue> transform) => _pipeline.ApplyMany<TValue>(transform, 4);

    /// <summary>
    /// Yields an array with four elements of the given type, with transform applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue[] Four<TValue>(Func<TValue, int, TValue> transform) 
        => _pipeline.ApplyMany<TValue>(transform, 4);

    /// <summary>
    /// Yields an array with five elements of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue[] Five<TValue>() => _pipeline.MentionMany<TValue>(5);

    /// <summary>
    /// Yields an array with five elements of the given type, with setup applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue[] Five<TValue>(Action<TValue> setup) => _pipeline.ApplyMany<TValue>(setup, 5);

    /// <summary>
    /// Yields an array with five elements of the given type, with setup applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected internal TValue[] Five<TValue>(Action<TValue, int> setup) => _pipeline.ApplyMany<TValue>(setup, 5);

    /// <summary>
    /// Yields an array with five elements of the given type, with transform applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue[] Five<TValue>(Func<TValue, TValue> transform) => _pipeline.ApplyMany<TValue>(transform, 5);

    /// <summary>
    /// Yields an array with five elements of the given type, with transform applied to each element
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected internal TValue[] Five<TValue>(Func<TValue, int, TValue> transform)
        => _pipeline.ApplyMany<TValue>(transform, 5);

    /// <summary>
    /// Yields an array with zero elements of the given type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue[] Zero<TValue>() => _pipeline.MentionMany<TValue>(0);

    /// <summary>
    /// Yields an array with any number of elements, including zero, of the given type. 
    /// If such an array exists it will be returned, otherwise and array with one element will be returned.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue[] AnyNumberOf<TValue>() => _pipeline.MentionMany<TValue>(1, 0);

    /// <summary>
    /// Yields an array with at least one elements of the given type. 
    /// If such an array exists it will be returned, otherwise and array with two elements will be returned.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue[] Some<TValue>() => _pipeline.MentionMany<TValue>(2, 1);

    /// <summary>
    /// Yields an array with at least two elements of the given type. 
    /// If such an array exists it will be returned, otherwise and array with three elements will be returned.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected internal TValue[] Many<TValue>() => _pipeline.MentionMany<TValue>(3, 2);

    /// <summary>
    /// Yields an array with some elements of the given type, different from other generated elements. 
    /// The elements are generated by calling Another internally. 
    /// The number of elements to generate can be specified but defaults to two
    /// </summary>
    /// <typeparam name="TValue">The type of values to generate</typeparam>
    /// <param name="count">The number of values to generate, default to 2</param>
    /// <returns>The generated array</returns>
    protected internal TValue[] SomeOther<TValue>(int count = 2)
        => [.. Enumerable.Range(0, count).Select(_ => Another<TValue>())];

    /// <summary>
    /// Yields an array with some elements of the given type and with a given setup, different from other generated elements. 
    /// The elements are generated by calling Another internally. 
    /// The number of elements to generate can be specified but defaults to two
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup">Provide the setup as a function to mutate the model, typically used with class</param>
    /// <param name="count">The number of values to generate, default to 2</param>
    /// <returns>The generated array</returns>
    protected internal TValue[] SomeOther<TValue>(Action<TValue> setup, int count = 2)
        => [.. Enumerable.Range(0, count).Select(_ => Another(setup))];

    /// <summary>
    /// Yields an array with some elements of the given type and with a given setup, different from other generated elements. 
    /// The elements are generated by calling Another internally. 
    /// The number of elements to generate can be specified but defaults to two
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup">Provide the setup as a function to mutate the model, typically used with class</param>
    /// <param name="count">The number of values to generate, default to 2</param>
    /// <returns>The generated array</returns>
    protected internal TValue[] SomeOther<TValue>(Action<TValue, int> setup, int count = 2)
        => [.. Enumerable.Range(0, count).Select(i => Another<TValue>(m => setup(m, i)))];

    /// <summary>
    /// Yields an array with some elements of the given type and with a given setup, different from other generated elements. 
    /// The elements are generated by calling Another internally. 
    /// The number of elements to generate can be specified but defaults to two
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform">Provide the setup as a function to transform the model, typically used with record</param>
    /// <param name="count">The number of values to generate, default to 2</param>
    /// <returns>The generated array</returns>
    protected internal TValue[] SomeOther<TValue>(Func<TValue, TValue> transform, int count = 2)
        => [.. Enumerable.Range(0, count).Select(_ => transform(Another<TValue>()))];

    /// <summary>
    /// Yields an array with some elements of the given type and with a given setup, different from other generated elements. 
    /// The elements are generated by calling Another internally. 
    /// The number of elements to generate can be specified but defaults to two
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform">Provide the setup as a function to transform the model, typically used with record</param>
    /// <param name="count">The number of values to generate, default to 2</param>
    /// <returns>The generated array</returns>
    protected internal TValue[] SomeOther<TValue>(Func<TValue, int, TValue> transform, int count = 2)
        => [.. Enumerable.Range(0, count).Select(i => transform(Another<TValue>(), i))];
}