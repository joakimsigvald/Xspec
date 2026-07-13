namespace Xspec.Architecture;

/// <summary>
/// One violation of an architecture rule, rendered as "subject: item"
/// </summary>
internal sealed record Violation<TSubject, TItem>(TSubject Subject, TItem Item)
{
    public override string ToString() => $"{Subject}: {Item}";
}
