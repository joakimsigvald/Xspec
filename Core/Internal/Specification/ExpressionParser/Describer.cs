namespace Xspec.Internal.Specification.ExpressionParserInternals;

/// <summary>
/// Entry point for turning an <see cref="Expr"/> tree into the human-readable
/// description used in test specifications and failure messages. Three modes:
/// <list type="bullet">
///   <item><see cref="DescribeValue"/> — values, lambda bodies, init blocks (via <see cref="ValueDescriber"/>)</item>
///   <item><see cref="DescribeCall"/> — mock setups and method-call expressions (via <see cref="CallDescriber"/>)</item>
///   <item><see cref="DescribeActual"/> — actuals being asserted; unwraps <c>Then()</c>/<c>And()</c> (via <see cref="ActualDescriber"/>)</item>
/// </list>
/// </summary>
internal static class Describer
{
    public static string DescribeValue(Expr expr) => ValueDescriber.Describe(expr);

    public static string DescribeCall(Expr expr, bool skipSubjectRef = false)
        => CallDescriber.Describe(expr, skipSubjectRef);

    public static string DescribeActual(string source, Expr expr) => ActualDescriber.Describe(source, expr);
}
