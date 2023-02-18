using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UIHeaderPresenter : IDisposable
{
    private UIHeader uiHeader;
    private CancellationTokenSource cts;
    private IDIContainer container;
    private IReadOnlyUserDataRepository userDataRepository = default;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    public event Action OnBack = ActionUtility.EmptyAction.Instance;
    public event Action OnLogOut = ActionUtility.EmptyAction.Instance;

    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();

    private UIPopUpSettings uiSetting;


    public UIHeaderPresenter(IDIContainer container)
    {
        this.container = container;
        UserDataRepository.OnModelUpdate += OnModelUpdate;
    }

    private void OnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.after.Coin != model.before?.Coin)
        {
            uiHeader?.coin.SetEntity(model.after.Coin);
        }
    }

    public async UniTask ShowHeaderAsync(bool showBackBtn = true, string title = "")
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await UserDataRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);
        await InstantiateUIIfNeed();
        ShowBackBtn(showBackBtn);
        SetTitle(title);
        uiHeader.In().Forget();
    }

    private async UniTask InstantiateUIIfNeed()
    {
        if(uiHeader == default)
        {
            uiHeader = await UILoader.Instantiate<UIHeader>(UICanvas.UICanvasType.Header, token: cts.Token);
            uiHeader.SetEntity(new UIHeader.Entity()
            {
                coin = UserDataRepository.Current.Coin,
                energy = UserDataRepository.Current.Energy,
                maxEnergy = UserSettingLocalRepository.MasterDataModel.MaxHappinessNumber,
                userName = UserDataRepository.Current.UserName,
                backBtn = new ButtonComponent.Entity(() => OnBack()),
                settingBtn = new ButtonComponent.Entity(() => OnSetting().Forget())
            });
        }
    }

    public void HideHeader()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        uiHeader?.Out().Forget();
    }

    public void ShowBackBtn(bool showBackBtn)
    {
        uiHeader.SetVisibleBackBtn(showBackBtn);
    }

    private void SetTitle(string title)
    {
        uiHeader.SetTitle(title);
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UserDataRepository.OnModelUpdate -= OnModelUpdate;
        UILoader.SafeRelease(ref uiSetting);
        UILoader.SafeRelease(ref uiHeader);
       
    }

    private async UniTask OnSetting()
    {
        var ucs = new UniTaskCompletionSource();
        uiSetting ??= await UILoader.Instantiate<UIPopUpSettings>(UICanvas.UICanvasType.PopUp, token: cts.Token);
        bool _LogOut = false;
        uiSetting.SetEntity(new UIPopUpSettings.Entity()
        {
            closeBtn = new ButtonComponent.Entity(()=> { ucs.TrySetResult(); }),
            logOutBtn = new ButtonComponent.Entity(()=> { ucs.TrySetResult(); _LogOut = true; }),
            bgmSlider = new UIComponentProgressBar.Entity()
            {
                progress = SoundController.GetBGMVolume(),
                OnChangeValue = UpdateBGM,
            },
            gfxSlider = new UIComponentProgressBar.Entity
            {
                progress = SoundController.GetGFXVolume(),
                OnChangeValue = UpdateGFX,
            },
            sfxSlider = new UIComponentProgressBar.Entity
            {
                progress = SoundController.GetSFXVolume(),
                OnChangeValue = UpdateSFX,
            },
        });
        await uiSetting.In();
        await ucs.Task;
        await uiSetting.Out();
        if (_LogOut) LogOut();
    }

    private void UpdateBGM(float f)
    {
        SoundController.SetBGMVolume(f);
    }

    private void UpdateSFX(float f)
    {
        SoundController.SetSFXVolume(f);
    }

    private void UpdateGFX(float f)
    {
        SoundController.SetGFXVolume(f);
    }

    private void LogOut()
    {
        OnLogOut();
    }
}
