namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Base for the parsed-expression tree. Each node knows its own
/// <see cref="Children"/> (for traversal) and how to render itself
/// <see cref="AsPath"/> (for dotted/generic path text). Domain-specific
/// rendering (mention lowering, lambda stripping, etc.) lives on the
/// <see cref="Describer"/> family.
/// </summary>
internal abstract record Expr(string Raw)
{
    public virtual IEnumerable<Expr> Children => [];
    public virtual string AsPath() => Raw;

    /// Strip a leading <c>$</c>/<c>@</c> prefix and re-emit the quoted contents.
    protected static string Requote(string raw)
    {
        int q = raw.IndexOf('"');
        return q < 0 || raw.Length < 2 || raw[^1] != '"' ? raw : $"\"{raw[(q + 1)..^1]}\"";
    }
}

internal sealed record Identifier(string Raw, string Name) : Expr(Raw)
    { public override string AsPath() => Name; }

internal sealed record Literal(string Raw) : Expr(Raw)
    { public string Quoted => Requote(Raw); }

internal sealed record InterpolatedString(string Raw) : Expr(Raw)
    { public string Quoted => Requote(Raw); }

internal sealed record Unknown(string Raw) : Expr(Raw);

internal sealed record Member(string Raw, Expr Target, string Name) : Expr(Raw)
{
    public override IEnumerable<Expr> Children => [Target];
    public override string AsPath() => $"{Target.AsPath()}.{Name}";
}

internal sealed record Generic(string Raw, Expr Target, IReadOnlyList<Expr> TypeArgs) : Expr(Raw)
{
    public override IEnumerable<Expr> Children => TypeArgs.Prepend(Target);
    public override string AsPath() =>
        $"{Target.AsPath()}<{string.Join(", ", TypeArgs.Select(t => t.Raw))}>";
}

internal sealed record Call(string Raw, Expr Target, IReadOnlyList<Expr> Args) : Expr(Raw)
{
    public override IEnumerable<Expr> Children => Args.Prepend(Target);

    /// The method/factory name being invoked, or null for non-named targets.
    public string? MethodName => Target switch
    {
        Identifier id => id.Name,
        Member m => m.Name,
        Generic { Target: Identifier gi } => gi.Name,
        Generic { Target: Member gm } => gm.Name,
        _ => null,
    };
}

internal sealed record Index(string Raw, Expr Target, IReadOnlyList<Expr> Args) : Expr(Raw)
    { public override IEnumerable<Expr> Children => Args.Prepend(Target); }

internal sealed record New(string Raw, string? TypeName, IReadOnlyList<Expr> Args, IReadOnlyList<Expr>? Init) : Expr(Raw)
    { public override IEnumerable<Expr> Children => Init is null ? Args : Args.Concat(Init); }

internal sealed record With(string Raw, Expr Target, IReadOnlyList<Expr> Init) : Expr(Raw)
    { public override IEnumerable<Expr> Children => Init.Prepend(Target); }

internal sealed record Assign(string Raw, string Op, Expr Target, Expr Value) : Expr(Raw)
    { public override IEnumerable<Expr> Children => [Target, Value]; }

internal sealed record Binary(string Raw, string Op, Expr Left, Expr Right) : Expr(Raw)
    { public override IEnumerable<Expr> Children => [Left, Right]; }

internal sealed record Unary(string Raw, string Op, Expr Operand) : Expr(Raw)
    { public override IEnumerable<Expr> Children => [Operand]; }

internal sealed record Postfix(string Raw, string Op, Expr Operand) : Expr(Raw)
    { public override IEnumerable<Expr> Children => [Operand]; }

internal sealed record Conditional(string Raw, Expr Cond, Expr Then, Expr Else) : Expr(Raw)
    { public override IEnumerable<Expr> Children => [Cond, Then, Else]; }

internal sealed record Cast(string Raw, string TypeName, Expr Operand) : Expr(Raw)
    { public override IEnumerable<Expr> Children => [Operand]; }

internal sealed record IsAs(string Raw, string Op, Expr Operand, string TypeName) : Expr(Raw)
    { public override IEnumerable<Expr> Children => [Operand]; }

internal sealed record Tuple(string Raw, IReadOnlyList<Expr> Items) : Expr(Raw)
    { public override IEnumerable<Expr> Children => Items; }

internal sealed record ArrayLit(string Raw, IReadOnlyList<Expr> Items) : Expr(Raw)
    { public override IEnumerable<Expr> Children => Items; }

internal sealed record Lambda(string Raw, IReadOnlyList<string> Params, Expr Body) : Expr(Raw)
{
    public override IEnumerable<Expr> Children => [Body];

    /// True for <c>p =&gt; p.Prop = value</c> (any number of params, any
    /// compound-assignment op). The first parameter must match the receiver
    /// of the member access (with <c>_</c> as a wildcard).
    public bool IsParamRefAssignment(out Member target, out Expr value, out string op)
    {
        target = null!; value = Body; op = string.Empty;
        if (Body is not Assign a || a.Target is not Member m || !IsParamRef(m.Target)) return false;
        target = m; value = a.Value; op = a.Op;
        return true;
    }

    /// True for <c>p =&gt; p.Method(args)</c>.
    public bool IsParamRefCall(out Member member, out IReadOnlyList<Expr> args)
    {
        member = null!; args = [];
        if (Body is not Call c || c.Target is not Member m || !IsParamRef(m.Target)) return false;
        member = m; args = c.Args;
        return true;
    }

    private bool IsParamRef(Expr e) =>
        e is Identifier id && Params.Count > 0 && (id.Name == Params[0] || Params[0] == "_");
}
