using Xspec.Internal.Specification;

namespace Xspec.Architecture;

/// <summary>
/// The set of elements that an architecture rule examines
/// </summary>
/// <typeparam name="T">The type of the elements</typeparam>
public sealed class ArchScope<T>
{
    private readonly SpecificationContext _specification;
    private readonly IReadOnlyCollection<T> _items;
    private readonly string _scope;

    internal ArchScope(SpecificationContext specification, IReadOnlyCollection<T> items, string scope)
    {
        _specification = specification;
        _items = items;
        _scope = scope;
    }

    /// <summary>
    /// Continue to state the invariant that the examined elements satisfy
    /// </summary>
    /// <param name="_">Ignore this parameter — it exists only to allow the named optional because-argument</param>
    /// <param name="because">The reason why the rule exists</param>
    /// <returns>A continuation for stating the invariant</returns>
    public ArchThen<T> Then(Ignore _ = default, string? because = null)
    {
        _specification.AddThen();
        if (because is not null)
            _specification.AddBecause(because);
        return new(_specification, _items, _scope);
    }
}
