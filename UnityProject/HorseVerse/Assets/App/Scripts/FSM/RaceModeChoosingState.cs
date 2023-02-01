public class RaceModeChoosingState : InjectedBState
{
    private RaceModeChoosingPresenter raceModeChoosingPresenter;

    public override void Enter()
    {
        base.Enter();
        raceModeChoosingPresenter = new RaceModeChoosingPresenter(Container);
        raceModeChoosingPresenter.OnFinishChooseRaceMode += OnFinishChooseRaceMode;
        raceModeChoosingPresenter.ChooseRaceModeAsync().Forget();
    }

    private void OnFinishChooseRaceMode()
    {
        Machine.ChangeState<QuickRaceMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        raceModeChoosingPresenter.OnFinishChooseRaceMode -= OnFinishChooseRaceMode;
        DisposeUtility.SafeDispose(ref raceModeChoosingPresenter);
    }
}