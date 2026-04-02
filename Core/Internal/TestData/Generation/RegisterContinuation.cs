using Xspec.Internal.Pipelines;

namespace Xspec.Internal.TestData.Generation;

internal class RegisterContinuation<TSUT, TTarget>(Fixture<TSUT> fixture) : IRegisterContinuation<TSUT, TTarget>
{
    public void As<TSource>() => fixture.Register<TTarget, TSource>();
}