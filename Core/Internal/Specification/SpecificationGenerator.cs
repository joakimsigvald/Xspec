using System.Runtime.CompilerServices;
using Xspec.Internal.TestData;
using Xunit.Sdk;

namespace Xspec.Internal.Specification;

internal static class SpecificationGenerator
{
    [ThreadStatic]
    private static SpecificationBuilder? _builder;

    [ThreadStatic]
    private static SpecificationAssignments? _assignments;

    internal static void Clear()
    {
        _builder = null;
        _assignments = null;
    }

    internal static void AddMockSetup<TService>(string callExpr)
        => Builder.Add(() => Builder.AddMockSetup<TService>(callExpr));

    internal static void AddMockReturns(string? returnsExpr = null)
        => Builder.Add(() => Builder.AddMockReturns(returnsExpr));

    internal static void AddMockThrowsDefault<TService, TError>()
        => Builder.Add(Builder.AddMockThrowsDefault<TService, TError>);

    internal static void AddMockThrowsDefault<TService>(string expectedExpr)
        => Builder.Add(() => Builder.AddMockThrowsDefault<TService>(expectedExpr));

    internal static void AddMockThrows<TError>()
        => Builder.Add(Builder.AddMockThrows<TError>);

    internal static void AddMockThrows(string expectedExpr)
        => Builder.Add(() => Builder.AddMockThrows(expectedExpr));

    internal static void AddWhen(string actExpr) => Builder.Add(() => Builder.AddWhen(actExpr));

    internal static void AddAfter(string setUpExpr) => Builder.Add(() => Builder.AddAfter(setUpExpr));

    internal static void AddBefore(string tearDownExpr) => Builder.Add(() => Builder.AddBefore(tearDownExpr));

    internal static void Assert(
        Action assert,
        string actual,
        string? expected,
        string verb)
    {
        Builder.Add(() => Builder.AddAssert(actual, verb, expected));
        try
        {
            Builder.SuppressRecording();
            assert();
            Builder.InciteRecording();
        }
        catch (XunitException ex)
        {
            var message = ex.Message;
            var innerXspecTEx = GetExpectedException(ex.InnerException as XunitException);
            if (innerXspecTEx is not null)
                message = $"{message}{Environment.NewLine}{innerXspecTEx.Message}";
            var assignmentList = ListAssignments();
            var specMessage = $"""

                    {Builder.Specification}
                    ----
                    {assignmentList}
                    """;
            throw new XunitException(message, new XunitException(specMessage));
        }
    }

    private static XunitException? GetExpectedException(XunitException? ex)
        => ex is null || ex.Message.StartsWith("Expected")
            ? ex
            : GetExpectedException(ex.InnerException as XunitException);

    internal static void AddThen() => Builder.Add(Builder.AddThen);

    internal static void AddGiven(string valueExpr, For scope)
        => Builder.Add(() => Builder.AddGiven(valueExpr, scope));

    internal static void AddUsing(string valueExpr, For scope)
        => Builder.Add(() => Builder.AddUsing(valueExpr, scope));

    internal static void AddGiven<TValue>(string setupExpr, bool isCustomExpression, string? article = null)
        => Builder.Add(() => Builder.AddGiven<TValue>(setupExpr, isCustomExpression, article));

    internal static void AddGivenCount<TModel>(string count)
        => Builder.Add(() => Builder.AddGivenCount<TModel>(count));

    internal static void AddVerify<TService>(string expressionExpr)
        => Builder.Add(() => Builder.AddVerify<TService>(expressionExpr));

    internal static void AddAssertThrows<TError>(string? binder = null)
        => Builder.Add(() => Builder.AddAssertThrows<TError>(binder));

    internal static void AddAssertThrows(string expectedExpr)
        => Builder.Add(() => Builder.AddAssertThrows(expectedExpr));

    internal static void AddTap(string expr) => Builder.Add(() => Builder.AddTap(expr));

    internal static void AddMockReturnsDefault<TService>(string returnsExpr)
         => Builder.Add(() => Builder.AddMockReturnsDefault<TService>(returnsExpr));

    internal static void AddAssert([CallerMemberName] string? assertName = null)
         => Builder.Add(() => Builder.AddAssert(assertName!));

    internal static void AddAssertConjunction(string conjunction)
         => Builder.Add(() => Builder.AddAssertConjunction(conjunction));

    internal static void AddThat() => Builder.Add(Builder.AddThat);

    internal static void AddUnique<TValue>()
         => Builder.Add(() => Builder.AddUnique(typeof(TValue).Alias()));

    internal static void Assign(Type type, int index, object? value) => Assignments.Assign(type, index, value);

    internal static void TagIndex(Type type, int index, string tagName)
         => Assignments.TagIndex(type, index, tagName);

    internal static string ListAssignments() => Assignments.ListAssignments();

    internal static SpecificationBuilder Builder => _builder ??= new();
    internal static SpecificationAssignments Assignments => _assignments ??= new();
}