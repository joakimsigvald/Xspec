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
            if (ts.Peek().Kind == TokenKind.Symbol && ts.Peek().Text is "." or "?.")
            {
                bool nullConditional = ts.Peek().Text == "?.";
                ts.Advance();
                if (ts.Peek().Kind != TokenKind.Word) 
                    break;

                string name = ts.Peek().Text;
                ts.Advance();
                expr = new Member(ts.RawFrom(save), expr, name, nullConditional);
                continue;
            }
            if (ts.IsSym("<") && TypeRefRule.CanBeGenericApplication(ts, expr)
                && TypeRefRule.TryParseGenericArgs(ts, out var typeArgs))
            {
                expr = new Generic(ts.RawFrom(save), expr, typeArgs);
                continue;
            }
            if (ts.AcceptSym("("))
            {
                bool closed = ts.TryParse(")", out var args);
                expr = new Call(ts.RawFrom(save), expr, args);
                if (!closed) 
                    return expr;

                continue;
            }
            if (ts.AcceptSym("["))
            {
                bool closed = ts.TryParse("]", out var args);
                expr = new IndexExpr(ts.RawFrom(save), expr, args);
                if (!closed) 
                    return expr;

                continue;
            }
            if (ts.IsWord("with"))
            {
                ts.Advance();
                if (!ts.AcceptSym("{")) break;
                bool closed = ts.TryParse("}", out var inits);
                expr = new With(ts.RawFrom(save), expr, inits);
                if (!closed) 
                    return expr;

                continue;
            }
            if (ts.Peek().Kind == TokenKind.Symbol && ts.Peek().Text is "++" or "--" or "!")
            {
                string op = ts.Peek().Text;
                ts.Advance();
                expr = new Postfix(ts.RawFrom(save), op, expr);
                continue;
            }
            break;
        }
        return expr;
    }
}