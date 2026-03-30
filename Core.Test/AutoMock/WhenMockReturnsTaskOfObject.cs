using Xspec.Assert;

namespace Xspec.Test.AutoMock;

public class WhenMockReturnsTaskOfObject : Spec<MyValueIntService, object>
{
    public WhenMockReturnsTaskOfObject() => When(_ => _.GetObjectAsync());

    [Fact]
    public void Then_ReturnAnObject()
    {
        Result.GetType().Is(typeof(object));
        Specification.Is(
            """
            When _.GetObjectAsync()
            Then Result.GetType() is typeof object
            """);
    }
}