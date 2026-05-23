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
        if (l.Params.Count == 1)
        {
            if (l.AsParamRefCall() is { } pc)
                return Prefixed(pc.Receiver, pc.Target.Name, $"({DescribeAll(pc.Args)})");
            if (l.AsParamRefAssign() is { } pa)
                return Prefixed(pa.Receiver, pa.Target.Name, $" {pa.Op} {Value.Describe(pa.Value)}");
            if (_skipSubjectRef && l.Body is Unknown u && u.Raw.StartsWith(l.Params[0] + "."))
                return u.Raw[(l.Params[0].Length + 1)..];
            return Value.Describe(l.Body);
        }
        if (l.AsParamRefAssign() is { Op: "=" } pa2)
            return $"{pa2.Target.Name} = {Value.Describe(pa2.Value)}";
        return l.Raw;
    }

    private string Prefixed(Identifier receiver, string memberName, string suffix) =>
        _skipSubjectRef ? $"{memberName}{suffix}" : $"{receiver.Name}.{memberName}{suffix}";
}
