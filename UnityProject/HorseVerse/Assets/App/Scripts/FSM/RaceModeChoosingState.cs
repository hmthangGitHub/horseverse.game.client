using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using RobustFSM.Interfaces;

public class RaceModeChoosingState : InjectedBState
{
    private RaceModeChoosingPresenter raceModeChoosingPresenter;
    private UIBackGroundPresenter uiBackGroundPresenter;
    private UIHorse3DViewPresenter uiHorse3DViewPresenter ;
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();

    public override void Enter()
    {
        base.Enter();
        OnEnterAsync().Forget();
    }

    private async UniTask OnEnterAsync()
    {
        await UIBackGroundPresenter.ShowBackGroundAsync();
        await UIHorse3DViewPresenter.HideHorse3DViewAsync();
        
        raceModeChoosingPresenter = new RaceModeChoosingPresenter(Container);
        raceModeChoosingPresenter.OnFinishChooseRaceMode += OnFinishChooseRaceMode;
        raceModeChoosingPresenter.OnViewHistory += OnViewHistory;
        raceModeChoosingPresenter.OnBack += OnBack;
        raceModeChoosingPresenter.ChooseRaceModeAsync()
                                 .Forget();
    }

    private void OnViewHistory()
    {
        Machine.ChangeState<RacingHistoryState>();
    }

    private void OnBack()
    {
        OnBackAsync().Forget();
    }

    private async UniTask OnBackAsync()
    {
        await OnBeforeExitStateAsync();
        ((IState)Machine).Machine.ChangeState<MainMenuState>();
    }

    private void OnFinishChooseRaceMode()
    {
        OnFinishChooseRaceModeAsync().Forget();
    }

    private async UniTask OnFinishChooseRaceModeAsync()
    {
        await OnBeforeExitStateAsync();
        Machine.ChangeState<RacingMenuState>();
    }

    private async UniTask OnBeforeExitStateAsync()
    {
        UIBackGroundPresenter.ReleaseBackGround();
    }

    public override void Exit()
    {
        base.Exit();
        raceModeChoosingPresenter.OnBack -= OnBack;
        raceModeChoosingPresenter.OnViewHistory -= OnViewHistory;
        raceModeChoosingPresenter.OnFinishChooseRaceMode -= OnFinishChooseRaceMode;
        DisposeUtility.SafeDispose(ref raceModeChoosingPresenter);
    }
}