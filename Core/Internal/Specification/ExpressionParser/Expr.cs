namespace Xspec.Internal.Specification.ExpressionParserInternals;

internal abstract record Expr(string Raw);

internal sealed record Identifier(string Raw, string Name) : Expr(Raw);

internal sealed record Literal(string Raw) : Expr(Raw);

internal sealed record InterpolatedString(string Raw) : Expr(Raw);

internal sealed record Member(string Raw, Expr Target, string Name) : Expr(Raw);

internal sealed record Generic(string Raw, Expr Target, IReadOnlyList<Expr> TypeArgs) : Expr(Raw);

internal sealed record Call(string Raw, Expr Target, IReadOnlyList<Expr> Args) : Expr(Raw);

internal sealed record Index(string Raw, Expr Target, IReadOnlyList<Expr> Args) : Expr(Raw);

internal sealed record New(
    string Raw,
    string? TypeName,
    IReadOnlyList<Expr> Args,
    IReadOnlyList<Expr>? Init) : Expr(Raw);

internal sealed record With(string Raw, Expr Target, IReadOnlyList<Expr> Init) : Expr(Raw);

internal sealed record Lambda(string Raw, IReadOnlyList<string> Params, Expr Body) : Expr(Raw);

internal sealed record Assign(string Raw, string Op, Expr Target, Expr Value) : Expr(Raw);

internal sealed record Binary(string Raw, string Op, Expr Left, Expr Right) : Expr(Raw);

internal sealed record Unary(string Raw, string Op, Expr Operand) : Expr(Raw);

internal sealed record Postfix(string Raw, string Op, Expr Operand) : Expr(Raw);

internal sealed record Conditional(string Raw, Expr Cond, Expr Then, Expr Else) : Expr(Raw);

internal sealed record Cast(string Raw, string TypeName, Expr Operand) : Expr(Raw);

internal sealed record IsAs(string Raw, string Op, Expr Operand, string TypeName) : Expr(Raw);

internal sealed record Tuple(string Raw, IReadOnlyList<Expr> Items) : Expr(Raw);

internal sealed record ArrayLit(string Raw, IReadOnlyList<Expr> Items) : Expr(Raw);

internal sealed record Unknown(string Raw) : Expr(Raw);
