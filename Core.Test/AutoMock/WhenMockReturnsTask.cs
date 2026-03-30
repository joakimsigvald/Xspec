namespace Xspec.Test.AutoMock;

public class WhenMockReturnsTask : Spec<MyValueIntService, string>
{
    public WhenMockReturnsTask() => When(_ => _.SetAndGetValueAsync(An<int>()));

    [Fact]
    public void Then_CallMockReturnTask() 
        => Then<IMyValueIntRepo>(_ => _.SetAsync(The<int>()))
            .And<IMyValueIntRepo>(_ => _.GetAsync(The<int>()));
}