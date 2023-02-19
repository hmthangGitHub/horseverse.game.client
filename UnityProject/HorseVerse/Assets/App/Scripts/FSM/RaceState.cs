internal class RaceState : InjectedBHState
{
    public override void AddStates()
    {
        base.AddStates();
        AddState<HorseRaceActionState>();
        AddState<BetModeRaceResultState>();
        SetInitialState<HorseRaceActionState>();
    }
}