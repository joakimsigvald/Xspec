namespace Xspec.Internal.Specification.ExpressionParserInternals;

internal sealed class Parser
{
    private readonly string _source;
    private readonly List<Token> _tokens;
    private int _pos;

    private Parser(string source, List<Token> tokens) { _source = source; _tokens = tokens; }

    public static Expr Parse(string source)
    {
        if (string.IsNullOrEmpty(source)) return new Unknown(source ?? string.Empty);
        var parser = new Parser(source, Tokenizer.Tokenize(source));
        try
        {
            var expr = parser.ParseExpression();
            return parser.Peek().Kind == TokenKind.EndOfInput ? expr : new Unknown(source.Trim());
        }
        catch { return new Unknown(source.Trim()); }
    }

    // --- token helpers ---

    private Token Peek(int offset = 0) => _tokens[Math.Min(_pos + offset, _tokens.Count - 1)];
    private void Advance() => _pos++;
    private bool IsSym(string s) => Peek().Kind == TokenKind.Symbol && Peek().Text == s;
    private bool IsWord(string s) => Peek().Kind == TokenKind.Word && Peek().Text == s;
    private bool AcceptSym(string s) { if (!IsSym(s)) return false; Advance(); return true; }

    private string RawFrom(int startTok)
    {
        if (startTok >= _tokens.Count) return string.Empty;
        int start = _tokens[startTok].Start;
        int end = _pos > 0 ? _tokens[_pos - 1].End : start;
        return end > start ? _source[start..end] : string.Empty;
    }

    /// <summary>
    /// Walks tokens with bracket-depth tracking across (), [], &lt;&gt;, {}.
    /// Stops when <paramref name="stopAtZero"/> returns true at depth 0, or at EOF.
    /// Returns the source text covered. Advances _pos to the stop token.
    /// </summary>
    private string ScanBalanced(Func<Token, bool> stopAtZero)
    {
        int startTok = _pos;
        int depth = 0;
        while (Peek().Kind != TokenKind.EndOfInput)
        {
            var t = Peek();
            if (depth == 0 && stopAtZero(t)) break;
            if (t.Kind == TokenKind.Symbol)
            {
                if (t.Text is "(" or "[" or "<" or "{") depth++;
                else if (t.Text is ")" or "]" or ">" or "}") depth--;
            }
            Advance();
        }
        if (startTok >= _tokens.Count) return string.Empty;
        int s = _tokens[startTok].Start;
        int e = _pos > startTok ? _tokens[_pos - 1].End : s;
        return e > s ? _source[s..e] : string.Empty;
    }

    /// <summary>
    /// Parses comma-separated expressions until <paramref name="terminator"/> is seen.
    /// Returns true if the terminator was consumed; false if missing.
    /// </summary>
    private bool ParseList(string terminator, out IReadOnlyList<Expr> items)
    {
        var list = new List<Expr>();
        if (!IsSym(terminator))
        {
            while (true)
            {
                list.Add(ParseExpression());
                if (!AcceptSym(",")) break;
            }
        }
        items = list;
        return AcceptSym(terminator);
    }

    // --- expression entry ---

    private Expr ParseExpression() => ParseLambda();

    private Expr ParseLambda()
    {
        int save = _pos;
        if (!TryParseLambdaParams(out var ps) || !AcceptSym("=>"))
        {
            _pos = save;
            return ParseAssignment();
        }
        int bodyTok = _pos;
        int bodyStart = bodyTok < _tokens.Count ? _tokens[bodyTok].Start : _source.Length;
        int bodySave = _pos;
        var body = ParseExpression();
        if (Peek().Kind != TokenKind.EndOfInput && !IsLambdaBoundary(Peek()))
        {
            _pos = bodySave;
            ScanBalanced(IsLambdaBoundary);
            int bodyEnd = _pos < _tokens.Count ? _tokens[_pos].Start : _source.Length;
            body = new Unknown(_source[bodyStart..bodyEnd].Trim());
        }
        return new Lambda(RawFrom(save), ps, body);
    }

    private static bool IsLambdaBoundary(Token t)
        => t.Kind == TokenKind.Symbol && t.Text is "," or ")" or "]" or "}";

    private bool TryParseLambdaParams(out IReadOnlyList<string> ps)
    {
        ps = [];
        if (Peek().Kind == TokenKind.Word && Peek(1).Text == "=>")
        {
            ps = [Peek().Text];
            Advance();
            return true;
        }
        if (!IsSym("(")) return false;
        int save = _pos;
        Advance();
        var list = new List<string>();
        while (!IsSym(")"))
        {
            if (Peek().Kind != TokenKind.Word) { _pos = save; return false; }
            list.Add(Peek().Text);
            Advance();
            if (!AcceptSym(",")) break;
        }
        if (!AcceptSym(")") || !IsSym("=>")) { _pos = save; return false; }
        ps = list;
        return true;
    }

    // --- assignment / conditional ---

