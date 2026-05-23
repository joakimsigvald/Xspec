namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Describes an expression in "value" context (used by <c>ParseValue</c>).
/// Mention detection runs first; the switch below covers every other shape.
/// </summary>
internal static class ValueDescriber
{
    public static string Describe(Expr expr)
    {
        if (MentionDescriber.TryDescribe(expr, out var mention)) return mention;
        return expr switch
        {
            Lambda l when l.Params.Count <= 2
                && DescribeHelpers.IsParamRefAssignment(l, out var prop, out var value, out var op)
                && op == "=" => $"{prop} = {Describe(value)}",
            Lambda l when l.Params.Count <= 1 && l.Body is With w => DescribeHelpers.DescribeWithInits(w),
            Lambda l when l.Params.Count <= 1 => Describe(l.Body),
            Lambda l => l.Raw,
            Assign a => $"{DescribeHelpers.DescribeAssignTarget(a.Target)} {a.Op} {Describe(a.Value)}",
            With w => DescribeHelpers.DescribeWithInits(w),
            Tuple t => $"({DescribeHelpers.JoinDescribed(t.Items)})",
            ArrayLit arr => $"[{DescribeHelpers.JoinDescribed(arr.Items)}]",
            Binary b => $"{Describe(b.Left)} {b.Op} {Describe(b.Right)}",
            Unary u => $"{u.Op}{Describe(u.Operand)}",
            Postfix p => $"{Describe(p.Operand)}{p.Op}",
            Conditional c => $"{Describe(c.Cond)} ? {Describe(c.Then)} : {Describe(c.Else)}",
            Cast c => $"({c.TypeName}){Describe(c.Operand)}",
            IsAs ia => $"{Describe(ia.Operand)} {ia.Op} {ia.TypeName}",
            InterpolatedString s => DescribeHelpers.Requote(s.Raw),
            Literal lit => DescribeHelpers.DescribeLiteral(lit.Raw),
            New n => NewDescriber.Describe(n),
            Call c when IsDefaultOf(c) => $"default {Describe(c.Args[0])}",
            Call c when c.Target is Identifier id && c.Args.Count >= 1
                => $"{DescribeHelpers.AsWords(id.Name)} {DescribeHelpers.JoinDescribed(c.Args)}",
            Call c => ShapeRenderer.DescribeCallShape(c),
            Generic g => g.Raw,
            Member m => ShapeRenderer.DescribeMember(m),
            Identifier id => id.Name,
            Unknown u => u.Raw,
            _ => expr.Raw,
        };
    }

    private static bool IsDefaultOf(Call c)
        => c.Target is Literal lit && lit.Raw == "default" && c.Args.Count == 1;
}
