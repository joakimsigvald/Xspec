namespace Xspec.Internal.Specification.ExpressionParsing.Tokenize;

internal static class Tokenizer
{
    private static readonly string[] _multiCharOps =
    [
        "...",
        "=>", "==", "!=", "<=", ">=", "&&", "||", "??", "?.", "..",
        "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=",
        "++", "--",
    ];

    public static List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();
        int i = 0;
        while (i < input.Length)
        {
            if (char.IsWhiteSpace(input[i])) { i++; continue; }
            var token = ReadNext(input, i);
            tokens.Add(token);
            i = token.End;
        }
        tokens.Add(new Token(TokenKind.EndOfInput, "", input.Length, input.Length));
        return tokens;
    }

    /// Reads the next non-whitespace token starting at <paramref name="start"/>.
    /// Dispatches by what the first character can begin: identifier, number,
    /// string literal (with optional <c>@</c>/<c>$</c> prefix), char literal,
    /// or symbol/operator.
    private static Token ReadNext(string input, int start)
    {
        char c = input[start];
        if (char.IsLetter(c) || c == '_') return ReadWord(input, start);
        if (char.IsDigit(c)) return ReadNumber(input, start);
        if (TryReadString(input, start, out int strEnd))
            return new Token(TokenKind.String, input[start..strEnd], start, strEnd);
        if (c == '\'') return ReadChar(input, start);
        return ReadSymbolToken(input, start);
    }

    private static Token ReadWord(string input, int start)
    {
        int i = start;
        while (i < input.Length && (char.IsLetterOrDigit(input[i]) || input[i] == '_')) i++;
        return new Token(TokenKind.Word, input[start..i], start, i);
    }

    private static Token ReadNumber(string input, int start)
    {
        int i = start;
        while (i < input.Length && (char.IsLetterOrDigit(input[i]) || input[i] is '.' or '_')) i++;
        return new Token(TokenKind.Number, input[start..i], start, i);
    }

    private static Token ReadChar(string input, int start)
    {
        int i = start + 1;
        while (i < input.Length && input[i] != '\'')
        {
            if (input[i] == '\\' && i + 1 < input.Length) i++;
            i++;
        }
        if (i < input.Length) i++;
        return new Token(TokenKind.Char, input[start..i], start, i);
    }

    private static Token ReadSymbolToken(string input, int start)
    {
        string sym = ReadSymbol(input, start);
        return new Token(TokenKind.Symbol, sym, start, start + sym.Length);
    }

    private static bool TryReadString(string input, int start, out int end)
    {
        var open = ReadStringOpen(input, start);
        if (open.ContentStart < 0) { end = start; return false; }
        end = SkipStringContent(input, open.ContentStart, open.Verbatim, open.Interpolated);
        return true;
    }

    /// Reads any <c>$</c>/<c>@</c> prefix followed by the opening <c>"</c>.
    /// Returns the position just after the opening quote (or -1 if no string).
    private static (int ContentStart, bool Verbatim, bool Interpolated) ReadStringOpen(string input, int start)
    {
        int p = start;
        bool verbatim = false, interpolated = false;
        while (p < input.Length && input[p] is '$' or '@')
        {
            if (input[p] == '$') interpolated = true; else verbatim = true;
            p++;
        }
        return p < input.Length && input[p] == '"' ? (p + 1, verbatim, interpolated) : (-1, false, false);
    }

    /// Scans string body from <paramref name="from"/> to the closing <c>"</c>,
    /// respecting escape rules: <c>\</c> for non-verbatim, <c>""</c> for verbatim,
    /// and (if interpolated) skipping past balanced <c>{...}</c> holes.
    private static int SkipStringContent(string input, int from, bool verbatim, bool interpolated)
    {
        int p = from;
        while (p < input.Length)
        {
            char ch = input[p];
            if (interpolated && ch == '{')
            {
                if (IsDoubled(input, p, '{')) { p += 2; continue; }
                p = SkipInterpolationHole(input, p + 1);
                continue;
            }
            if (interpolated && ch == '}' && IsDoubled(input, p, '}')) { p += 2; continue; }
            if (!verbatim && ch == '\\' && p + 1 < input.Length) { p += 2; continue; }
            if (verbatim && ch == '"' && IsDoubled(input, p, '"')) { p += 2; continue; }
            if (ch == '"') return p + 1;
            p++;
        }
        return p;
    }

    /// Walks past the matching <c>}</c> for a <c>{</c> we've already consumed.
    /// Tracks brace depth so nested holes don't fool the scanner.
    private static int SkipInterpolationHole(string input, int from)
    {
        int p = from, depth = 1;
        while (p < input.Length && depth > 0)
        {
            if (input[p] == '{') depth++;
            else if (input[p] == '}') depth--;
            p++;
        }
        return p;
    }

    private static bool IsDoubled(string input, int p, char ch)
        => p + 1 < input.Length && input[p + 1] == ch;

    private static string ReadSymbol(string input, int i)
    {
        foreach (var op in _multiCharOps)
            if (i + op.Length <= input.Length && input.AsSpan(i, op.Length).SequenceEqual(op))
                return op;
        return input[i].ToString();
    }
}