namespace Xspec.Internal.Specification.ExpressionParsing.Expressions;

internal sealed record Unknown(string Raw) : Expr(Raw);