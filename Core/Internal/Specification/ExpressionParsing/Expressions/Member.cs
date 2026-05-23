namespace Xspec.Internal.Specification.ExpressionParsing.Expressions;

internal sealed record Member(string Raw, Expr Target, string Name) : Expr(Raw)
{
    public override IEnumerable<Expr> Children => [Target];
    public override string AsPath() => $"{Target.AsPath()}.{Name}";
    public override Mention? AsMention() => Target.AsMention();
}