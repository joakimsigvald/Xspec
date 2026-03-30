namespace Xspec.Internal.Pipelines;

internal record Command(Delegate Invocation, string Expression);