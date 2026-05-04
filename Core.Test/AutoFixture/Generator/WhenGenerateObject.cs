using Xspec.Assert;

namespace Xspec.Test.AutoFixture.Generator;

public class WhenGenerateTwoLevelModel : Spec<ModelLevel1>
{
    [Fact]
    public void ThenRootIsGenerated()
        => When(_ => _).Then().Result.Is().not.Null();

    [Fact]
    public void Then1stLevelSubComponentIsGenerated()
        => When(_ => _).Then().Result.Sub1.Is().not.Null();

    [Fact]
    public void Then2ndLevelSubComponentIsGenerated()
        => When(_ => _).Then().Result.Sub1.Sub2.Is().not.Null();
}

public class WhenGenerateThreeLevelModel : Spec<ModelLevel1>
{
    [Fact]
    public void ThenFourthLevelRemainsNull()
        => When(_ => _).Then().Result.Sub1.Sub2.Sub3.Is().Null();
}

public class WhenGenerateWrappedSubComponent : Spec<WrappedModel>
{
    [Fact]
    public void ThenRootIsGenerated()
        => When(_ => _).Then().Result.Is().not.Null();

    [Fact]
    public void ThenWrappedSubComponentIsGenerated()
        => When(_ => _).Then().Result.LazySub.Value.Is().not.Null();

    [Fact]
    public void ThenWrappedSubComponentBypassesRootDepthLimit()
        // Because Lazy<T> defers generation to the MockingStrategy, 
        // evaluating .Value triggers a fresh GenerationRequest with Depth = 0.
        // Therefore, its sub-component (SubSub) is successfully generated.
        => When(_ => _).Then().Result.LazySub.Value.Sub2.Sub3.Is().not.Null();
}

public class ModelLevel4
{
}

public class ModelLevel3
{
    public ModelLevel4 Sub3 { get; set; }
}

public class ModelLevel2
{
    public ModelLevel3 Sub2 { get; set; }
}

public class ModelLevel1
{
    public ModelLevel2 Sub1 { get; set; }
}

public class WrappedModel
{
    // Lazy<T> is handled by MockingStrategy, so it resets the depth counter
    public Lazy<ModelLevel2> LazySub { get; set; }
}