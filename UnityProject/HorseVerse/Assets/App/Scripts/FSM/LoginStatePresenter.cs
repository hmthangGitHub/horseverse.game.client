using System;
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
    private UILoginOTP uiLoginOTP;
    private UILogin uiLogin;
    private UILoadingPresenter uiLoadingPresenter;
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ??=container.Inject<UILoadingPresenter>();

    private UserDataRepository userDataRepository;
    private UserDataRepository UserDataRepository => userDataRepository ??= container.Inject<UserDataRepository>();

    public LoginStatePresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ConnectAndLoginAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
#if UNITY_WEBGL || WEB_SOCKET
        string host = "ws://tcp.prod.game.horsesoflegends.com";
        int port = 8669;
#else
        string host = "tcp.prod.game.horsesoflegends.com";
        int port = 8670;
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
        UILoader.SafeRelease(ref uiSV);
#endif

#if UNITY_WEBGL || WEB_SOCKET
        await SocketClient.Connect(host, port);
#else
        await SocketClient.Connect(host, port);
#endif
        var kq = await doLoginWithAccessToken();

        UILoadingPresenter.HideLoading();
        if (kq == false)
        {
            int type = 0;
            bool res = false;

            while (!res)
            {
                type = await doSelectLoginType();
                if (type == 1)
                    res = await doLoginWithOTP();
                else
                    res = await doLoginWithEmail();
            }
        }
    }

    private async UniTask LoginAsync()
    {
        var res = await SocketClient.Send<LoginRequest, LoginResponse>(new LoginRequest()
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
        await HandleLoginResponse(res);
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

    private async UniTask<bool> doLoginWithAccessToken()
    {
        var token = PlayerPrefs.GetString("USER_LOGIN_ACCESS_TOKEN", "");
        if (!string.IsNullOrEmpty(token))
        {
            var res = await SocketClient.Send<LoginRequest, LoginResponse>(new LoginRequest()
            {
                LoginType = LoginType.AccessToken,
                ClientInfo = new io.hverse.game.protogen.ClientInfo()
                {
                    AccessToken = token,
                    Platform = GetCurrentPlatform(),
                    DeviceId = SystemInfo.deviceUniqueIdentifier,
                    Model = SystemInfo.deviceModel,
                    Version = Application.version,
                }
            });
            await HandleLoginResponse(res);
            if(res.ResultCode == 100)
                return true;
        }
        return false;
    }

    private async UniTask<int> doSelectLoginType()
    {
        var ui = await UILoader.Instantiate<UILoginSelection>(token: cts.Token);
        bool wait = true;
        int type = 0;
        ui.SetEntity(new UILoginSelection.Entity()
        {
            emailLoginBtn = new ButtonComponent.Entity(() => { type = 0; wait = false; }),
            otpLoginBtn = new ButtonComponent.Entity(() => { type = 1; wait = false; })
        });
        await ui.In();
        await UniTask.WaitUntil(() => wait == false);
        UILoader.SafeRelease(ref ui);
        return type;
    }

    private async UniTask<bool> doLoginWithEmail()
    {
        uiLogin = await UILoader.Instantiate<UILogin>(token: cts.Token);
        bool closed = false;
        uiLogin.SetEntity(new UILogin.Entity()
        {
            id = new UIComponentInputField.Entity(),
            passWord = new UIComponentInputField.Entity(),
            loginBtn = new ButtonComponent.Entity(() =>
            {
                LoginAsync().Forget();
                closed = true;
            })
        });
        uiLogin.In().Forget();
        await UniTask.WaitUntil(() => closed == true);
        return true;
    }

    private async UniTask<bool> doLoginWithOTP()
    {
        uiLoginOTP = await UILoader.Instantiate<UILoginOTP>(token: cts.Token);
        bool closed = false;
        bool result = false;
        uiLoginOTP.SetEntity(new UILoginOTP.Entity()
        {
            id = new UIComponentInputField.Entity(),
            code = new UIComponentInputField.Entity(),
            loginBtn = new ButtonComponent.Entity(() => { LoginOTPAsync().Forget(); closed = true; result = true; }),
            cancelBtn = new ButtonComponent.Entity(() => { closeOTP().Forget(); closed = true; result = false; }),
            getCodeBtn = new ButtonComponent.Entity(() => GetCodeOTPAsync().Forget())
        });
        await uiLoginOTP.In();
        await UniTask.WaitUntil(() => closed == true);
        return result;
    }

    private async UniTask closeOTP()
    {
        await uiLoginOTP.Out();
        UILoader.SafeRelease(ref uiLoginOTP);
        uiLoginOTP = default;
    }

    private async UniTask LoginOTPAsync()
    {
        var res = await SocketClient.Send<LoginRequest, LoginResponse>(new LoginRequest()
        {
            LoginType = LoginType.LoginEmailCode,
            ClientInfo = new io.hverse.game.protogen.ClientInfo()
            {
                Email = uiLoginOTP.id.inputField.text,
                EmailCode = uiLoginOTP.code.inputField.text,
                Platform = GetCurrentPlatform(),
                DeviceId = SystemInfo.deviceUniqueIdentifier,
                Model = SystemInfo.deviceModel,
                Version = Application.version,
            }
        });
        await HandleLoginResponse(res);
    }

    private async UniTask GetCodeOTPAsync()
    {
        if (SocketClient == null) Debug.Log("socket is null");
        else Debug.Log("Try to get code");
        var res = await SocketClient.Send<EmailCodeRequest, EmailCodeResponse>(new EmailCodeRequest()
        {
            Email = uiLoginOTP.id.inputField.text,
            ClientInfo = new io.hverse.game.protogen.ClientInfo()
            {
                Email = uiLoginOTP.id.inputField.text,
                Password = "",
                Platform = GetCurrentPlatform(),
                DeviceId = SystemInfo.deviceUniqueIdentifier,
                Model = SystemInfo.deviceModel,
                Version = Application.version,
            }
        });
        if (res != null && res.ResultCode == 100)
        {
            var uiConfirm = await UILoader.Instantiate<UIPopupMessage>(token: cts.Token);
            bool wait = true;

            uiConfirm.SetEntity(new UIPopupMessage.Entity()
            {
                title = "SUCCEED",
                message = "Code sent successfully, please check your mailbox",
                confirmBtn = new ButtonComponent.Entity(() => { wait = false; })
            });
            await uiConfirm.In();
            await UniTask.WaitUntil(() => wait == false);
            UILoader.SafeRelease(ref uiConfirm);
        }
    }

    private async UniTask HandleLoginResponse(LoginResponse res)
    {
        if (res?.ResultCode == 100)
        {
            var model = new UserDataModel()
            {
                Coin = res.PlayerInfo.Chip,
                Energy = res.PlayerInfo.Energy,
                CurrentHorseNftId = res.PlayerInfo.CurrentHorse.NftId,
                MaxEnergy = 100,
                UserId = res.PlayerInfo.Id,
                UserName = res.PlayerInfo.Name,
                Exp = 0,
                Level = 1,
                NextLevelExp = 1,
                TraningTimeStamp = 0,
            };
            await UserDataRepository.UpdateDataAsync(new UserDataModel[] { model });
            PlayerPrefs.SetString("USER_LOGIN_ACCESS_TOKEN", res.PlayerInfo.AccessToken);
        }
        else
        {
            throw new Exception("Login Failed");
        }
    }
}