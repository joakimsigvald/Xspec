using Xspec.Internal.Specification.ExpressionParsing.Tokenize;
using Xspec.Internal.Specification.ExpressionParsing.Expressions;

namespace Xspec.Internal.Specification.ExpressionParsing.Parse;

/// <summary>
/// <c>new TypeName(...args)</c>, <c>new TypeName { ...init }</c>,
/// <c>new TypeName(...args) { ...init }</c>, <c>new int[] { ... }</c>,
/// and target-typed <c>new(...args)</c>.
/// </summary>
internal static class NewExprRule
{
    public static Expr Parse(TokenStream ts)
    {
        int save = ts.Pos;
        ts.Advance();                                       // consume 'new'

        string? typeName = null;
        if (ts.Peek().Kind == TokenKind.Word && ts.Peek().Text != "with")
        {
            int nameStart = ts.Pos;
            ts.Advance();
            while (ts.AcceptSym("."))
            {
                if (ts.Peek().Kind != TokenKind.Word) break;
                ts.Advance();
            }
            if (ts.IsSym("<")) TypeRefRule.TryParseGenericArgs(ts, out _);
            typeName = ts.Source[ts.TokenStart(nameStart)..ts.TokenEnd(ts.Pos - 1)];
        }

        IReadOnlyList<Expr> args = [];
        IReadOnlyList<Expr>? init = null;

        if (ts.AcceptSym("("))
        {
            if (!ts.TryParse(")", out args)) return new Unknown(ts.RawFrom(save));
        }
        else if (ts.AcceptSym("["))
        {
            if (!ts.TryParse("]", out args)) return new Unknown(ts.RawFrom(save));
        }
        if (ts.AcceptSym("{"))
        {
            if (!ts.TryParse("}", out var initList)) return new Unknown(ts.RawFrom(save));
            init = initList;
        }
        return new New(ts.RawFrom(save), typeName, args, init);
    }
}