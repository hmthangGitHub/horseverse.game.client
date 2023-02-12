using Cysharp.Threading.Tasks;
using RobustFSM.Interfaces;

public class RacingMatchFindingState : InjectedBState
{
    private RacingMatchFindingPresenter presenter;
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UILoadingPresenter uiLoadingPresenter;
    private HorseRaceContext horseRaceContext;
    private UIHeaderPresenter uiHeaderPresenter;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private UIHorse3DViewPresenter UiHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ??= this.Container.Inject<UILoadingPresenter>();
    
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    
    public override void Enter()
    {
        base.Enter();
        presenter = new RacingMatchFindingPresenter(Container);
        presenter.OnFoundMatch += OnFoundMatch;
        presenter.OnCancelFindMatch += OnCancelFindMatch;
        presenter.FindMatchAsync().Forget();
    }

    private void OnFoundMatch(RaceScriptData raceScriptData)
    {
    }
    
    private async UniTaskVoid OnFoundMatchAsync (RaceScriptData data)
    {
        await UILoadingPresenter.ShowLoadingAsync();

        UiHorse3DViewPresenter.Dispose();
        UIHeaderPresenter.Dispose();
        HorseRaceContext.RaceScriptData = data;
        ((IState)Machine).Machine.ChangeState<HorseRaceState>();
    }

    private void OnCancelFindMatch()
    {
        Machine.ChangeState<EmptyState>();
    }

    public override void Exit()
    {
        base.Exit();
        presenter.OnFoundMatch -= OnFoundMatch;
        presenter.OnCancelFindMatch -= OnCancelFindMatch;
        DisposeUtility.SafeDispose(ref presenter);
        uiHorse3DViewPresenter = default;
        uiLoadingPresenter = default;
    }
}