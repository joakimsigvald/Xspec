using Xspec.Architecture;

namespace Xspec.Test.Architecture;

// Architecture policy (what makes a project a contract or a host) belongs in user code,
// as extension methods on the model types — the method name becomes the sentence fragment.
internal static class ArchPolicy
{
    internal static bool IsContract(this Project project) => project.Name.EndsWith(".Contracts");

    internal static bool IsHost(this Project project) => project.Name.EndsWith(".Host");
}
