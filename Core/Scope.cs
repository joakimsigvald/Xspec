namespace Xspec;

/// <summary>
/// Scope defines which part of the test pipeline provided values and setups are aplied to.
/// They can either be used when constructing the subject under test, or mocks (`Construction`), 
/// or they can be applied as defaults (`Default`) for test-data generation (input),
/// or neither (`None`) or both (`All`)
/// </summary>
[Flags]
public enum Scope
{
    /// <summary>
    /// Default scope neither applies to Default nor Construction.
    /// </summary>
    None,
    /// <summary>
    /// Used as fallback test data when generating arbitrary values.
    /// </summary>
    Default,

    /// <summary>
    /// Injected directly into the constructor of the Subject Under Test.
    /// </summary>
    Construction,

    /// <summary>
    /// Used both for SUT construction and as default test data.
    /// </summary>
    All
}