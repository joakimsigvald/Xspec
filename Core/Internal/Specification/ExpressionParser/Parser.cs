namespace Xspec.Internal.Specification.ExpressionParserInternals;

internal sealed class Parser
{
    private readonly string _source;
    private readonly List<Token> _tokens;
    private int _pos;

    private Parser(string source, List<Token> tokens)
    {
        _source = source;
        _tokens = tokens;
    }

    public static Expr Parse(string source)
    {
        if (string.IsNullOrEmpty(source))
            return new Unknown(source ?? string.Empty);
        var tokens = Tokenizer.Tokenize(source);
        var parser = new Parser(source, tokens);
        try
        {
            var expr = parser.ParseExpression();
            if (parser.Peek().Kind != TokenKind.EndOfInput)
                return new Unknown(source.Trim());
            return expr;
        }
        catch
        {
            return new Unknown(source.Trim());
        }
    }

    private Token Peek(int offset = 0) => _tokens[Math.Min(_pos + offset, _tokens.Count - 1)];

    private Token Advance() => _tokens[_pos++];

    private bool IsSym(string s) => Peek().Kind == TokenKind.Symbol && Peek().Text == s;
    private bool IsWord(string s) => Peek().Kind == TokenKind.Word && Peek().Text == s;

    private bool AcceptSym(string s)
    {
        if (IsSym(s)) { Advance(); return true; }
        return false;
    }

    private string RawFrom(int startTokenIdx)
    {
        if (startTokenIdx >= _tokens.Count) return string.Empty;
        int start = _tokens[startTokenIdx].Start;
        int end = _pos > 0 ? _tokens[_pos - 1].End : start;
        if (end <= start) return string.Empty;
        return _source[start..end];
    }

    private Expr ParseExpression() => ParseLambda();

    private Expr ParseLambda()
    {
        int save = _pos;
        if (TryParseLambdaParams(out var paramList))
        {
            if (AcceptSym("=>"))
            {
                int bodyStartTok = _pos;
                int bodyStartChar = bodyStartTok < _tokens.Count
                    ? _tokens[bodyStartTok].Start
                    : _source.Length;
                int bodyStartSave = _pos;
                var body = ParseExpression();
                if (Peek().Kind != TokenKind.EndOfInput && !IsLambdaBoundary(Peek()))
                {
                    _pos = bodyStartSave;
                    int depth = 0;
                    while (Peek().Kind != TokenKind.EndOfInput)
                    {
                        var t = Peek();
                        if (depth == 0 && IsLambdaBoundary(t)) break;
                        if (t.Kind == TokenKind.Symbol)
                        {
                            if (t.Text is "(" or "[" or "{") depth++;
                            else if (t.Text is ")" or "]" or "}") depth--;
                        }
                        Advance();
                    }
                    int bodyEndChar = _pos < _tokens.Count
                        ? _tokens[_pos].Start
                        : _source.Length;
                    body = new Unknown(_source[bodyStartChar..bodyEndChar].Trim());
                }
                return new Lambda(RawFrom(save), paramList, body);
            }
            _pos = save;
        }
        return ParseAssignment();
    }

    private static bool IsLambdaBoundary(Token t)
        => t.Kind == TokenKind.Symbol && t.Text is "," or ")" or "]" or "}";

    private bool TryParseLambdaParams(out IReadOnlyList<string> paramList)
    {
        paramList = [];
        if (Peek().Kind == TokenKind.Word && Peek(1).Kind == TokenKind.Symbol && Peek(1).Text == "=>")
        {
            paramList = [Peek().Text];
            Advance();
            return true;
        }
        if (IsSym("("))
        {
            int save = _pos;
            Advance();
            var list = new List<string>();
            if (!IsSym(")"))
            {
                while (true)
                {
                    if (Peek().Kind != TokenKind.Word) { _pos = save; return false; }
                    list.Add(Peek().Text);
                    Advance();
                    if (AcceptSym(",")) continue;
                    break;
                }
            }
            if (!AcceptSym(")")) { _pos = save; return false; }
            if (!IsSym("=>")) { _pos = save; return false; }
            paramList = list;
            return true;
        }
        return false;
    }

