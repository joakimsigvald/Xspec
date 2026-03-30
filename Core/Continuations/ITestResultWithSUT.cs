namespace Xspec.Continuations;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TSUT"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface ITestResultWithSUT<TSUT, TResult> : ITestResult<TResult> 
{
    /// <summary>
    /// Provide the subject under test for non-static test methods.
    /// For static test-methods only returns the default- or auto-generated value of the type declared as Subject under test.
    /// </summary>
    TSUT SubjectUnderTest { get; }
}