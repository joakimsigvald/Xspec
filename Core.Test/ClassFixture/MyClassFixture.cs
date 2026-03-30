using Xspec.Test.AutoMock;

namespace Xspec.Test.ClassFixture;

public class MyClassFixture : Spec<MyService>
{
    private readonly MyComponent _component;

    public MyClassFixture()
        => Given(_component = new MyComponent(An<IMyLogger>(), A(1)))
        .After(_ => _component.SetValue(_component.GetValue() + 1))
        .Before(_ => _component.SetValue(0));
}