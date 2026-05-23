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

    /// If this expression (or its outer wrappers) contains a Mention factory
    /// — <c>A&lt;T&gt;</c> / <c>An&lt;T&gt;</c> / <c>The&lt;T&gt;</c> etc. —
    /// describe its root, verb, type args, and any constraints. Otherwise null.
    public virtual Mention? AsMention() => null;

    /// Strip a leading <c>$</c>/<c>@</c> prefix and re-emit the quoted contents.
    protected static string Requote(string raw)
    {
        int q = raw.IndexOf('"');
        return q < 0 || raw.Length < 2 || raw[^1] != '"' ? raw : $"\"{raw[(q + 1)..^1]}\"";
    }
}

/// A Mention factory occurrence in an expression tree.
/// <paramref name="Root"/> is the outermost call/generic that bounds it
/// (used by the describer to check for drilldown).
internal sealed record Mention(Expr Root, string Verb, string TypeArgs, IReadOnlyList<Expr>? Constraints);

/// <c>p =&gt; p.Prop = value</c> shape (any compound-assignment op).
internal sealed record ParamRefAssign(Identifier Receiver, Member Target, string Op, Expr Value);

/// <c>p =&gt; p.Method(args)</c> shape.
internal sealed record ParamRefCall(Identifier Receiver, Member Target, IReadOnlyList<Expr> Args);

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
    public override Mention? AsMention() => Target.AsMention();
}

internal sealed record Generic(string Raw, Expr Target, IReadOnlyList<Expr> TypeArgs) : Expr(Raw)
{
    public override IEnumerable<Expr> Children => TypeArgs.Prepend(Target);
    public override string AsPath() =>
        $"{Target.AsPath()}<{string.Join(", ", TypeArgs.Select(t => t.Raw))}>";

    public override Mention? AsMention() => Target is Identifier id && TypeArgs.Count > 0
        ? new Mention(this, id.Name, string.Join(", ", TypeArgs.Select(t => t.Raw)), null)
        : null;
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

    /// If this call directly wraps a Mention factory, its Args become the
    /// mention's constraints. Otherwise the inner mention is passed through.
    public override Mention? AsMention() => Target.AsMention() switch
    {
        null => null,
        var inner when Target is Generic && ReferenceEquals(inner.Root, Target)
            => inner with { Root = this, Constraints = Args.Count > 0 ? Args : null },
        var inner => inner,
    };
}

internal sealed record Index(string Raw, Expr Target, IReadOnlyList<Expr> Args) : Expr(Raw)
{
    public override IEnumerable<Expr> Children => Args.Prepend(Target);
    public override Mention? AsMention() => Target.AsMention();
}

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

    /// Match <c>p =&gt; p.Prop = value</c> (any compound-assignment op).
    /// The first parameter must match the receiver (with <c>_</c> as wildcard).
    public ParamRefAssign? AsParamRefAssign() =>
        Body is Assign { Target: Member { Target: Identifier rcv } m } a && IsParamRef(rcv)
            ? new ParamRefAssign(rcv, m, a.Op, a.Value)
            : null;

    /// Match <c>p =&gt; p.Method(args)</c>.
    public ParamRefCall? AsParamRefCall() =>
        Body is Call { Target: Member { Target: Identifier rcv } m } c && IsParamRef(rcv)
            ? new ParamRefCall(rcv, m, c.Args)
            : null;

    private bool IsParamRef(Identifier id) =>
        Params.Count > 0 && (id.Name == Params[0] || Params[0] == "_");
}
