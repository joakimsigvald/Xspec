namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Renders the textual shape of call/member expressions without applying any
/// Mention-style verb lowering. Used as a fallback when no specialized
/// description rule fires.
/// </summary>
internal static class ShapeRenderer
{
    public static string DescribeCallShape(Call call)
    {
        var argText = DescribeHelpers.JoinDescribed(call.Args);
        return call.Target switch
        {
            Generic g => $"{DescribeCallTarget(g)}({argText})",
            Member m => $"{DescribeMemberTarget(m.Target)}.{m.Name}({argText})",
            Identifier id => $"{id.Name}({argText})",
            _ => $"{call.Target.Raw}({argText})",
        };
    }

    public static string DescribeCallTarget(Expr target) => target switch
    {
        Generic g when g.Target is Identifier id
            => $"{id.Name}<{string.Join(", ", g.TypeArgs.Select(t => t.Raw))}>",
        Member m => $"{DescribeMemberTarget(m.Target)}.{m.Name}",
        Identifier id => id.Name,
        _ => target.Raw,
    };

    public static string DescribeMemberTarget(Expr target) => target switch
    {
        Identifier id => id.Name,
        Member m => $"{DescribeMemberTarget(m.Target)}.{m.Name}",
        _ => target.Raw,
    };

    public static string DescribeMember(Member m) => $"{DescribeMemberTarget(m.Target)}.{m.Name}";

    public static string? GetMethodName(Call c) => c.Target switch
    {
        Identifier id => id.Name,
        Member m => m.Name,
        Generic g when g.Target is Identifier gi => gi.Name,
        Generic g when g.Target is Member gm => gm.Name,
        _ => null,
    };
}
