using Xspec.Assert;
using Xspec.Internal.Specification;
using Xspec.Test.AutoFixture;
using Xspec.Test.Subjects.RecordStructDefaults;

namespace Xspec.Test.Internal.Specification.TypeExtensions;

public class WhenAlias : Spec<Type, string>
{
    public WhenAlias() => When(_ => _.Alias());

    [Fact] public void GivenString() => Given(typeof(string)).Then().Result.Is("string");
    [Fact] public void GivenMyModel() => Given(typeof(MyModel)).Then().Result.Is("MyModel");
    [Fact] public void GivenArrayOfMyModel() => Given(typeof(MyModel[])).Then().Result.Is("MyModel[]");
    [Fact] public void GivenArrayOfInt() => Given(typeof(int[])).Then().Result.Is("int[]");
    [Fact] public void GivenJaggedArrayOfInt() => Given(typeof(int[][])).Then().Result.Is("int[][]");
    [Fact] public void Given2DArrayOfInt() => Given(typeof(int[,])).Then().Result.Is("int[,]");
    [Fact] public void GivenListOfInt() => Given(typeof(List<int>)).Then().Result.Is("List<int>");
    [Fact] public void GivenIEnumerableOfInt() => Given(typeof(IEnumerable<int>)).Then().Result.Is("IEnumerable<int>");
    [Fact] public void GivenGenericClass() => Given(typeof(Moq.Mock<MyModel>)).Then().Result.Is("Mock<MyModel>");
    [Fact] public void GivenGenericInterface() => Given(typeof(Moq.IMock<MyModel>)).Then().Result.Is("IMock<MyModel>");
    [Fact] public void GivenTwoGenericParameters() => Given(typeof(Key<int, long>)).Then().Result.Is("Key<int, long>");
    [Fact] public void GivenNestedGenericParameters() => Given(typeof(Moq.Mock<Moq.IMock<MyModel>>)).Then().Result.Is("Mock<IMock<MyModel>>");
}