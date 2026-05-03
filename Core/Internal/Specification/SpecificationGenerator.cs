using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace Xspec.Internal.Specification;

internal static class SpecificationGenerator
{
    internal static void Assert(
        Action assert,
        string actual,
        string? expected,
        string verb)
    {
        SpecificationContext.StaticBuilder.Add(() => SpecificationContext.StaticBuilder.AddAssert(actual, verb, expected));
        try
        {
            SpecificationContext.StaticBuilder.SuppressRecording();
            assert();
            SpecificationContext.StaticBuilder.InciteRecording();
        }
        catch (XunitException ex)
        {
            var message = ex.Message;
            var innerXspecTEx = GetExpectedException(ex.InnerException as XunitException);
            if (innerXspecTEx is not null)
                message = $"{message}{Environment.NewLine}{innerXspecTEx.Message}";
            var assignmentList = SpecificationContext.ListAssignments();
            var specMessage = $"""

                    {SpecificationContext.StaticBuilder}
                    ----
                    {assignmentList}
                    """;
            throw new XunitException(message, new XunitException(specMessage));
        }
    }

    internal static void AddThen() => SpecificationContext.StaticBuilder.Add(SpecificationContext.StaticBuilder.AddThen);

    internal static void AddVerify<TService>(string expressionExpr)
        => SpecificationContext.StaticBuilder.Add(() => SpecificationContext.StaticBuilder.AddVerify<TService>(expressionExpr));

    internal static void AddAssertThrows<TError>(string? binder = null)
        => SpecificationContext.StaticBuilder.Add(() => SpecificationContext.StaticBuilder.AddAssertThrows<TError>(binder));

    internal static void AddAssertThrows(string expectedExpr)
        => SpecificationContext.StaticBuilder.Add(() => SpecificationContext.StaticBuilder.AddAssertThrows(expectedExpr));

    internal static void AddAssert([CallerMemberName] string? assertName = null)
         => SpecificationContext.StaticBuilder.Add(() => SpecificationContext.StaticBuilder.AddAssert(assertName!));

    internal static void AddAssertConjunction(string conjunction)
         => SpecificationContext.StaticBuilder.Add(() => SpecificationContext.StaticBuilder.AddAssertConjunction(conjunction));

    internal static void AddThat() => SpecificationContext.StaticBuilder.Add(SpecificationContext.StaticBuilder.AddThat);

    private static XunitException? GetExpectedException(XunitException? ex)
        => ex is null || ex.Message.StartsWith("Expected")
            ? ex
            : GetExpectedException(ex.InnerException as XunitException);
}