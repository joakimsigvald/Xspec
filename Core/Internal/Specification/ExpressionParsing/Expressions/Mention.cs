namespace Xspec.Internal.Specification.ExpressionParsing.Expressions;

/// <summary>
/// A Mention factory occurrence in an expression tree.
/// <paramref name="Root"/> is the outermost call/generic that bounds it
/// (used by the describer to check for drilldown).
/// </summary>
internal sealed record Mention(Expr Root, string Verb, string TypeArgs, IReadOnlyList<Expr>? Constraints);