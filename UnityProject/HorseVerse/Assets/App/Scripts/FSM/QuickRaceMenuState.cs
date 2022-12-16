using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QuickRaceMenuState : InjectedBState
{
    private UIQuickRacePresenter uiQuickRacePresenter;
    private UIHeaderPresenter uiHeaderPresenter;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();

    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UIHorse3DViewPresenter UiHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();

    private UILoadingPresenter uiLoadingPresenter;
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ??= this.Container.Inject<UILoadingPresenter>();
    
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();

    public override void Enter()
    {
        base.Enter();
        OnEnterStateAsync().Forget();
    }

    private async UniTask OnEnterStateAsync()
    {
        uiQuickRacePresenter = new UIQuickRacePresenter(this.Container);
        UIHeaderPresenter.ShowHeaderAsync(true, "RACE").Forget();
        UIHeaderPresenter.OnBack += OnBack;
        UIHeaderPresenter.OnLogOut += OnLogOut;
        uiQuickRacePresenter.OnFoundMatch += OnFoundMatch;
        await uiQuickRacePresenter.ShowUIQuickRaceAsync();
    }
    
    private void OnFoundMatch (RaceMatchData data)
    {
        OnFoundMatchAsync(data).Forget();
    }

    private async UniTaskVoid OnFoundMatchAsync (RaceMatchData data)
    {
        await UILoadingPresenter.ShowLoadingAsync();

        UiHorse3DViewPresenter.Dispose();
        UIHeaderPresenter.Dispose();
        Container.Bind(data);
        this.Machine.ChangeState<HorseRaceState>();
    }

    private void OnBack()
    {
        this.GetMachine<QuickRaceState>().GetMachine<InitialState>().ChangeState<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        Release();
    }

    void Release()
    {
        uiQuickRacePresenter.OnFoundMatch -= OnFoundMatch;
        UIHeaderPresenter.HideHeader();
        UIHeaderPresenter.OnBack -= OnBack;
        uiQuickRacePresenter.Dispose();
        uiQuickRacePresenter = default;
    }

    private void OnLogOut()
    {
        OnLogOutAsync().Forget();
    }

    private async UniTask OnLogOutAsync()
    {
        await uiHorse3DViewPresenter.HideHorse3DViewAsync();
        uiHorse3DViewPresenter.Dispose();
        await SocketClient.Close();
#if MULTI_ACCOUNT
        var indexToken = PlayerPrefs.GetString(GameDefine.TOKEN_CURRENT_KEY_INDEX, "");
        PlayerPrefs.DeleteKey(GameDefine.TOKEN_STORAGE + indexToken);
        PlayerPrefs.DeleteKey(GameDefine.TOKEN_CURRENT_KEY_INDEX);
#else
        PlayerPrefs.DeleteKey(GameDefine.TOKEN_STORAGE);
#endif
        AudioManager.Instance?.StopMusic();
        this.Machine.ChangeState<LoginState>();
        Release();
    }

}
