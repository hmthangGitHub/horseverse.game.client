using Cysharp.Threading.Tasks;

public class BetModeHistoryState : InjectedBState
{
    private UIHeaderPresenter uiHeaderPresenter;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private BetModeHistoryPresenter betModeHistoryPresenter;
    private BetModeHistoryPresenter BetModeHistoryPresenter => betModeHistoryPresenter ??= Container.Inject<BetModeHistoryPresenter>();
    
    public override void Enter()
    {
        base.Enter();
        UIHeaderPresenter.OnBack += OnBackBtn;
        UIHeaderPresenter.ShowHeaderAsync(true, "ARENA HISTORY").Forget();
        BetModeHistoryPresenter.ShowHistoryAsync().Forget();
    }

    private void OnBackBtn()
    {
        OnBackBtnAsync().Forget();
    }

    private async UniTask OnBackBtnAsync()
    {
        await BetModeHistoryPresenter.Out();
        Machine.ChangeState<BetModeUIEntryState>();
    }

    public override void Exit()
    {
        base.Exit();
        UIHeaderPresenter.OnBack -= OnBackBtn;
        betModeHistoryPresenter = default;
    }
}