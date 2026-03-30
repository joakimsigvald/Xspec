using System.Runtime.CompilerServices;
using Xspec.Internal.Specification;

namespace Xspec.Assert.Continuations.String;

/// <summary>
/// Object that allows an assertions to be made on the provided string
/// </summary>
public record DoesString : StringConstraint<DoesStringContinuation>
{
    /// <summary>
    /// Asserts that the string contains the expected string
    /// </summary>
    public ContinueWith<DoesStringContinuation> Contain(
        string expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(
            Describe(expected), 
            actual => Xunit.Assert.Contains(expected, actual), 
            expectedExpr!, 
            verbalizationStrategy: VerbalizationStrategy.PresentSingularS).And();

    /// <summary>
    /// Asserts that the string starts with a prefix
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public ContinueWith<DoesStringContinuation> StartWith(
        string expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(
            Describe(expected),
            actual => Xunit.Assert.StartsWith(expected, actual),
            expectedExpr!,
            verbalizationStrategy: VerbalizationStrategy.PresentSingularS).And();

    /// <summary>
    /// Asserts that the string ends with a suffix
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="expectedExpr"></param>
    /// <returns></returns>
    public ContinueWith<DoesStringContinuation> EndWith(
        string expected, [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null)
        => Assert(
            Describe(expected),
            actual => Xunit.Assert.EndsWith(expected, actual), 
            expectedExpr!, 
            verbalizationStrategy: VerbalizationStrategy.PresentSingularS)
        .And();

    internal override DoesStringContinuation Continue() => Create(Actual, ActualExpr);
}