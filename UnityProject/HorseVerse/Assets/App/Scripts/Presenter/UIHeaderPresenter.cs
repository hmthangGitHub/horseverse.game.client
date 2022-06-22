using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIHeaderPresenter : IDisposable
{
    private UIHeader uiHeader;
    private CancellationTokenSource cts;
    private IDIContainer container;
    private IReadOnlyUserDataRepository userDataRepository = default;
    public IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();

    public UIHeaderPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTaskVoid ShowHeaderAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await UserDataRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);
        uiHeader ??= await UILoader.Load<UIHeader>(UICanvas.UICanvasType.Header, token: cts.Token);
        uiHeader.SetEntity(new UIHeader.Entity()
        {
            coin = UserDataRepository.Current.Coin,
            energy = UserDataRepository.Current.Energy,
            maxEnergy = UserDataRepository.Current.MaxEnergy,
            userName = UserDataRepository.Current.UserName,
        });
        uiHeader.In().Forget();
    }

    public void HideHeader()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        uiHeader?.Out().Forget();
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeUnload(ref uiHeader);
    }
}
