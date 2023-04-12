using Cysharp.Threading.Tasks;

public class BetHistoryState : InjectedBHState
{
    private BetModeHistoryPresenter betModeHistoryPresenter;
    
    public override void Enter()
    {
        base.Enter();
        OnEnterStateAsync().Forget();
    }

    public override void Exit()
    {
        base.Exit();
        DisposeUtility.SafeDispose(ref betModeHistoryPresenter);
    }

    public override void AddStates()
    {
        base.AddStates();
        AddState<BetHistoryListState>();
        AddState<BetHistoryDetailState>();
        AddState<BetHistoryUserBetSummaryState>();
        SetInitialState<BetHistoryListState>();
    }

    private async UniTask OnEnterStateAsync()
    {
        betModeHistoryPresenter = new BetModeHistoryPresenter(Container);
        betModeHistoryPresenter.ShowHistoryAsync()
                               .Forget();
    }
}