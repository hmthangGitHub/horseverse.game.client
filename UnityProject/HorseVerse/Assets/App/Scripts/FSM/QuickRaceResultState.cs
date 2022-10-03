using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class QuickRaceResultState : InjectedBState
{
    public override void Enter()
    {
        base.Enter();
        OnStateEnterAsync().Forget();
    }

    private async UniTask OnStateEnterAsync()
    {
        using var presenter = new QuickRaceResultPresenter(Container);
        await presenter.ShowResultAsync();
        this.GetSuperMachine<RootFSM>().ChangeToChildStateRecursive<MainMenuState>();
    }
}