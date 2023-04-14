public class RacingHistoryState : InjectedBHState
{
    public override void AddStates()
    {
        base.AddStates();
        AddState<HorseRaceActionState>();
        AddState<RacingHistoryMenuState>();
        SetInitialState<RacingHistoryMenuState>();
    }

    public override void Enter()
    {
        base.Enter();
        Container.Bind(new HorseRaceManagerFactory(Container));
    }

    public override void Exit()
    {
        base.Exit();
        Container.RemoveAndDisposeIfNeed<HorseRaceManagerFactory>();
    }
}
