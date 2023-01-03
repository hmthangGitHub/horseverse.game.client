using System;
using System.Collections;
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

    private ServerDefine serverDefine;

    public LoginStatePresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ConnectAndLoginAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await doLoadServerSetting();
#if UNITY_WEBGL || WEB_SOCKET
        string host = "ws://tcp.prod.game.horsesoflegends.com";
        int port = 8669;
#else
        string host = "tcp.prod.game.horsesoflegends.com";
        int port = 8670;
        if (serverDefine != default)
        {
            host = serverDefine.Production.Host;
            port = serverDefine.Production.Port;
        }
#endif
        await doInitLocalLocalization();
#if CUSTOM_SERVER
        var uiSV = await UILoader.Instantiate<UIPopUpServerSelection>(token: cts.Token);
        bool wait = true;
        int currentProfileIndex = 0;
        uiSV.SetEntity(new UIPopUpServerSelection.Entity()
        {
            cancelBtn = new ButtonComponent.Entity(()=> { wait = false; currentProfileIndex = uiSV.entity.CurrentProfileIndex; }),
            connectBtn = new ButtonComponent.Entity(()=> { 
                host = uiSV.hostInput.inputField.text; 
                port = System.Convert.ToInt32(uiSV.portInput.inputField.text); 
                wait = false;
                currentProfileIndex = uiSV.entity.CurrentProfileIndex;
            }),
            hostInput = new UIComponentInputField.Entity() { defaultValue = host, interactable = true},
            portInput = new UIComponentInputField.Entity() { defaultValue = port.ToString(), interactable = true },
            CurrentProfileIndex = currentProfileIndex,
            serverDefine = this.serverDefine,
        });
        uiSV.In().Forget();
        UILoadingPresenter.HideLoading();
        await UniTask.WaitUntil(() => wait == false);
        
        UILoader.SafeRelease(ref uiSV);
#endif

