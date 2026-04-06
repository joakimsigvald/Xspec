using Xspec.Continuations;
using Xspec.Internal.Pipelines;

namespace Xspec.Internal.TestData.Generation;

[Obsolete("Do not use")]
internal class RegisterContinuation<TSUT, TTarget>(Fixture<TSUT> fixture) : IRegisterContinuation<TSUT, TTarget>
{
    public void As<TSource>() => fixture.Register<TTarget, TSource>();
    public void As<TSource>(Func<TSource, TTarget> convert) => fixture.Register(convert);
}