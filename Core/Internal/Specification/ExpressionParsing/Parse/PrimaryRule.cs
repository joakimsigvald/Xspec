using Xspec.Internal.Specification.ExpressionParsing.Tokenize;
using Xspec.Internal.Specification.ExpressionParsing.Expressions;

namespace Xspec.Internal.Specification.ExpressionParsing.Parse;

/// <summary>
/// Primary expression: identifiers, literals, parenthesized expressions,
/// tuples, array literals, prefix unary fallbacks, and the <c>new</c>
/// expression (delegated to <see cref="NewExprRule"/>).
/// </summary>
internal static class PrimaryRule
{
    public static Expr Parse(TokenStream ts)
    {
        int save = ts.Pos;
        var t = ts.Peek();

        if (t.Kind == TokenKind.Word)
        {
            if (t.Text == "new") return NewExprRule.Parse(ts);
            ts.Advance();
            if (t.Text is "true" or "false" or "null" or "default") return new Literal(t.Text);
            return new Identifier(t.Text, t.Text);
        }
        if (t.Kind is TokenKind.Number or TokenKind.Char)
        {
            ts.Advance();
            return new Literal(t.Text);
        }
        if (t.Kind == TokenKind.String)
        {
            ts.Advance();
            bool interpolated = t.Text.StartsWith('$')
                || (t.Text.Length > 1 && t.Text[0] is '@' or '$' && t.Text[1] == '$');
            return interpolated ? new InterpolatedString(t.Text) : new Literal(t.Text);
        }
        if (t.Kind == TokenKind.Symbol)
        {
            if (t.Text == "(") return ParseParenOrTuple(ts, save);
            if (t.Text == "[")
            {
                ts.Advance();
                if (!ts.TryParse("]", out var items)) return new Unknown(ts.Source.Trim());
                return new ArrayLit(ts.RawFrom(save), items);
            }
            if (t.Text is "-" or "+" or "!" or "~") return UnaryRule.Parse(ts);
        }
        ts.Advance();
        return new Unknown(t.Text);
    }

    private static Expr ParseParenOrTuple(TokenStream ts, int save)
    {
        ts.Advance();                                       // consume '('
        if (ts.AcceptSym(")")) return new TupleExpr(ts.RawFrom(save), []);

        var first = LambdaRule.Parse(ts);
        if (ts.AcceptSym(")")) return first;                // parenthesised expression
        if (!ts.AcceptSym(",")) return new Unknown(ts.Source.Trim());

        var items = ParseTupleRest(ts, first);
        return ts.AcceptSym(")") ? new TupleExpr(ts.RawFrom(save), items) : new Unknown(ts.Source.Trim());
    }

    /// Reads the remaining items of a tuple after the first item and its
    /// trailing comma have been consumed. Stops when no further comma follows.
    private static List<Expr> ParseTupleRest(TokenStream ts, Expr first)
    {
        var items = new List<Expr> { first };
        do items.Add(LambdaRule.Parse(ts));
        while (ts.AcceptSym(","));
        return items;
    }
}