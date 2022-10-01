public class TrainingState : InjectedBHState
{
    public override void AddStates()
    {
        base.AddStates();
        AddState<TrainingUIState>();
        AddState<TrainingActionState>();
        SetInitialState<TrainingUIState>();
    }
}