    private Expr ParseAssignment()
    {
        int save = _pos;
        var left = ParseConditional();
        if (Peek().Kind == TokenKind.Symbol &&
            Peek().Text is "=" or "+=" or "-=" or "*=" or "/=" or "%=" or "&=" or "|=" or "^=")
        {
            string op = Peek().Text;
            Advance();
            return new Assign(RawFrom(save), op, left, ParseAssignment());
        }
        return left;
    }

    private Expr ParseConditional()
    {
        int save = _pos;
        var cond = ParseBinary(MinPrecedence);
        if (!AcceptSym("?")) return cond;
        var thenExpr = ParseExpression();
        if (!AcceptSym(":")) return new Unknown(_source.Trim());
        return new Conditional(RawFrom(save), cond, thenExpr, ParseExpression());
    }

    // --- binary precedence (Pratt) ---

    private const int MinPrecedence = 1;
    private const int RelationalPrecedence = 5;

    // (operator, precedence, right-associative)
    private static readonly (string Op, int Prec, bool RightAssoc)[] _binaryOps =
    [
        ("??", 1, true),
        ("||", 2, false), ("|", 2, false),
        ("&&", 3, false), ("&", 3, false),
        ("==", 4, false), ("!=", 4, false),
        ("<", 5, false), (">", 5, false), ("<=", 5, false), (">=", 5, false),
        ("+", 6, false), ("-", 6, false),
        ("*", 7, false), ("/", 7, false), ("%", 7, false),
    ];

    private Expr ParseBinary(int minPrec)
    {
        int save = _pos;
        var left = ParseUnary();
        while (true)
        {
            // `is` / `as` sit at relational precedence and take a type ref on the right.
            if ((IsWord("is") || IsWord("as")) && RelationalPrecedence >= minPrec)
            {
                string op = Peek().Text;
                Advance();
                left = new IsAs(RawFrom(save), op, left, ConsumeTypeRef());
                continue;
            }

            var matched = MatchBinaryOp(minPrec);
            if (matched is null) break;
            Advance();
            int nextMin = matched.Value.RightAssoc ? matched.Value.Prec : matched.Value.Prec + 1;
            left = new Binary(RawFrom(save), matched.Value.Op, left, ParseBinary(nextMin));
        }
        return left;
    }

    private (string Op, int Prec, bool RightAssoc)? MatchBinaryOp(int minPrec)
    {
        if (Peek().Kind != TokenKind.Symbol) return null;
        var text = Peek().Text;
        foreach (var op in _binaryOps)
            if (op.Op == text && op.Prec >= minPrec) return op;
        return null;
    }

    // --- unary / cast ---

    private Expr ParseUnary()
    {
        int save = _pos;
        if (Peek().Kind == TokenKind.Symbol && Peek().Text is "!" or "-" or "+" or "~" or "++" or "--")
        {
            string op = Peek().Text;
            Advance();
            return new Unary(RawFrom(save), op, ParseUnary());
        }
        if (IsSym("(") && LooksLikeCast())
        {
            int castSave = _pos;
            Advance();
            string typeName = ConsumeTypeRef();
            if (AcceptSym(")")) return new Cast(RawFrom(castSave), typeName, ParseUnary());
            _pos = castSave;
        }
        return ParsePostfix();
    }

    private bool LooksLikeCast()
    {
        int save = _pos;
        try
        {
            Advance();                                  // consume '('
            if (Peek().Kind != TokenKind.Word) return false;
            ScanBalanced(t => t.Kind == TokenKind.Symbol && (t.Text is ")" or ","));
            if (!IsSym(")")) return false;
            Advance();                                  // consume ')'
            var nxt = Peek();
            return nxt.Kind is TokenKind.Word or TokenKind.Number
                || (nxt.Kind == TokenKind.Symbol && nxt.Text is "(" or "-" or "!" or "~");
        }
        finally { _pos = save; }
    }

    // --- postfix ---

    private Expr ParsePostfix()
    {
        int save = _pos;
        var expr = ParsePrimary();
        while (true)
        {
            if (Peek().Kind == TokenKind.Symbol && Peek().Text is "." or "?.")
            {
                string dot = Peek().Text;
                Advance();
                if (Peek().Kind != TokenKind.Word) break;
                string name = (dot == "?." ? "?." : "") + Peek().Text;
                Advance();
                expr = new Member(RawFrom(save), expr, name);
                continue;
            }
            if (IsSym("<") && CanBeGenericApplication(expr) && TryParseGenericArgs(out var typeArgs))
            {
                expr = new Generic(RawFrom(save), expr, typeArgs);
                continue;
            }
            if (AcceptSym("("))
            {
                bool closed = ParseList(")", out var args);
                expr = new Call(RawFrom(save), expr, args);
                if (!closed) return expr;
                continue;
            }
            if (AcceptSym("["))
            {
                bool closed = ParseList("]", out var args);
                expr = new Index(RawFrom(save), expr, args);
                if (!closed) return expr;
                continue;
            }
            if (IsWord("with"))
            {
                Advance();
                if (!AcceptSym("{")) break;
                bool closed = ParseList("}", out var inits);
                expr = new With(RawFrom(save), expr, inits);
                if (!closed) return expr;
                continue;
            }
            if (Peek().Kind == TokenKind.Symbol && Peek().Text is "++" or "--" or "!")
            {
                string op = Peek().Text;
                Advance();
                expr = new Postfix(RawFrom(save), op, expr);
                continue;
            }
            break;
        }
        return expr;
    }

