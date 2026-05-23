using Xspec.Internal.Specification.ExpressionParsing.Tokenize;
using Xspec.Internal.Specification.ExpressionParsing.Expressions;

namespace Xspec.Internal.Specification.ExpressionParsing.Parse;

/// <summary>
/// Comma-separated expression list bounded by a terminator. Implemented as
/// an extension on <see cref="TokenStream"/> so call sites stay terse, while
/// the dependency on <see cref="LambdaRule"/> (and through it the full
/// expression grammar) lives in the Parse namespace, not in Tokenize.
/// </summary>
internal static class ParseList
{
    /// <summary>
    /// Parses comma-separated expressions until <paramref name="terminator"/>.
    /// Returns true if the terminator was consumed; false if missing.
    /// </summary>
    public static bool TryParse(this TokenStream ts, string terminator, out IReadOnlyList<Expr> items)
    {
        var list = new List<Expr>();
        if (!ts.IsSym(terminator))
        {
            while (true)
            {
                list.Add(LambdaRule.Parse(ts));
                if (!ts.AcceptSym(",")) break;
            }
        }
        items = list;
        return ts.AcceptSym(terminator);
    }
}