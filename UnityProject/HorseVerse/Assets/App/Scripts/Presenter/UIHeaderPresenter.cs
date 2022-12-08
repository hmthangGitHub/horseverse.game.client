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
    public IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    public event Action OnBack = ActionUtility.EmptyAction.Instance;

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
                maxEnergy = UserDataRepository.Current.MaxEnergy,
                userName = UserDataRepository.Current.UserName,
                backBtn = new ButtonComponent.Entity(() => OnBack())
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

    public void SetTitle(string title)
    {
        uiHeader.SetTitle(title);
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UserDataRepository.OnModelUpdate -= OnModelUpdate;
        UILoader.SafeRelease(ref uiHeader);
    }
}
