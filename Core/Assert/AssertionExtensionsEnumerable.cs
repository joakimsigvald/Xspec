using System.Runtime.CompilerServices;
using Xspec.Assert.Continuations;
using Xspec.Assert.Continuations.Enumerable;

namespace Xspec.Assert;

/// <summary>
/// Fluent assertions on enumerables
/// </summary>
public static class AssertionExtensionsEnumerable
{
    /// <summary>
    /// Assert that both enumerables are the same instance
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public static ContinueWith<IsEnumerableContinuation<TItem>> Is<TItem>(
        this IEnumerable<TItem> actual,
        IEnumerable<TItem> expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).SameAs(expected, expectedExpr!);

    /// <summary>
    /// Get available assertions for the given enumerable
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsEnumerable<TItem> Is<TItem>(
        this IEnumerable<TItem>? actual,
        Ignore _ = default,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => IsEnumerable<TItem>.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given enumerable
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="actual"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static DoesEnumerable<TItem> Does<TItem>(
        this IEnumerable<TItem>? actual,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => DoesEnumerable<TItem>.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for enumerable
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static HasEnumerable<TItem> Has<TItem>(
        this IEnumerable<TItem>? actual,
        Ignore _ = default,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => HasEnumerable<TItem>.Create(actual, actualExpr!);
}