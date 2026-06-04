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
        int strEnd = LiteralScanner.TryFindStringEnd(input, start);
        if (strEnd >= 0) return new Token(TokenKind.String, input[start..strEnd], start, strEnd);
        int charEnd = LiteralScanner.TryFindCharEnd(input, start);
        if (charEnd >= 0) return new Token(TokenKind.Char, input[start..charEnd], start, charEnd);
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

    private static Token ReadSymbolToken(string input, int start)
    {
        string sym = ReadSymbol(input, start);
        return new Token(TokenKind.Symbol, sym, start, start + sym.Length);
    }

    private static string ReadSymbol(string input, int i)
    {
        foreach (var op in _multiCharOps)
            if (i + op.Length <= input.Length && input.AsSpan(i, op.Length).SequenceEqual(op))
                return op;
        return input[i].ToString();
    }
}
