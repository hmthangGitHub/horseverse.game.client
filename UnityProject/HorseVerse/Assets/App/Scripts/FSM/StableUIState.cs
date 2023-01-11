using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableUIState : InjectedBState
{
    private UIHorseStablePresenter uiHorseStablePresenter = default;
    private UIHeaderPresenter uiHeaderPresenter = default;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private UIHorse3DViewPresenter uiHorse3DViewPresenter = default;
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();

    public override void Enter()
    {
        base.Enter();
        //UIHorse3DViewPresenter.HideHorse3DViewAsync().Forget();
        UIHorse3DViewPresenter.ShowHorse3DViewAsync(1).Forget();
        uiHorseStablePresenter ??= new UIHorseStablePresenter(Container);
        uiHorseStablePresenter.OnViewHorseDetail += OnViewHorseDetail;
        UIHeaderPresenter.ShowHeaderAsync(true, "STABLE").Forget();
        UIHeaderPresenter.OnBack += OnBack;
        uiHorseStablePresenter.ShowUIHorseStableAsync().Forget();

    }

    private void OnBack()
    {
        OnBackAsync().Forget();
    }

    private async UniTaskVoid OnBackAsync()
    {
        await uiHorseStablePresenter.OutAsync();
        this.GetMachine<StableState>().GetMachine<InitialState>().ChangeState<MainMenuState>();
    }

    private void OnViewHorseDetail()
    {
        this.Machine.ChangeState<StableHorseDetailState>();
    }

    public override void Exit()
    {
        base.Exit();
        Release();
    }

    void Release()
    {
        UIHeaderPresenter.HideHeader();
        uiHorseStablePresenter.OnViewHorseDetail -= OnViewHorseDetail;
        UIHeaderPresenter.OnBack -= OnBack;
        uiHorseStablePresenter.Dispose();
        uiHorseStablePresenter = default;
    }
}
