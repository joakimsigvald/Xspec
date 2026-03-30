using Xspec.Assert;
using Xspec.Test.TestData;
using Xunit.Sdk;

namespace Xspec.Test.Given;

public class WhenGivenThatThrows : Spec<MyService, MyModel>
{
    [Fact]
    public void GivenExceptionType_ThenThrowsExceptionOfThatType()
    {
        When(_ => _.GetModel())
            .Given<IMyRepository>().That(_ => _.GetModel()).Throws<NotFound>()
            .Then().Throws<NotFound>();
        Specification.Is(
            """
            Given IMyRepository.GetModel() throws NotFound
            When _.GetModel()
            Then throws NotFound
            """);
    }

    [Fact]
    public void GivenDefaultExceptionType_ThenThrowsExceptionOfThatType()
    {
        When(_ => _.GetModel())
            .Given<IMyRepository>().Throws<NotFound>()
            .Then().Throws<NotFound>();
        Specification.Is(
            """
            Given IMyRepository throws NotFound
            When _.GetModel()
            Then throws NotFound
            """);
    }

    [Fact]
    public void GivenException_ThenThrowsThatException()
    {
        When(_ => _.GetModel())
             .Given<IMyRepository>().That(_ => _.GetModel()).Throws(A<NotFound>)
             .Then().Throws(The<NotFound>);
        Specification.Is(
            """
            Given IMyRepository.GetModel() throws a NotFound
            When _.GetModel()
            Then throws the NotFound
            """);
    }

    [Fact]
    public void GivenDefaultException_ThenThrowsThatException()
    {
        When(_ => _.GetModel())
             .Given<IMyRepository>().Throws(A<NotFound>)
             .Then().Throws(The<NotFound>);
        Specification.Is(
            """
            Given IMyRepository throws a NotFound
            When _.GetModel()
            Then throws the NotFound
            """);
    }

    [Fact]
    public void GivenSpecificException_ThenThrowsThatException()
    {
        When(_ => _.GetModel())
            .Given<IMyRepository>().That(_ => _.GetModel()).Throws(An<Exception>)
            .And(new Exception(A<string>()))
            .Then().Throws(The<Exception>).and.Throws<Exception>(_ => _.Message.Is(The<string>()));
        Specification.Is(
            """
            Given new Exception(a string)
              and IMyRepository.GetModel() throws an Exception
            When _.GetModel()
            Then throws the Exception
              and throws Exception where _.Message is the string
            """);
    }

    [Fact]
    public void GivenExceptionWithProperties_ThenThrowsExceptionWithThoseProperties()
    {
        When(_ => _.GetModel())
             .Given<IMyRepository>().That(_ => _.GetModel()).Throws(A<NotFound>)
             .Then().Throws<NotFound>(_ => _.Message.Is(The<NotFound>().Message));
        Specification.Is(
            """
            Given IMyRepository.GetModel() throws a NotFound
            When _.GetModel()
            Then throws NotFound where _.Message is the NotFound's Message
            """);
    }

    [Fact]
    public void GivenExceptionWithCondition_ThenThrowsExceptionSatisfyingCondition()
    {
        When(_ => _.GetModel())
             .Given<IMyRepository>().That(_ => _.GetModel()).Throws(A<NotFound>)
             .Then().Throws<NotFound>(_ => _.Message == The<NotFound>().Message);
        Specification.Is(
            """
            Given IMyRepository.GetModel() throws a NotFound
            When _.GetModel()
            Then throws NotFound where _.Message == The<NotFound>().Message
            """);
    }

    [Fact]
    public void GivenExceptionWithConditionNotThrown_ThenFailWithMessage()
    {
        When(_ => _.GetModel())
             .Given<IMyRepository>().That(_ => _.GetModel()).Throws(A<NotFound>);
        var ex = Xunit.Assert.Throws<XunitException>(() => Then().Throws<NotFound>(_ => _.Message == "Something else"));
        ex.Message.Is("""Thrown exception Xspec.Test.TestData.NotFound didn't satisfy _.Message == "Something else".""");
    }
}