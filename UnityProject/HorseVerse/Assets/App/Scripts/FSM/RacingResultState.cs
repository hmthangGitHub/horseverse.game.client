using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class RacingResultState : InjectedBState
{
    private UIBackGroundPresenter uiBackGroundPresenter;
    private QuickRaceResultPresenter presenter;
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();
    private HorseRaceContext horseRaceContext;
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    
    public override void Enter()
    {
        base.Enter();
        OnStateEnterAsync().Forget();
    }

    private async UniTask OnStateEnterAsync()
    {
        await UIBackGroundPresenter.ShowBackGroundAsync();
        if (HorseRaceContext.RaceMatchDataContext.IsReplay == false)
        {
            presenter = new QuickRaceResultPresenter(Container);
            await presenter.ShowResultAsync();
            this.GetSuperMachine<RootFSM>().ChangeToChildStateRecursive<RacingMenuState>();
        }
        else
        {
            this.GetSuperMachine<RootFSM>().ChangeToChildStateRecursive<RacingHistoryState>();
        }
    }

    public override void Exit()
    {
        base.Exit();
        DisposeUtility.SafeDispose(ref presenter);
    }
}