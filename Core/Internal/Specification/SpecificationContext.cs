namespace Xspec.Internal.Specification;

internal class SpecificationContext
{
    [ThreadStatic]
    private static SpecificationBuilder? _builder;

    [ThreadStatic]
    private static SpecificationAssignments? _assignments;

    public override string ToString() => Builder.ToString();

    internal void AddWhen(string actExpr) => Builder.Add(() => Builder.AddWhen(actExpr));

    internal void AddAfter(string setUpExpr) => Builder.Add(() => Builder.AddAfter(setUpExpr));

    internal void AddBefore(string tearDownExpr) => Builder.Add(() => Builder.AddBefore(tearDownExpr));

    internal void AddGiven(string valueExpr, For scope) => Builder.Add(() => Builder.AddGiven(valueExpr, scope));

    internal void AddUsing(string valueExpr, For scope) => Builder.Add(() => Builder.AddUsing(valueExpr, scope));

    internal void AddGiven<TValue>(string setupExpr, bool isCustomExpression, string? article = null)
        => Builder.Add(() => Builder.AddGiven<TValue>(setupExpr, isCustomExpression, article));

    internal void AddGivenCount<TModel>(string count)
        => Builder.Add(() => Builder.AddGivenCount<TModel>(count));

    internal void AddGivenThat(string customArrangementExpr)
        => Builder.Add(() => Builder.AddGivenThat(customArrangementExpr));

    internal void AddMockSetup<TService>(string callExpr)
        => Builder.Add(() => Builder.AddMockSetup<TService>(callExpr));

    internal void AddMockReturns(string? returnsExpr = null)
        => Builder.Add(() => Builder.AddMockReturns(returnsExpr));

    internal void AddMockThrowsDefault<TService, TError>()
        => Builder.Add(Builder.AddMockThrowsDefault<TService, TError>);

    internal void AddMockThrowsDefault<TService>(string expectedExpr)
        => Builder.Add(() => Builder.AddMockThrowsDefault<TService>(expectedExpr));

    internal void AddMockThrows<TError>()
        => Builder.Add(Builder.AddMockThrows<TError>);

    internal void AddMockThrows(string expectedExpr)
        => Builder.Add(() => Builder.AddMockThrows(expectedExpr));

    internal void AddMockReturnsDefault<TService>(string returnsExpr)
         => Builder.Add(() => Builder.AddMockReturnsDefault<TService>(returnsExpr));

    internal void AddTap(string expr) => Builder.Add(() => Builder.AddTap(expr));

    internal void TagIndex(Type type, int index, string tagName)
         => Assignments.TagIndex(type, index, tagName);

    internal void Assign(Type type, int index, object? value) => Assignments.Assign(type, index, value);

    private static SpecificationBuilder Builder => _builder ??= new();

    private static SpecificationAssignments Assignments => _assignments ??= new();

    internal static SpecificationBuilder StaticBuilder => Builder;

    internal static SpecificationAssignments StaticAssignments => Assignments;

    internal void Release()
    {
        _builder = null;
        _assignments = null;
    }

    internal static object ListAssignments() => StaticAssignments.ListAssignments();
}