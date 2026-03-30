using Xspec.Internal.Pipelines;

namespace Xspec;

/// <summary>
/// Interface for classes that are a container of one or more unit tests. Implemented by Spec
/// </summary>
/// <typeparam name="TSUT"></typeparam>
public interface IFixture<TSUT> 
{
    /// <summary>
    /// Retrieve the test specification for print-out after or during test execution
    /// </summary>
    string Specification { get; }
    internal Fixture<TSUT> Fixture { get; }
}