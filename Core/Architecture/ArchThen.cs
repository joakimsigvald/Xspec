using Xspec.Internal.Specification;

namespace Xspec.Architecture;

/// <summary>
/// Continuation for stating the invariant of an architecture rule
/// </summary>
/// <typeparam name="T">The type of the examined elements</typeparam>
public sealed class ArchThen<T>
{
    private readonly SpecificationContext _specification;
    private readonly IReadOnlyCollection<T> _items;
    private readonly string _scope;

    internal ArchThen(SpecificationContext specification, IReadOnlyCollection<T> items, string scope)
    {
        _specification = specification;
        _items = items;
        _scope = scope;
    }

    /// <summary>
    /// The examined elements, as the subject of the invariant
    /// </summary>
    public They<T> They => new(_specification, _items, _scope);
}
