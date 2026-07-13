using System.Runtime.CompilerServices;
using Xspec.Internal.Specification;
using Xunit.Sdk;

namespace Xspec.Architecture;

/// <summary>
/// The examined elements of an architecture rule, as the subject of the invariant
/// </summary>
/// <typeparam name="T">The type of the examined elements</typeparam>
public sealed class They<T>
{
    private readonly SpecificationContext _specification;
    private readonly IReadOnlyCollection<T> _items;
    private readonly string _scope;

    internal They(SpecificationContext specification, IReadOnlyCollection<T> items, string scope)
    {
        _specification = specification;
        _items = items;
        _scope = scope;
    }

    /// <summary>
    /// Assert that the given selection is empty for each of the examined elements.
    /// A failure lists the violations one per line.
    /// </summary>
    /// <typeparam name="TItem">The type of the selected items</typeparam>
    /// <param name="select">Selects, for each element, the items that violate the rule</param>
    /// <param name="selectExpr">Captured automatically by the compiler — do not provide</param>
    public void HaveNo<TItem>(
        Func<T, IEnumerable<TItem>> select,
        [CallerArgumentExpression(nameof(select))] string? selectExpr = null)
    {
        var selection = selectExpr.DescribeSelector();
        Violation<T, TItem>[] violations = [.. _items.SelectMany(
            subject => select(subject).Select(item => new Violation<T, TItem>(subject, item)))];
        _specification.Assert(
            () =>
            {
                if (violations.Length > 0)
                    throw new XunitException(
                        $"Expected {_scope} to have no {selection} but found {Describe(violations)}");
            },
            "they", selection, "have no");
    }

    private static string Describe<TSubject, TItem>(IReadOnlyCollection<Violation<TSubject, TItem>> violations)
        => $"{violations.Count} violation{(violations.Count == 1 ? string.Empty : "s")}:"
            + string.Concat(violations.Select(violation => $"{Environment.NewLine}  {violation}"));
}
