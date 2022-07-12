internal class RaceState : InjectedBHState
{
    public override void AddStates()
    {
        base.AddStates();
        AddState<HorseRaceState>();
        AddState<BetModeRaceResultState>();
        SetInitialState<HorseRaceState>();
    }
}