    // --- generic / type-ref ---

    private bool CanBeGenericApplication(Expr target)
    {
        if (target is not Identifier and not Member) return false;
        int save = _pos;
        try
        {
            if (!AcceptSym("<")) return false;
            ScanBalanced(t => t.Kind == TokenKind.Symbol &&
                (t.Text is ">" or ";" or "{" or "}"));
            if (!IsSym(">")) return false;
            Advance();
            var follow = Peek();
            if (follow.Kind == TokenKind.EndOfInput) return true;
            return follow.Kind == TokenKind.Symbol
                && follow.Text is "(" or "." or "," or ")" or "]" or ";" or "?" or ":" or "==" or "!=";
        }
        finally { _pos = save; }
    }

    private bool TryParseGenericArgs(out IReadOnlyList<Expr> typeArgs)
    {
        int save = _pos;
        if (!AcceptSym("<")) { typeArgs = []; return false; }
        var list = new List<Expr>();
        if (!IsSym(">"))
        {
            while (true)
            {
                var typeText = ConsumeTypeRef();
                list.Add(new Identifier(typeText, typeText));
                if (!AcceptSym(",")) break;
            }
        }
        if (!AcceptSym(">")) { _pos = save; typeArgs = []; return false; }
        typeArgs = list;
        return true;
    }

    private string ConsumeTypeRef()
        => ScanBalanced(t => t.Kind == TokenKind.Symbol &&
            (t.Text is ")" or "]" or ">" || t.Text == ","));

    // --- primary ---

    private Expr ParsePrimary()
    {
        int save = _pos;
        var t = Peek();

        if (t.Kind == TokenKind.Word)
        {
            if (t.Text == "new") return ParseNew();
            Advance();
            if (t.Text is "true" or "false" or "null" or "default") return new Literal(t.Text);
            return new Identifier(t.Text, t.Text);
        }

        if (t.Kind is TokenKind.Number or TokenKind.Char)
        {
            Advance();
            return new Literal(t.Text);
        }

        if (t.Kind == TokenKind.String)
        {
            Advance();
            bool interpolated = t.Text.StartsWith('$')
                || (t.Text.Length > 1 && t.Text[0] is '@' or '$' && t.Text[1] == '$');
            return interpolated ? new InterpolatedString(t.Text) : new Literal(t.Text);
        }

        if (t.Kind == TokenKind.Symbol)
        {
            if (t.Text == "(") return ParseParenOrTuple(save);
            if (t.Text == "[")
            {
                Advance();
                if (!ParseList("]", out var items)) return new Unknown(_source.Trim());
                return new ArrayLit(RawFrom(save), items);
            }
            if (t.Text is "-" or "+" or "!" or "~") return ParseUnary();
        }

        Advance();
        return new Unknown(t.Text);
    }

    private Expr ParseParenOrTuple(int save)
    {
        Advance();                                  // consume '('
        if (AcceptSym(")")) return new Tuple(RawFrom(save), []);
        var first = ParseExpression();
        if (!AcceptSym(","))
        {
            if (!AcceptSym(")")) return new Unknown(_source.Trim());
            return first;
        }
        var items = new List<Expr> { first };
        while (true)
        {
            items.Add(ParseExpression());
            if (!AcceptSym(",")) break;
        }
        if (!AcceptSym(")")) return new Unknown(_source.Trim());
        return new Tuple(RawFrom(save), items);
    }

    private Expr ParseNew()
    {
        int save = _pos;
        Advance();                                  // consume 'new'

        string? typeName = null;
        if (Peek().Kind == TokenKind.Word && Peek().Text != "with")
        {
            int nameStart = _pos;
            Advance();
            while (AcceptSym("."))
            {
                if (Peek().Kind != TokenKind.Word) break;
                Advance();
            }
            if (IsSym("<")) TryParseGenericArgs(out _);
            typeName = _source[_tokens[nameStart].Start.._tokens[_pos - 1].End];
        }

        IReadOnlyList<Expr> args = [];
        IReadOnlyList<Expr>? init = null;

        if (AcceptSym("("))
        {
            if (!ParseList(")", out args)) return new Unknown(_source.Trim());
        }
        else if (AcceptSym("["))
        {
            if (!ParseList("]", out args)) return new Unknown(_source.Trim());
        }
        if (AcceptSym("{"))
        {
            if (!ParseList("}", out var initList)) return new Unknown(_source.Trim());
            init = initList;
        }

        return new New(RawFrom(save), typeName, args, init);
    }
}
