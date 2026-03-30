using Xspec.Assert;

namespace Xspec.Test.Internal.Verification;

public class WhenThen : Spec<MyStateService>
{
    [Fact] public void ExposeSubjectUnderTestAfterThen() 
        => When(_ => _.SetState(An<int>()))
        .Then().SubjectUnderTest.GetState().Is(The<int>());
}