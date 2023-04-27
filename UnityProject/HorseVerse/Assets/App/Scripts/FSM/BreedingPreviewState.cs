public class BreedingPreviewState : InjectedBState
{
    private BreedingPreviewPresenter presenter;
    public override void Enter()
    {
        base.Enter();
        presenter = new BreedingPreviewPresenter(Container);
        presenter.OnBack += OnBackToSlotState;
        presenter.ShowHorseBreedingPreviewAsync().Forget();
    }

    private void OnBackToSlotState()
    {
        Machine.ChangeState<BreedingSlotState>();
    }

    public override void Exit()
    {
        base.Exit();
        presenter.OnBack -= OnBackToSlotState;
        DisposeUtility.SafeDispose(ref presenter);
    }
}