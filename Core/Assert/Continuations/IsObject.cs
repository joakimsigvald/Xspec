using System.Runtime.CompilerServices;
using static Xunit.Assert;

namespace Xspec.Assert.Continuations;

/// <summary>
/// Object that allows an assertions to be made on the provided object
/// </summary>
public record IsObject : Constraint<object, IsObject>
{
    /// <summary>
    /// Assert that the object is not same as the given object
    /// </summary>
    /// <returns>A continuation for making additional asserts on the object</returns>
    public ContinueWith<IsObject> Not(
        object expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(Describe(expected), actual => NotSame(expected, actual), expectedExpr!).And();

    /// <summary>
    /// Assert that the object is null
    /// </summary>
    public ContinueWith<IsObject> Null() => Assert(Ignore.Me, Xunit.Assert.Null).And();

    /// <summary>
    /// Assert that the object is equal to the given object
    /// </summary>
    /// <returns>A continuation for making additional asserts on the object</returns>
    public ContinueWith<IsObject> EqualTo(
        object expected,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(Describe(expected), actual => Equal(expected, actual), expectedExpr!).And();

    /// <summary>
    /// Assert that the object is equivalent to the given object with respect to public fields and properties, but ignoring type
    /// (if it walks like a duck...)
    /// </summary>
    /// <returns>A continuation for making additional asserts on the object</returns>
    public ContinueWith<IsObject> Like(
        object expected,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(Describe(expected), actual => Equivalent(expected, actual), expectedExpr!).And();

    /// <summary>
    /// Assert that the object is equivalent to the given object with respect to public fields and properties, but ignoring type
    /// (if it walks like a duck...)
    /// </summary>
    /// <returns>A continuation for making additional asserts on the object</returns>
    public ContinueWith<IsObject> EquivalentTo(
        object expected,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(Describe(expected), actual => Equivalent(expected, actual), expectedExpr!).And();
}