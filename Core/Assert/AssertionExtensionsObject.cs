using System.Runtime.CompilerServices;
using Xspec.Assert.Continuations;

namespace Xspec.Assert;

/// <summary>
/// Fluent assertions on Object
/// </summary>
public static class AssertionExtensionsObject
{
    /// <summary>
    /// Verify that actual object is same reference as expected and return continuation for further assertions of the object
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr"></param>
    /// <param name="expectedExpr"></param>
    /// <returns>Continuation for further assertions of the object</returns>
    public static ContinueWith<IsObject> Is(
        this object actual,
        object expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Verify that actual struct is same as expected
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="actualExpr"></param>
    /// <param name="expectedExpr"></param>
    /// <returns>Continuation for further assertions of the struct</returns>
    public static ContinueWith<IsObject> Is<TValue>(
        this TValue actual,
        TValue expected,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        where TValue : struct
        => actual.Is(actualExpr: actualExpr!).Value(expected, expectedExpr!);

    /// <summary>
    /// Get available assertions for the given object
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static IsObject Is(
        this object actual,
        Ignore _ = default,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => IsObject.Create(actual, actualExpr!);

    /// <summary>
    /// Get available assertions for the given object
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="_"></param>
    /// <param name="actualExpr"></param>
    /// <returns></returns>
    public static HasObject Has(
        this object actual,
        Ignore _ = default,
        [CallerArgumentExpression(nameof(actual))] string? actualExpr = null)
        => HasObject.Create(actual, actualExpr!);
}