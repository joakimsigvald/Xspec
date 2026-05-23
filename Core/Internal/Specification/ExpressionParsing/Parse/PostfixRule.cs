using Xspec.Internal.Specification.ExpressionParsing.Tokenize;
using Xspec.Internal.Specification.ExpressionParsing.Expressions;

namespace Xspec.Internal.Specification.ExpressionParsing.Parse;

/// <summary>
/// Postfix loop: member access (<c>.</c> / <c>?.</c>), generic application,
/// invocation, indexing, <c>with { }</c> blocks, and the postfix operators
/// <c>++</c> / <c>--</c> / <c>!</c> (null-forgiving).
/// </summary>
internal static class PostfixRule
{
    public static Expr Parse(TokenStream ts)
    {
        int save = ts.Pos;
        var expr = PrimaryRule.Parse(ts);
        while (true)
        {
            if (TryParseMember(ts, save, ref expr)) continue;
            if (TryParseGeneric(ts, save, ref expr)) continue;
            if (TryParseInvocation(ts, save, ref expr, out bool callClosed)) { if (!callClosed) return expr; continue; }
            if (TryParseIndexing(ts, save, ref expr, out bool idxClosed)) { if (!idxClosed) return expr; continue; }
            if (TryParseWith(ts, save, ref expr, out bool withClosed)) { if (!withClosed) return expr; continue; }
            if (TryParsePostfixOp(ts, save, ref expr)) continue;
            break;
        }
        return expr;
    }

    private static bool TryParseMember(TokenStream ts, int save, ref Expr expr)
    {
        if (ts.Peek().Kind != TokenKind.Symbol || ts.Peek().Text is not ("." or "?.")) return false;
        bool nullConditional = ts.Peek().Text == "?.";
        ts.Advance();
        if (ts.Peek().Kind != TokenKind.Word) return false;
        string name = ts.Peek().Text;
        ts.Advance();
        expr = new Member(ts.RawFrom(save), expr, name, nullConditional);
        return true;
    }

    private static bool TryParseGeneric(TokenStream ts, int save, ref Expr expr)
    {
        if (!ts.IsSym("<") || !TypeRefRule.CanBeGenericApplication(ts, expr)) return false;
        if (!TypeRefRule.TryParseGenericArgs(ts, out var typeArgs)) return false;
        expr = new Generic(ts.RawFrom(save), expr, typeArgs);
        return true;
    }

    private static bool TryParseInvocation(TokenStream ts, int save, ref Expr expr, out bool closed)
        => TryParseBracketed(ts, save, "(", ")", (raw, target, args) => new Call(raw, target, args), ref expr, out closed);

    private static bool TryParseIndexing(TokenStream ts, int save, ref Expr expr, out bool closed)
        => TryParseBracketed(ts, save, "[", "]", (raw, target, args) => new IndexExpr(raw, target, args), ref expr, out closed);

    private static bool TryParseWith(TokenStream ts, int save, ref Expr expr, out bool closed)
    {
        closed = false;
        if (!ts.IsWord("with")) return false;
        int withSave = ts.Pos;
        ts.Advance();
        if (!ts.AcceptSym("{")) { ts.Pos = withSave; return false; }
        closed = ts.TryParse("}", out var inits);
        expr = new With(ts.RawFrom(save), expr, inits);
        return true;
    }

    private static bool TryParsePostfixOp(TokenStream ts, int save, ref Expr expr)
    {
        if (ts.Peek().Kind != TokenKind.Symbol || ts.Peek().Text is not ("++" or "--" or "!")) return false;
        string op = ts.Peek().Text;
        ts.Advance();
        expr = new Postfix(ts.RawFrom(save), op, expr);
        return true;
    }

    private static bool TryParseBracketed(TokenStream ts, int save, string open, string close,
        Func<string, Expr, IReadOnlyList<Expr>, Expr> build, ref Expr expr, out bool closed)
    {
        closed = false;
        if (!ts.AcceptSym(open)) return false;
        closed = ts.TryParse(close, out var args);
        expr = build(ts.RawFrom(save), expr, args);
        return true;
    }
}
