namespace Xspec.Assert.Continuations;

/// <summary>
/// Object that allows an assertions to be made on the provided bool
/// </summary>
public record IsBool : Constraint<bool, IsBool>
{
    /// <summary>
    /// Assert that the value is true
    /// </summary>
    public ContinueWith<IsBool> True() => Assert(Ignore.Me, Xunit.Assert.True).And();

    /// <summary>
    /// Assert that the value is false
    /// </summary>
    public ContinueWith<IsBool> False() => Assert(Ignore.Me, Xunit.Assert.False).And();
}