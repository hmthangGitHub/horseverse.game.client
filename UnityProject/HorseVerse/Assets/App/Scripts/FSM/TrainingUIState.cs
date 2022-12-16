using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingUIState : InjectedBState
{
    private UIHorseTrainingPresenter presenter;
    private UIHeaderPresenter UIHeaderPresenter => Container.Inject<UIHeaderPresenter>();

    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();

    public override void Enter()
    {
        base.Enter();
        presenter = new UIHorseTrainingPresenter(Container);

        UIHeaderPresenter.OnBack += OnBack;
        UIHeaderPresenter.ShowHeaderAsync(true, "ADVENTURE").Forget();
        UIHeaderPresenter.OnLogOut += OnLogOut;
        presenter.ToTrainingActionState += ToTrainingActionState;
        presenter.ShowUIHorseTrainingAsync().Forget();
    }

    private void ToTrainingActionState()
    {
        this.Machine.ChangeState<TrainingActionState>();
    }

    private void OnBack()
    {
        this.GetSuperMachine<RootFSM>().ChangeToChildStateRecursive<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        Release();
    }

    void Release()
    {
        UIHeaderPresenter.HideHeader();
        UIHeaderPresenter.OnBack -= OnBack;
        presenter.Dispose();
        presenter = null;
    }

    private void OnLogOut()
    {
        OnLogOutAsync().Forget();
    }

    private async UniTask OnLogOutAsync()
    {
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
