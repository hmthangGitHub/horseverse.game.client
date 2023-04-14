public class RacingThirdPersonMenuState : InjectedBHState
{
    public override void AddStates()
    {
        base.AddStates();
        AddState<RacingMenuState>();
        AddState<HorseRaceActionState>();
        SetInitialState<RacingMenuState>();
    }

    public override void Enter()
    {
        base.Enter();
        Container.Bind(new HorseRaceThirdPersonFactory(Container));
    }

    public override void Exit()
    {
        base.Exit();
        Container.RemoveAndDisposeIfNeed<HorseRaceThirdPersonFactory>();
    }
}
