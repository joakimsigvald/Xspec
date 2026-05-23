namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Call-mode description (used by <c>ParseCall</c>). Recognizes the
/// lambda-body shapes a mock setup or action expression typically takes
/// (<c>_ =&gt; _.Method(args)</c>, <c>_ =&gt; _.X = value</c>) and falls
/// back to value-mode for anything else. <see cref="_skipSubjectRef"/>
/// drops the leading <c>_.</c> when the caller (e.g. mock setup) prepends
/// the receiver name itself.
/// </summary>
internal sealed class CallDescriber : Describer
{
    private readonly bool _skipSubjectRef;

    public CallDescriber(bool skipSubjectRef) { _skipSubjectRef = skipSubjectRef; }

    public override string Describe(Expr expr)
    {
        if (expr is Lambda l) return DescribeLambda(l);
        return expr switch
        {
            New n => DescribeNew(n),
            _ when TryDescribeMention(expr, out var m) => m,
            Call c => $"{c.Target.AsPath()}({DescribeAll(c.Args)})",
            _ => Value.Describe(expr),
        };
    }

    private string DescribeLambda(Lambda l)
    {
        if (l.Params.Count == 1 && l.IsParamRefCall(out var mc, out var args))
            return Prefixed(mc.Target, mc.Name, $"({DescribeAll(args)})");

        if (l.Params.Count == 1 && l.IsParamRefAssignment(out var target, out var value, out var op))
            return Prefixed(target.Target, target.Name, $" {op} {Value.Describe(value)}");

        // skipSubjectRef rescue: body failed to parse but starts with "param.".
        if (l.Params.Count == 1 && _skipSubjectRef && l.Body is Unknown u
            && u.Raw.StartsWith(l.Params[0] + "."))
            return u.Raw[(l.Params[0].Length + 1)..];

        if (l.Params.Count >= 2
            && l.IsParamRefAssignment(out var t2, out var v2, out var op2) && op2 == "=")
            return $"{t2.Name} = {Value.Describe(v2)}";

        return l.Params.Count == 1 ? Value.Describe(l.Body) : l.Raw;
    }

    private string Prefixed(Expr receiver, string memberName, string suffix)
    {
        var prefix = _skipSubjectRef ? "" : $"{((Identifier)receiver).Name}.";
        return $"{prefix}{memberName}{suffix}";
    }
}
