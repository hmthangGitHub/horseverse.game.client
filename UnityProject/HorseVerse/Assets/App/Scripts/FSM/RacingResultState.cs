using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class RacingResultState : InjectedBState
{
    private UIBackGroundPresenter uiBackGroundPresenter;
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();
    public override void Enter()
    {
        base.Enter();
        OnStateEnterAsync().Forget();
    }

    private async UniTask OnStateEnterAsync()
    {
        await UIBackGroundPresenter.ShowBackGroundAsync();
        using var presenter = new QuickRaceResultPresenter(Container);
        await presenter.ShowResultAsync();
        this.GetSuperMachine<RootFSM>().ChangeToChildStateRecursive<RacingMenuState>();
    }
}