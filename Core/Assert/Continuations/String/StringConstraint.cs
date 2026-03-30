namespace Xspec.Assert.Continuations.String;

/// <summary>
/// Object that allows an assertions to be made on the provided string
/// </summary>
public abstract record StringConstraint<TContinuation> : Constraint<string, TContinuation>
    where TContinuation : StringConstraint<TContinuation>, new();