    private Expr ParseAssignment()
    {
        int save = _pos;
        var left = ParseConditional();
        if (Peek().Kind == TokenKind.Symbol &&
            Peek().Text is "=" or "+=" or "-=" or "*=" or "/=" or "%=" or "&=" or "|=" or "^=")
        {
            string op = Peek().Text;
            Advance();
            var right = ParseAssignment();
            return new Assign(RawFrom(save), op, left, right);
        }
        return left;
    }

    private Expr ParseConditional()
    {
        int save = _pos;
        var cond = ParseNullCoalesce();
        if (AcceptSym("?"))
        {
            var thenExpr = ParseExpression();
            if (AcceptSym(":"))
            {
                var elseExpr = ParseExpression();
                return new Conditional(RawFrom(save), cond, thenExpr, elseExpr);
            }
            return new Unknown(_source.Trim());
        }
        return cond;
    }

    private Expr ParseNullCoalesce()
    {
        int save = _pos;
        var left = ParseLogicalOr();
        if (AcceptSym("??"))
        {
            var right = ParseNullCoalesce();
            return new Binary(RawFrom(save), "??", left, right);
        }
        return left;
    }

    private Expr ParseLogicalOr() => ParseLeftAssoc(ParseLogicalAnd, "||", "|");
    private Expr ParseLogicalAnd() => ParseLeftAssoc(ParseEquality, "&&", "&");
    private Expr ParseEquality() => ParseLeftAssoc(ParseRelational, "==", "!=");

    private Expr ParseRelational()
    {
        int save = _pos;
        var left = ParseAdditive();
        while (true)
        {
            if (IsWord("is") || IsWord("as"))
            {
                string op = Peek().Text;
                Advance();
                var typeName = ConsumeTypeRef();
                left = new IsAs(RawFrom(save), op, left, typeName);
                continue;
            }
            if (IsSym("<") || IsSym(">") || IsSym("<=") || IsSym(">="))
            {
                string op = Peek().Text;
                Advance();
                var right = ParseAdditive();
                left = new Binary(RawFrom(save), op, left, right);
                continue;
            }
            break;
        }
        return left;
    }

    private Expr ParseAdditive() => ParseLeftAssoc(ParseMultiplicative, "+", "-");
    private Expr ParseMultiplicative() => ParseLeftAssoc(ParseUnary, "*", "/", "%");

    private Expr ParseLeftAssoc(Func<Expr> next, params string[] ops)
    {
        int save = _pos;
        var left = next();
        while (true)
        {
            if (Peek().Kind != TokenKind.Symbol) break;
            string? matched = null;
            foreach (var op in ops)
                if (Peek().Text == op) { matched = op; break; }
            if (matched is null) break;
            Advance();
            var right = next();
            left = new Binary(RawFrom(save), matched, left, right);
        }
        return left;
    }

    private Expr ParseUnary()
    {
        int save = _pos;
        if (Peek().Kind == TokenKind.Symbol &&
            Peek().Text is "!" or "-" or "+" or "~" or "++" or "--")
        {
            string op = Peek().Text;
            Advance();
            var operand = ParseUnary();
            return new Unary(RawFrom(save), op, operand);
        }
        if (IsSym("(") && LooksLikeCast())
        {
            int castSave = _pos;
            Advance();
            string typeName = ConsumeTypeRef();
            if (AcceptSym(")"))
            {
                var operand = ParseUnary();
                return new Cast(RawFrom(castSave), typeName, operand);
            }
            _pos = castSave;
        }
        return ParsePostfix();
    }

    private bool LooksLikeCast()
    {
        int save = _pos;
        try
        {
            Advance();
            if (Peek().Kind != TokenKind.Word) return false;
            int depth = 0;
            int p = _pos;
            while (p < _tokens.Count)
            {
                var t = _tokens[p];
                if (t.Kind == TokenKind.Symbol)
                {
                    if (t.Text == "<" || t.Text == "(") depth++;
                    else if (t.Text == ">" || t.Text == ")")
                    {
                        if (depth == 0 && t.Text == ")")
                        {
                            if (p + 1 < _tokens.Count)
                            {
                                var nxt = _tokens[p + 1];
                                if (nxt.Kind == TokenKind.Word) return true;
                                if (nxt.Kind == TokenKind.Number) return true;
                                if (nxt.Kind == TokenKind.Symbol && (nxt.Text == "(" || nxt.Text == "-" || nxt.Text == "!" || nxt.Text == "~"))
                                    return true;
                            }
                            return false;
                        }
                        depth--;
                    }
                    else if (t.Text == "," && depth == 0) return false;
                }
                if (t.Kind == TokenKind.EndOfInput) return false;
                p++;
            }
            return false;
        }
        finally { _pos = save; }
    }

