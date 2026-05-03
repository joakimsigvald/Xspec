using System.Diagnostics.CodeAnalysis;
using Xspec.Internal.Specification;

namespace Xspec.Assert.Continuations;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TContinuation"></typeparam>
/// <typeparam name="TThat"></typeparam>
public class ContinueWithThat<TContinuation, TThat> : ContinueWith<TContinuation> 
    where TContinuation : Constraint
{
    private readonly TThat _that;

    internal ContinueWithThat(TContinuation continuation, TThat that) : base(continuation)
        => _that = that;

    /// <summary>
    /// Continuation to apply assertions on the element
    /// </summary>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Special convension of binding words")]
    public TThat that
    {
        get
        {
            SpecificationContext.Current.AddThat();
            return _that;
        }
    }
}