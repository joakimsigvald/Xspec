namespace Xspec;

/// <summary>
/// Continuation for providing a data-generaton strategy for the given type
/// </summary>
/// <typeparam name="TSUT"></typeparam>
/// <typeparam name="TTarget"></typeparam>
public interface IRegisterContinuation<TSUT, TTarget>
{
    /// <summary>
    /// When providing values of type `TTarget`, a value of type `TSource` will be generated and converted to type `TTarget`
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    void As<TSource>();
}