    private Expr ParsePostfix()
    {
        int save = _pos;
        var expr = ParsePrimary();
        while (true)
        {
            if (AcceptSym("."))
            {
                if (Peek().Kind != TokenKind.Word) break;
                string name = Peek().Text;
                Advance();
                expr = new Member(RawFrom(save), expr, name);
                continue;
            }
            if (AcceptSym("?."))
            {
                if (Peek().Kind != TokenKind.Word) break;
                string name = Peek().Text;
                Advance();
                expr = new Member(RawFrom(save), expr, "?." + name);
                continue;
            }
            if (IsSym("<") && CanBeGenericApplication(expr))
            {
                int genSave = _pos;
                if (TryParseGenericArgs(out var typeArgs))
                {
                    expr = new Generic(RawFrom(save), expr, typeArgs);
                    continue;
                }
                _pos = genSave;
            }
            if (AcceptSym("("))
            {
                var args = ParseExprList(")");
                if (!AcceptSym(")"))
                {
                    expr = new Call(RawFrom(save), expr, args);
                    return expr;
                }
                expr = new Call(RawFrom(save), expr, args);
                continue;
            }
            if (AcceptSym("["))
            {
                var args = ParseExprList("]");
                if (!AcceptSym("]"))
                {
                    expr = new Index(RawFrom(save), expr, args);
                    return expr;
                }
                expr = new Index(RawFrom(save), expr, args);
                continue;
            }
            if (IsWord("with"))
            {
                Advance();
                if (!AcceptSym("{")) break;
                var inits = ParseExprList("}");
                if (!AcceptSym("}"))
                {
                    expr = new With(RawFrom(save), expr, inits);
                    return expr;
                }
                expr = new With(RawFrom(save), expr, inits);
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

    private bool CanBeGenericApplication(Expr target)
    {
        if (target is not Identifier && target is not Member) return false;
        int save = _pos;
        try
        {
            if (!AcceptSym("<")) return false;
            int depth = 1;
            while (depth > 0 && Peek().Kind != TokenKind.EndOfInput)
            {
                var t = Peek();
                if (t.Kind == TokenKind.Symbol)
                {
                    if (t.Text == "<") depth++;
                    else if (t.Text == ">") depth--;
                    else if (t.Text == ">>" && depth >= 2) { depth -= 2; Advance(); continue; }
                    else if (t.Text == ";" || t.Text == "{" || t.Text == "}") return false;
                }
                Advance();
            }
            if (depth != 0) return false;
            var follow = Peek();
            if (follow.Kind == TokenKind.EndOfInput) return true;
            if (follow.Kind == TokenKind.Symbol)
                return follow.Text is "(" or "." or "," or ")" or "]" or ";" or "?" or ":" or "==" or "!=";
            return false;
        }
        finally { _pos = save; }
    }

    private bool TryParseGenericArgs(out IReadOnlyList<Expr> typeArgs)
    {
        typeArgs = [];
        int save = _pos;
        if (!AcceptSym("<")) return false;
        var list = new List<Expr>();
        if (!IsSym(">") && !IsSym(">>"))
        {
            while (true)
            {
                int argStart = _pos;
                var typeText = ConsumeTypeRef();
                list.Add(new Identifier(typeText, typeText));
                if (AcceptSym(",")) continue;
                break;
            }
        }
        if (AcceptSym(">"))
        {
            typeArgs = list;
            return true;
        }
        if (IsSym(">>"))
        {
            var tok = Peek();
            _tokens[_pos] = new Token(TokenKind.Symbol, ">", tok.Start + 1, tok.End);
            typeArgs = list;
            return true;
        }
        _pos = save;
        return false;
    }

    private string ConsumeTypeRef()
    {
        int startIdx = _pos;
        int depth = 0;
        while (Peek().Kind != TokenKind.EndOfInput)
        {
            var t = Peek();
            if (t.Kind == TokenKind.Symbol)
            {
                if (t.Text == "<" || t.Text == "(" || t.Text == "[") depth++;
                else if (t.Text == ">" || t.Text == ")" || t.Text == "]")
                {
                    if (depth == 0) break;
                    depth--;
                }
                else if (t.Text == ">>")
                {
                    if (depth <= 1) break;
                    depth -= 2;
                }
                else if (t.Text == "," && depth == 0) break;
            }
            Advance();
        }
        if (startIdx >= _tokens.Count) return string.Empty;
        int sStart = _tokens[startIdx].Start;
        int sEnd = _pos > startIdx ? _tokens[_pos - 1].End : sStart;
        return sEnd > sStart ? _source[sStart..sEnd] : string.Empty;
    }

    private IReadOnlyList<Expr> ParseExprList(string terminator)
    {
        var list = new List<Expr>();
        if (IsSym(terminator)) return list;
        while (true)
        {
            var item = ParseExpression();
            list.Add(item);
            if (!AcceptSym(",")) break;
        }
        return list;
    }

    private Expr ParsePrimary()
    {
        int save = _pos;
        var t = Peek();

        if (t.Kind == TokenKind.Word)
        {
            if (t.Text == "new") return ParseNew();
            if (t.Text == "true" || t.Text == "false" || t.Text == "null" || t.Text == "default")
            {
                Advance();
                return new Literal(t.Text);
            }
            Advance();
            return new Identifier(t.Text, t.Text);
        }

        if (t.Kind == TokenKind.Number || t.Kind == TokenKind.Char)
        {
            Advance();
            return new Literal(t.Text);
        }

        if (t.Kind == TokenKind.String)
        {
            Advance();
            return t.Text.StartsWith('$') || (t.Text.Length > 1 && (t.Text[0] is '@' or '$') && t.Text[1] == '$')
                ? new InterpolatedString(t.Text)
                : new Literal(t.Text);
        }

        if (t.Kind == TokenKind.Symbol)
        {
            if (t.Text == "(")
            {
                Advance();
                if (AcceptSym(")"))
                    return new Tuple(RawFrom(save), []);
                var first = ParseExpression();
                if (AcceptSym(","))
                {
                    var list = new List<Expr> { first };
                    while (true)
                    {
                        list.Add(ParseExpression());
                        if (!AcceptSym(",")) break;
                    }
                    if (!AcceptSym(")")) return new Unknown(_source.Trim());
                    return new Tuple(RawFrom(save), list);
                }
                if (!AcceptSym(")")) return new Unknown(_source.Trim());
                return first;
            }
            if (t.Text == "[")
            {
                Advance();
                var items = ParseExprList("]");
                if (!AcceptSym("]")) return new Unknown(_source.Trim());
                return new ArrayLit(RawFrom(save), items);
            }
            if (t.Text == "-" || t.Text == "+" || t.Text == "!" || t.Text == "~")
            {
                return ParseUnary();
            }
        }

        Advance();
        return new Unknown(t.Text);
    }

    private Expr ParseNew()
    {
        int save = _pos;
        Advance();

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
            if (IsSym("<"))
            {
                int nameWithGenSave = _pos;
                if (TryParseGenericArgs(out _))
                {
                }
                else _pos = nameWithGenSave;
            }
            int nameEndIdx = _pos;
            int nameStartChar = _tokens[nameStart].Start;
            int nameEndChar = _tokens[nameEndIdx - 1].End;
            typeName = _source[nameStartChar..nameEndChar];
        }

        IReadOnlyList<Expr> args = [];
        IReadOnlyList<Expr>? init = null;

        if (AcceptSym("("))
        {
            args = ParseExprList(")");
            if (!AcceptSym(")")) return new Unknown(_source.Trim());
        }
        if (AcceptSym("["))
        {
            args = ParseExprList("]");
            if (!AcceptSym("]")) return new Unknown(_source.Trim());
        }
        if (AcceptSym("{"))
        {
            init = ParseExprList("}");
            if (!AcceptSym("}")) return new Unknown(_source.Trim());
        }

        return new New(RawFrom(save), typeName, args, init);
    }
}
