using Xspec.Assert;

namespace Xspec.Test.Tests.Delay;

public abstract class WhenGetStateAfterSetStateWithAfterDelay : Spec<DelayedState, int>
{
    private static readonly Tag<int> delay = new(), state = new(), wait = new();

    protected WhenGetStateAfterSetStateWithAfterDelay()
        => Given().Default(delay)
        .When(_ => _.State)
        .After(_ => _.SetState(The(state)), () => The(wait));

    public class GivenZeroDelay : WhenGetStateAfterSetStateWithAfterDelay
    {
        public GivenZeroDelay() => Given(delay).Is(0);
        [Fact] public void ThenGetNewState() => Result.Is(The(state));
    }

    public class GivenWaitShorterThanDelay : WhenGetStateAfterSetStateWithAfterDelay
    {
        public GivenWaitShorterThanDelay() => Given(delay).Is(200).And(wait).Is(100);
        [Fact] public void ThenGetInitialState() => Result.Is(0);
    }

    public class GivenWaitLongerThanDelay : WhenGetStateAfterSetStateWithAfterDelay
    {
        public GivenWaitLongerThanDelay() => Given(delay).Is(100).And(wait).Is(200);
        [Fact]
        public void ThenGetNewState()
        {
            Result.Is(The(state));
            Specification.Is(
                """
                Given wait is 200
                  and delay is 100
                  and delay is default
                When _.State
                After wait () => The(wait) ms
                After _.SetState(the state)
                Then Result is the state
                """);
        }
    }
}

public abstract class WhenGetStateAfterSetStateWithAsyncTaskDelay : Spec<DelayedState, int>
{
    private static readonly Tag<int> delay = new(), state = new(), wait = new();

    protected WhenGetStateAfterSetStateWithAsyncTaskDelay()
        => Given().Default(() => The(delay))
        .When(_ => _.State)
        .After(async _ =>
        {
            _.SetState(The(state));
            await Task.Delay(The(wait));
        });

    public class GivenZeroDelay : WhenGetStateAfterSetStateWithAsyncTaskDelay
    {
        public GivenZeroDelay() => Given(delay).Is(0);
        [Fact] public void ThenGetNewState() => Result.Is(The(state));
    }

    public class GivenWaitShorterThanDelay : WhenGetStateAfterSetStateWithAsyncTaskDelay
    {
        public GivenWaitShorterThanDelay() => Given(delay).Is(200).And(wait).Is(100);
        [Fact] public void ThenGetInitialState() => Result.Is(0);
    }

    public class GivenWaitLongerThanDelay : WhenGetStateAfterSetStateWithAsyncTaskDelay
    {
        public GivenWaitLongerThanDelay() => Given(delay).Is(100).And(wait).Is(200);
        [Fact] public void ThenGetNewState() => Result.Is(The(state));
    }
}