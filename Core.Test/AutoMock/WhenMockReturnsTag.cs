using Xspec.Assert;

namespace Xspec.Test.AutoMock;

public class WhenMockReturnsTag : Spec<InterfaceService, int> 
{
    const int _123 = 123;
    static readonly Tag<int> value = new();

    public WhenMockReturnsTag() 
        => When(_ => _.GetServiceValue())
        .Given<IMyService>().That(_ => _.GetValue()).Returns(value)
        .And(value).Is(_123);

    [Fact]
    public void ThenReturnTaggedValue()
    {
        Then().Result.Is(_123);
        Specification.Is(
            """
            Given value is _123
              and IMyService.GetValue() returns value
            When _.GetServiceValue()
            Then Result is _123
            """);
    }
}

public class WhenMockWithTag : Spec<InterfaceService>
{
    static readonly Tag<int> value = new(nameof(value));

    [Fact]
    public void WhenThrowsSpecificException()
    {
        When(_ => _.SetValue(The(value)))
        .Given<IMyService>().That(_ => _.SetValue(The(value))).Throws(() => new ArgumentException())
        .Then().Throws<ArgumentException>();
        Specification.Is(
            """
            Given IMyService.SetValue(the value) throws new ArgumentException()
            When _.SetValue(the value)
            Then throws ArgumentException
            """);
    }

    [Fact]
    public void WhenThrowsTypeOfException()
    {
        When(_ => _.SetValue(The(value)))
        .Given<IMyService>().That(_ => _.SetValue(The(value))).Throws<ArgumentException>()
        .Then().Throws<ArgumentException>();
        Specification.Is(
            """
            Given IMyService.SetValue(the value) throws ArgumentException
            When _.SetValue(the value)
            Then throws ArgumentException
            """);
    }
}