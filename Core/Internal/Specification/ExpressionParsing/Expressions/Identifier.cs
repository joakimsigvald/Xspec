namespace Xspec.Internal.Specification.ExpressionParsing.Expressions;

internal sealed record Identifier(string Raw, string Name) : Expr(Raw)
{
    public override string AsPath() => Name;
}