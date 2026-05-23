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
            char c = input[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (char.IsLetter(c) || c == '_')
            {
                int start = i;
                while (i < input.Length && (char.IsLetterOrDigit(input[i]) || input[i] == '_'))
                    i++;
                tokens.Add(new Token(TokenKind.Word, input[start..i], start, i));
                continue;
            }

            if (char.IsDigit(c))
            {
                int start = i;
                while (i < input.Length && (char.IsLetterOrDigit(input[i]) || input[i] == '.' || input[i] == '_'))
                    i++;
                tokens.Add(new Token(TokenKind.Number, input[start..i], start, i));
                continue;
            }

            if (TryReadString(input, i, out int strEnd))
            {
                tokens.Add(new Token(TokenKind.String, input[i..strEnd], i, strEnd));
                i = strEnd;
                continue;
            }

            if (c == '\'')
            {
                int start = i++;
                while (i < input.Length && input[i] != '\'')
                {
                    if (input[i] == '\\' && i + 1 < input.Length) i++;
                    i++;
                }
                if (i < input.Length) i++;
                tokens.Add(new Token(TokenKind.Char, input[start..i], start, i));
                continue;
            }

            int symStart = i;
            string sym = ReadSymbol(input, i);
            i += sym.Length;
            tokens.Add(new Token(TokenKind.Symbol, sym, symStart, i));
        }

        tokens.Add(new Token(TokenKind.EndOfInput, "", input.Length, input.Length));
        return tokens;
    }

    private static bool TryReadString(string input, int start, out int end)
    {
        end = start;
        int p = start;
        bool interpolated = false;
        bool verbatim = false;
        while (p < input.Length && (input[p] == '$' || input[p] == '@'))
        {
            if (input[p] == '$') interpolated = true;
            else verbatim = true;
            p++;
        }
        if (p >= input.Length || input[p] != '"')
            return false;
        p++;
        int braceDepth = 0;
        while (p < input.Length)
        {
            char ch = input[p];
            if (interpolated && ch == '{')
            {
                if (p + 1 < input.Length && input[p + 1] == '{') { p += 2; continue; }
                braceDepth++; p++; continue;
            }
            if (interpolated && ch == '}')
            {
                if (braceDepth > 0) { braceDepth--; p++; continue; }
                if (p + 1 < input.Length && input[p + 1] == '}') { p += 2; continue; }
                p++; continue;
            }
            if (braceDepth > 0) { p++; continue; }
            if (!verbatim && ch == '\\' && p + 1 < input.Length) { p += 2; continue; }
            if (verbatim && ch == '"' && p + 1 < input.Length && input[p + 1] == '"') { p += 2; continue; }
            if (ch == '"') { p++; end = p; return true; }
            p++;
        }
        end = p;
        return true;
    }

    private static string ReadSymbol(string input, int i)
    {
        foreach (var op in _multiCharOps)
            if (i + op.Length <= input.Length && input.AsSpan(i, op.Length).SequenceEqual(op))
                return op;
        return input[i].ToString();
    }
}