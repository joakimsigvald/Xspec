namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Ternary <c>?:</c> level.
/// </summary>
internal static class ConditionalRule
{
    public static Expr Parse(TokenStream ts)
    {
        int save = ts.Pos;
        var cond = BinaryRule.Parse(ts, BinaryRule.MinPrecedence);
        if (!ts.AcceptSym("?")) return cond;
        var thenExpr = LambdaRule.Parse(ts);
        if (!ts.AcceptSym(":")) return new Unknown(ts.Source.Trim());
        return new Conditional(ts.RawFrom(save), cond, thenExpr, LambdaRule.Parse(ts));
    }
}
