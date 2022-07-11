using System;

public class QuickRaceResultState : InjectedBState
{
    private QuickRaceResultPresenter presenter;
    private UIRaceResultList uiRaceResultList;
    public override void Enter()
    {
        base.Enter();
        presenter = new QuickRaceResultPresenter(Container);
        presenter.OnBackToMainState += ToMainState;
        presenter.ShowResultAsynnc().Forget();
    }

    private void ToMainState()
    {
        this.GetSuperMachine<RootFSM>().ToChildStateRecursive<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        UILoader.SafeRelease(ref uiRaceResultList);
        presenter?.Dispose();
    }
}