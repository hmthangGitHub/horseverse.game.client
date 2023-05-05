public class BreedingFinishState : InjectedBState
{
    private BreedingFinishPresenter presenter;
    private UIHeaderPresenter uiHeaderPresenter;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    
    public override void Enter()
    {
        base.Enter();
        UIHeaderPresenter.OnBack += OnBack;
        presenter = new BreedingFinishPresenter(Container);
        presenter.FinishBreedingAsync().Forget();
    }

    private void OnBack()
    {
        Machine.ChangeState<BreedingSlotState>();
    }

    public override void Exit()
    {
        base.Exit();
        UIHeaderPresenter.OnBack -= OnBack;
        DisposeUtility.SafeDispose(ref presenter);
        uiHeaderPresenter = default;
    }
}