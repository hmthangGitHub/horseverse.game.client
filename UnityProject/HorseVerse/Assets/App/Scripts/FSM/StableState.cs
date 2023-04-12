public class StableState : InjectedBHState
{
    public override void AddStates()
    {
        base.AddStates();
        AddState<StableUIState>();
        AddState<StableHorseDetailState>();
        SetInitialState<StableUIState>();
    }
}
