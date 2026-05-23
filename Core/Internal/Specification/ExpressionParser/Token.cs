namespace Xspec.Internal.Specification.ExpressionParserInternals;

internal enum TokenKind
{
    Word,
    Number,
    String,
    Char,
    Symbol,
    EndOfInput,
}

internal readonly record struct Token(TokenKind Kind, string Text, int Start, int End);
