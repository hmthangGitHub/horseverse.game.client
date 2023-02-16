using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class RacingResultState : InjectedBState
{
    private UIBackGroundPresenter uiBackGroundPresenter;
    private QuickRaceResultPresenter presenter;
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();
    public override void Enter()
    {
        base.Enter();
        OnStateEnterAsync().Forget();
    }

    private async UniTask OnStateEnterAsync()
    {
        await UIBackGroundPresenter.ShowBackGroundAsync();
        presenter = new QuickRaceResultPresenter(Container);
        await presenter.ShowResultAsync();
        this.GetSuperMachine<RootFSM>().ChangeToChildStateRecursive<RacingMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        DisposeUtility.SafeDispose(ref presenter);
    }
}