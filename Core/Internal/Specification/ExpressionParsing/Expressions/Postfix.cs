namespace Xspec.Internal.Specification.ExpressionParsing.Expressions;

internal sealed record Postfix(string Raw, string Op, Expr Operand) : Expr(Raw)
{
    public override IEnumerable<Expr> Children => [Operand];
}