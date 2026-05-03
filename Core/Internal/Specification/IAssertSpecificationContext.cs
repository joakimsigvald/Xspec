using System.Runtime.CompilerServices;

namespace Xspec.Internal.Specification;

internal interface IAssertSpecificationContext 
{
    void Assert(Action assert, string actual, string? expected, string verb);
    void AddThen();
    void AddVerify<TService>(string expressionExpr);
    void AddAssertThrows<TError>(string? binder = null);
    void AddAssertThrows(string expectedExpr);
    void AddAssert([CallerMemberName] string? assertName = null);
    void AddAssertConjunction(string conjunction);
    void AddThat();
}