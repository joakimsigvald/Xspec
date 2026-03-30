using System.Runtime.CompilerServices;
using static Xunit.Assert;

namespace Xspec.Assert.Continuations.String;

/// <summary>
/// Object that allows an assertions to be made on the provided string
/// </summary>
public record IsString : StringConstraint<IsStringContinuation>
{
    /// <summary>
    /// Asserts that the string is equivalent to expected, ignoring casing and leading or trailing whitespace
    /// actual.Should().BeEquivalentTo(expected)
    /// </summary>
    public ContinueWith<IsStringContinuation> Like(
        string expected,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(
            Describe(expected),
            actual => Equal(actual?.Trim().ToLower(), 
                expected?.Trim().ToLower()), 
            expectedExpr!).And();

    /// <summary>
    /// Asserts that the string is equivalent to expected, ignoring casing and leading or trailing whitespace
    /// actual.Should().BeEquivalentTo(expected)
    /// </summary>
    public ContinueWith<IsStringContinuation> EquivalentTo(
        string expected,
        [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(
            Describe(expected),
            actual => Equal(actual?.Trim().ToLower(), expected?.Trim().ToLower()), 
            expectedExpr!).And();

    /// <summary>
    /// Asserts that the string is null
    /// </summary>
    public ContinueWith<IsStringContinuation> Null()
        => Assert(Ignore.Me, actual => Xunit.Assert.Null(Actual)).And();

    /// <summary>
    /// Asserts that the string is empty
    /// </summary>
    public ContinueWith<IsStringContinuation> Empty()
        => Assert(Ignore.Me, NotNullAnd(Xunit.Assert.Empty)).And();

    /// <summary>
    /// Asserts that the string is null or empty
    /// </summary>
    public ContinueWith<IsStringContinuation> NullOrEmpty()
        => Assert(Ignore.Me, actual => Xunit.Assert.Empty(actual ?? string.Empty)).And();

    /// <summary>
    /// Asserts that the string does not contain non-whitespace characters
    /// </summary>
    public ContinueWith<IsStringContinuation> NullOrWhitespace()
        => Assert(Ignore.Me, actual => Xunit.Assert.Empty((Actual ?? string.Empty).Trim())).And();

    /// <summary>
    /// Asserts that the string is upper case
    /// </summary>
    public ContinueWith<IsStringContinuation> UpperCase() => Assert(Ignore.Me, NotNullAnd(actual => Equal(actual, actual.ToUpper()))).And();

    /// <summary>
    /// Asserts that the string is lower case
    /// </summary>
    public ContinueWith<IsStringContinuation> LowerCase() => Assert(Ignore.Me, NotNullAnd(actual => Equal(actual, actual.ToLower()))).And();

    /// <summary>
    /// Asserts that the string is not the given value
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public ContinueWith<IsStringContinuation> Not(
        string expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(
            Describe(expected),
            actual => NotEqual(actual, expected), 
            expectedExpr!).And();

    internal override ContinueWith<IsStringContinuation> Value(
        string expected, string expectedExpr)
        => Assert(() => Equal(expected, Actual), string.Empty, expectedExpr).And();

    internal override IsStringContinuation Continue() => Create(Actual, ActualExpr);
}