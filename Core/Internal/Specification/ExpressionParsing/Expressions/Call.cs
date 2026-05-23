namespace Xspec.Internal.Specification.ExpressionParsing.Expressions;

internal sealed record Call(string Raw, Expr Target, IReadOnlyList<Expr> Args) : Expr(Raw)
{
    public override IEnumerable<Expr> Children => Args.Prepend(Target);

    /// The method/factory name being invoked, or null for non-named targets.
    public string? MethodName => Target switch
    {
        Identifier id => id.Name,
        Member m => m.Name,
        Generic { Target: Identifier gi } => gi.Name,
        Generic { Target: Member gm } => gm.Name,
        _ => null,
    };

    /// If this call directly wraps a Mention factory, its Args become the
    /// mention's constraints. Otherwise the inner mention is passed through.
    public override Mention? AsMention() => Target.AsMention() switch
    {
        null => null,
        var inner when Target is Generic && ReferenceEquals(inner.Root, Target)
            => inner with { Root = this, Constraints = Args.Count > 0 ? Args : null },
        var inner => inner,
    };
}