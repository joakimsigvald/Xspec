namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Describes an expression in "call" context (used by <c>ParseCall</c>).
/// Recognizes the call/assignment shapes a lambda body typically takes
/// (<c>_ =&gt; _.Method(args)</c>, <c>_ =&gt; _.X = value</c>, etc.) and falls
/// back to <see cref="ValueDescriber"/> for shapes that don't match.
/// <paramref name="skipSubjectRef"/> drops the leading <c>_.</c> for callers
/// (e.g. mock setup) that prepend the receiver name themselves.
/// </summary>
internal static class CallDescriber
{
    public static string Describe(Expr expr, bool skipSubjectRef = false)
    {
        if (expr is Lambda l) return DescribeLambda(l, skipSubjectRef);
        return expr switch
        {
            New n => NewDescriber.Describe(n),
            _ when MentionDescriber.TryDescribe(expr, out var m) => m,
            Call c => ShapeRenderer.DescribeCallShape(c),
            _ => ValueDescriber.Describe(expr),
        };
    }

    private static string DescribeLambda(Lambda l, bool skipSubjectRef)
    {
        // 1-param `_ => _.X(args)` / `x => x.X(args)`
        if (l.Params.Count == 1 && l.Body is Call mc && mc.Target is Member mem
            && DescribeHelpers.IsParamRef(l.Params, mem.Target))
        {
            var prefix = skipSubjectRef ? "" : $"{((Identifier)mem.Target).Name}.";
            return $"{prefix}{mem.Name}({DescribeHelpers.JoinDescribed(mc.Args)})";
        }
        // 1-param `_ => _.X = value` (any compound-assignment op)
        if (l.Params.Count == 1 && l.Body is Assign asgn && asgn.Target is Member assMem
            && DescribeHelpers.IsParamRef(l.Params, assMem.Target))
        {
            var prefix = skipSubjectRef ? "" : $"{((Identifier)assMem.Target).Name}.";
            return $"{prefix}{assMem.Name} {asgn.Op} {ValueDescriber.Describe(asgn.Value)}";
        }
        // 1-param `_ => _.<unparseable>` — strip `_.` from raw
        if (l.Params.Count == 1 && skipSubjectRef && l.Body is Unknown u
            && u.Raw.StartsWith(l.Params[0] + "."))
            return u.Raw[(l.Params[0].Length + 1)..];
        // multi-param `(x, i) => x.X = value` — strip params and subject
        if (l.Params.Count >= 2
            && DescribeHelpers.IsParamRefAssignment(l, out var prop, out var value, out var op)
            && op == "=")
            return $"{prop} = {ValueDescriber.Describe(value)}";
        return l.Params.Count == 1 ? ValueDescriber.Describe(l.Body) : l.Raw;
    }
}
