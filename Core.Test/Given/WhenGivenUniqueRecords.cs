using Xspec.Assert;
using Xspec.Test.TestData;
using Xunit.Sdk;

namespace Xspec.Test.Given;

public class WhenGivenUniqueRecords : Spec<MyRecord[]>
{
    [Fact]
    public void WithEnoughValueSpace_ThenGenerateUniqueModelArray()
    {
        int range = 10;
        When(_ => Five<MyRecord>()).Given().Default<int>(i => i % range).Using("Abc", For.Input)
            .Then().Result.Is().Distinct()
            .and.Has().All(m => m.Id >= 0 && m.Id < range);
        Specification.Is(
            """
            Using "Abc" for Input
            Given int is i % range
            When five MyRecord
            Then Result is distinct
                and has all m.Id >= 0 && m.Id < range
            """);
    }

    [Fact]
    public void WithNotEnoughValueSpace_ThenThrowXUnitException()
    {
        int range = 4;
        Xunit.Assert.Throws<XunitException>(() => 
        When(_ => Five<MyRecord>()).Given().Default<int>(i => i % range).Using("Abc", For.Input)
            .Then().Result.Is().Distinct()
            .and.Has().All(m => m.Id >= 0 && m.Id < range));
    }
}