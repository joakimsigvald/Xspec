namespace Xspec.Assert.Continuations;

/// <summary>
/// Object that allows an assertions to be made on the provided object
/// </summary>
public record HasObject : Constraint<object, HasObject>
{
    /// <summary>
    /// Assert that the object is of the given type
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <returns></returns>
    public ContinueWith<HasObject> Type<TObject>()
        => Assert(Ignore.Me, actual => (actual is TObject).Is().True(), typeof(TObject).Name).And();
}