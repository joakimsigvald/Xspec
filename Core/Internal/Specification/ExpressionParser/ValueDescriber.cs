using Xspec.Internal.Specification;

namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Value-mode description. Mention detection runs first; the switch below
/// covers every other shape. Recursion uses <see cref="Describe"/> directly
/// since values inside values stay in value mode.
/// </summary>
internal sealed class ValueDescriber : Describer
{
    public override string Describe(Expr expr)
    {
        if (TryDescribeMention(expr, out var mention)) return mention;
        return expr switch
        {
            Lambda l when l.Params.Count <= 2 && l.AsParamRefAssign() is { Op: "=" } pa
                => $"{pa.Target.Name} = {Describe(pa.Value)}",
            Lambda l when l.Params.Count <= 1 && l.Body is With w => DescribeAll(w.Init),
            Lambda l when l.Params.Count <= 1 => Describe(l.Body),
            Lambda l => l.Raw,
            Assign a => $"{AssignTargetName(a.Target)} {a.Op} {Describe(a.Value)}",
            With w => DescribeAll(w.Init),
            Tuple t => $"({string.Join(", ", t.Items.Select(Describe))})",
            ArrayLit arr => $"[{string.Join(", ", arr.Items.Select(Describe))}]",
            Binary b => $"{Describe(b.Left)} {b.Op} {Describe(b.Right)}",
            Unary u => $"{u.Op}{Describe(u.Operand)}",
            Postfix p => $"{Describe(p.Operand)}{p.Op}",
            Conditional c => $"{Describe(c.Cond)} ? {Describe(c.Then)} : {Describe(c.Else)}",
            Cast c => $"({c.TypeName}){Describe(c.Operand)}",
            IsAs ia => $"{Describe(ia.Operand)} {ia.Op} {ia.TypeName}",
            InterpolatedString s => s.Quoted,
            Literal lit => lit.Quoted,
            New n => DescribeNew(n),
            Call c when IsDefaultOf(c) => $"default {Describe(c.Args[0])}",
            Call c when c.Target is Identifier id && c.Args.Count >= 1
                => $"{id.Name.AsWords()} {string.Join(", ", c.Args.Select(Describe))}",
            Call c => $"{c.Target.AsPath()}({string.Join(", ", c.Args.Select(Describe))})",
            Identifier id => id.Name,
            Unknown u => u.Raw,
            _ => expr.AsPath(),
        };
    }

    private static bool IsDefaultOf(Call c)
        => c.Target is Literal lit && lit.Raw == "default" && c.Args.Count == 1;

    private static string AssignTargetName(Expr target) => target switch
    {
        Member m => m.Name,
        Identifier id => id.Name,
        _ => target.Raw,
    };
}