#if MULTI_ACCOUNT
        PlayerPrefs.SetString(GameDefine.TOKEN_CURRENT_KEY_INDEX, currentProfileIndex.ToString());
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
        await UniTask.Delay(1000);
    }

    private async UniTask<bool> LoginAsync()
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
        }, 5.0f);
        return await HandleLoginResponse(res);
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
        uiLogin = default;
        uiLoginOTP = default;
        socketClient = default;
        uiLoadingPresenter = default;
    }

    private async UniTask<bool> doLoginWithAccessToken()
    {
#if MULTI_ACCOUNT
        var indexToken = PlayerPrefs.GetString(GameDefine.TOKEN_CURRENT_KEY_INDEX, "");
        var token = PlayerPrefs.GetString(GameDefine.TOKEN_STORAGE + indexToken, "");
#else
        var token = PlayerPrefs.GetString(GameDefine.TOKEN_STORAGE, "");
#endif
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
            }, 10.0f);
            if (res.ResultCode == 100)
            {
                await HandleLoginResponse(res);
                return true;
            }
            else
            {
                return false;
            }
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
        uiLogin ??= await UILoader.Instantiate<UILogin>(token: cts.Token);
        bool closed = false;
        bool result = false;
        uiLogin.SetEntity(new UILogin.Entity()
        {
            id = new UIComponentInputField.Entity(),
            passWord = new UIComponentInputField.Entity(),
            loginBtn = new ButtonComponent.Entity(() =>
            {
                result = true;
                closed = true;
            }),
            cancelBtn = new ButtonComponent.Entity(()=> {
                closeAccount().Forget(); closed = true; result = false;
            }), 
            registerBtn = new ButtonComponent.Entity(()=> { 
                
            })
        });
        uiLogin.In().Forget();
        await UniTask.WaitUntil(() => closed == true);
        if (result)
            return await LoginAsync();
        return false;
    }

    private async UniTask closeAccount()
    {
        await uiLogin.Out();
        UILoader.SafeRelease(ref uiLogin);
        uiLogin = default;
    }

    private async UniTask<bool> doLoginWithOTP()
    {
        uiLoginOTP ??= await UILoader.Instantiate<UILoginOTP>(token: cts.Token);
        bool closed = false;
        bool result = false;
        uiLoginOTP.SetEntity(new UILoginOTP.Entity()
        {
            id = new UIComponentInputField.Entity(),
            code = new UIComponentInputField.Entity(),
            loginBtn = new ButtonComponent.Entity(() => { closed = true; result = true; }),
            cancelBtn = new ButtonComponent.Entity(() => { closeOTP().Forget(); closed = true; result = false; }),
            getCodeBtn = new ButtonComponent.Entity(() => GetCodeOTPAsync().Forget())
        });
        await uiLoginOTP.In();
        await UniTask.WaitUntil(() => closed == true);
        if(result) 
            return await LoginOTPAsync();
        return result;
    }

    private async UniTask closeOTP()
    {
        await uiLoginOTP.Out();
        UILoader.SafeRelease(ref uiLoginOTP);
        uiLoginOTP = default;
    }

    private async UniTask<bool> LoginOTPAsync()
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
        return await HandleLoginResponse(res);
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

    private async UniTask<bool> HandleLoginResponse(LoginResponse res)
    {
        if (res?.ResultCode == 100)
        {
            // GetMasterData 
            var data = await GetMasterData();

            var model = new UserDataModel()
            {
                Coin = res.PlayerInfo.Chip,
                Energy = res.PlayerInfo.Energy,
                CurrentHorseNftId = res.PlayerInfo.CurrentHorse.NftId,
                MaxEnergy = (data != default) ? data.MaxHappinessNumber : 0,
                UserId = res.PlayerInfo.Id,
                UserName = res.PlayerInfo.Name,
                Exp = 0,
                Level = 1,
                NextLevelExp = 1,
                TraningTimeStamp = 0,
            };
            await UserDataRepository.UpdateDataAsync(new UserDataModel[] { model });
#if MULTI_ACCOUNT
            var indexToken = PlayerPrefs.GetString(GameDefine.TOKEN_CURRENT_KEY_INDEX, "");
            PlayerPrefs.SetString(GameDefine.TOKEN_STORAGE + indexToken, res.PlayerInfo.AccessToken);
#else
            PlayerPrefs.SetString(GameDefine.TOKEN_STORAGE, res.PlayerInfo.AccessToken);
#endif
            return true;
        }
        else
        {
            //throw new Exception("Login Failed");
            await ShowMessagePopUp("NOTICE", LanguageManager.GetText($"RESULT_CODE_{res.ResultCode}"));
            return false;
        }
    }

    private async UniTask ShowMessagePopUp(string title, string message)
    {
        var uiConfirm = await UILoader.Instantiate<UIPopupMessage>(token: cts.Token);
        bool wait = true;

        uiConfirm.SetEntity(new UIPopupMessage.Entity()
        {
            title = title,
            message = message,
            confirmBtn = new ButtonComponent.Entity(() => { wait = false; })
        });
        await uiConfirm.In();
        await UniTask.WaitUntil(() => wait == false);
        UILoader.SafeRelease(ref uiConfirm);
    }

    private async UniTask<MasterDataResponse> GetMasterData()
    {
        var res = await SocketClient.Send<MasterDataRequest, MasterDataResponse>(new MasterDataRequest());
        return res;
    }

    IEnumerator doInitLocalLocalization(System.Action onFinish = null)
    {
        string path = $"Localization/Localization";
        ResourceRequest rq = Resources.LoadAsync<LanguageDependentText>(path);
        yield return rq;
        if (rq.asset != null)
        {
            LanguageDependentText depen = rq.asset as LanguageDependentText;
            LanguageManager.InitializeLocal(SystemLanguage.English, depen);
        }
        onFinish?.Invoke();
    }

    IEnumerator doLoadServerSetting()
    {
#if UNITY_WEBGL || WEB_SOCKET
        var rq = Resources.LoadAsync<ServerDefine>("Settings/WSServerDefine");
        yield return rq;
        if(rq.asset != null)
        {
            serverDefine = (ServerDefine)rq.asset;
        }
#else
        var rq = Resources.LoadAsync<ServerDefine>("Settings/TCPServerDefine");
        yield return rq;
        if(rq.asset != null)
        {
            serverDefine = (ServerDefine)rq.asset;
        }
#endif
    }
}