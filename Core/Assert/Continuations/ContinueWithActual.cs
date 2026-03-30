using System.Diagnostics.CodeAnalysis;

namespace Xspec.Assert.Continuations;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TActual"></typeparam>
/// <param name="actual"></param>
public class ContinueWithActual<TActual>(TActual? actual)
{
    /// <summary>
    /// Continuation to apply additional assertions on the value
    /// </summary>
    [Obsolete("Use and instead")]
    public TActual? And => actual;

    /// <summary>
    /// Continuation to apply additional assertions on the value
    /// </summary>
    [Obsolete("Use but instead")]
    public TActual? But => actual;
    /// <summary>
                                      /// Continuation to apply additional assertions on the value
                                      /// </summary>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Special convension of binding words")]
    public TActual? and => actual;

    /// <summary>
    /// Continuation to apply additional assertions on the value
    /// </summary>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Special convension of binding words")]
    public TActual? but => actual;
}