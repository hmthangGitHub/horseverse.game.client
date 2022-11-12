﻿using System;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using UnityEngine;
using ClientInfo = io.hverse.game.protogen.ClientInfo;
using Platform = io.hverse.game.protogen.Platform;

public class LoginStatePresenter : IDisposable
{
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();
    private readonly IDIContainer container;
    private CancellationTokenSource cts;
    private UniTaskCompletionSource ucs;
    private UILogin uiLogin;
    private UILoginOTP uiLoginOTP;
    private UILoadingPresenter uiLoadingPresenter;
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ??=container.Inject<UILoadingPresenter>();

    public LoginStatePresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ConnectAndLoginAsync()
    {
        ucs = new UniTaskCompletionSource();
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
#if UNITY_WEBGL || WEB_SOCKET
        string host = "ws://tcp.prod.game.horsesoflegends.com";
        int port = 8769;
#else
        string host = "tcp.prod.game.horsesoflegends.com";
        int port = 8770;
#endif

#if CUSTOM_SERVER
        var uiSV = await UILoader.Instantiate<UIPopUpServerSelection>(token: cts.Token);
        bool wait = true;
        
        uiSV.SetEntity(new UIPopUpServerSelection.Entity()
        {
            cancelBtn = new ButtonComponent.Entity(()=> { wait = false; }),
            connectBtn = new ButtonComponent.Entity(()=> { host = uiSV.hostInput.inputField.text; port = System.Convert.ToInt32(uiSV.portInput.inputField.text); wait = false; }),
            hostInput = new UIComponentInputField.Entity() { defaultValue = host, interactable = true},
            portInput = new UIComponentInputField.Entity() { defaultValue = port.ToString(), interactable = true },
        });
        uiSV.In().Forget();
        UILoadingPresenter.HideLoading();
        await UniTask.WaitUntil(() => wait == false);
        UILoader.SafeRelease(ref uiLogin);
#endif

#if UNITY_WEBGL || WEB_SOCKET
        await SocketClient.Connect(host, port);
#else
        await SocketClient.Connect(host, port);
#endif

        await doLoginWithOTP();

        uiLogin = await UILoader.Instantiate<UILogin>(token: cts.Token);
        uiLogin.SetEntity(new UILogin.Entity()
        {
            id = new UIComponentInputField.Entity(),
            passWord = new UIComponentInputField.Entity(),
            loginBtn = new ButtonComponent.Entity(() => LoginAsync().Forget())
        });
        uiLogin.In().Forget();
        UILoadingPresenter.HideLoading();
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    private async UniTask LoginAsync()
    {
        await SocketClient.Send<LoginRequest, LoginResponse>(new LoginRequest()
        {
            LoginType = LoginType.Email,
            ClientInfo = new io.hverse.game.protogen.ClientInfo()
            {
                Email = uiLogin.id.inputField.text,
                Password = uiLogin.passWord.inputField.text,
                Platform = GetCurrentPlatform(),
                DeviceId = SystemInfo.deviceUniqueIdentifier,
                Model = SystemInfo.deviceModel,
                Version = Application.version,
            }
        });
        await SocketClient.Send<GameMessage, GameMessage>(new GameMessage()
        {
            MsgType = GameMessageType.PingMessage
        });
        ucs.TrySetResult();
    }

    private io.hverse.game.protogen.Platform GetCurrentPlatform()
    {
#if UNITY_WEBGL
        return io.hverse.game.protogen.Platform.Web;
#endif
#if UNITY_ANDROID
        return io.hverse.game.protogen.Platform.Android;
#endif
#if UNITY_IOS
        return io.hverse.game.protogen.Platform.Ios;
#endif
        return io.hverse.game.protogen.Platform.Web;
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeRelease(ref uiLogin);
        if(uiLoginOTP != default) UILoader.SafeRelease(ref uiLoginOTP);
        socketClient = default;
        uiLoadingPresenter = default;
    }

    private async UniTask doLoginWithOTP()
    {
        uiLoginOTP = await UILoader.Instantiate<UILoginOTP>(token: cts.Token);
        bool closed = false;
        uiLoginOTP.SetEntity(new UILoginOTP.Entity()
        {
            id = new UIComponentInputField.Entity(),
            code = new UIComponentInputField.Entity(),
            loginBtn = new ButtonComponent.Entity(() => { LoginOTPAsync().Forget(); closed = true; }),
            cancelBtn = new ButtonComponent.Entity(() => { closeOTP().Forget(); closed = true; }),
            getCodeBtn = new ButtonComponent.Entity(() => GetCodeOTPAsync().Forget())
        });
        await uiLoginOTP.In();
        await UniTask.WaitUntil(() => closed == true);
    }

    private async UniTask closeOTP()
    {
        await uiLoginOTP.Out();
        UILoader.SafeRelease(ref uiLoginOTP);
        uiLoginOTP = default;
    }

    private async UniTask LoginOTPAsync()
    {
        await SocketClient.Send<LoginRequest, LoginResponse>(new LoginRequest()
        {
            LoginType = LoginType.Email,
            ClientInfo = new io.hverse.game.protogen.ClientInfo()
            {
                Email = uiLogin.id.inputField.text,
                Password = uiLogin.passWord.inputField.text,
                Platform = GetCurrentPlatform(),
                DeviceId = SystemInfo.deviceUniqueIdentifier,
                Model = SystemInfo.deviceModel,
                Version = Application.version,
            }
        });
        await SocketClient.Send<GameMessage, GameMessage>(new GameMessage()
        {
            MsgType = GameMessageType.PingMessage
        });
        ucs.TrySetResult();
    }

    private async UniTask GetCodeOTPAsync()
    {
        await SocketClient.Send<LoginRequest, LoginResponse>(new LoginRequest()
        {
            LoginType = LoginType.Email,
            ClientInfo = new io.hverse.game.protogen.ClientInfo()
            {
                Email = uiLogin.id.inputField.text,
                Password = uiLogin.passWord.inputField.text,
                Platform = GetCurrentPlatform(),
                DeviceId = SystemInfo.deviceUniqueIdentifier,
                Model = SystemInfo.deviceModel,
                Version = Application.version,
            }
        });
        await SocketClient.Send<GameMessage, GameMessage>(new GameMessage()
        {
            MsgType = GameMessageType.PingMessage
        });
        ucs.TrySetResult();
    }
}