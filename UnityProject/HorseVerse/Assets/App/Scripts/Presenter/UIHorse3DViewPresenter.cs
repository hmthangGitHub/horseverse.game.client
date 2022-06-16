using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIHorse3DViewPresenter : IDisposable
{
    private IDIContainer container = default;
    private IReadOnlyUserDataRepository userDataRepository = null;
    public IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private UIHorse3DView uiHorse3DView = default;
    private string currentHorsemodelPath;
    private CancellationTokenSource cts = default;

    private MasterHorseContainer masterHorseContainer = default;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private bool isIn = false;
    public UIHorse3DViewPresenter(IDIContainer container)
    {
        this.container = container;    
    }

    public async UniTaskVoid ShowHorse3DViewAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await UserDataRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);
        UserDataRepository.OnModelUpdate += UserDataRepositoryOnModelUpdate;

        if (uiHorse3DView == default)
        {
            uiHorse3DView ??= await UILoader.Load<UIHorse3DView>(token: cts.Token);
            uiHorse3DView.SetEntity(new UIHorse3DView.Entity()
            {
                horseLoader = new HorseLoader.Entity()
                {
                    horse = MasterHorseContainer.MasterHorseIndexer[UserDataRepository.Current.MasterHorseId].ModelPath
                }
            });
        }
        uiHorse3DView.In().Forget();
        isIn = true;
    }

    public void HideHorse3DView()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UserDataRepository.OnModelUpdate -= UserDataRepositoryOnModelUpdate;
        uiHorse3DView.Out().Forget();
        isIn = false;
    }

    private void UserDataRepositoryOnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (isIn && model.after.MasterHorseId != model.before.MasterHorseId)
        {
            uiHorse3DView.SetEntity(new UIHorse3DView.Entity()
            {
                horseLoader = new HorseLoader.Entity()
                {
                    horse = MasterHorseContainer.MasterHorseIndexer[UserDataRepository.Current.MasterHorseId].ModelPath
                }
            });
            uiHorse3DView.In().Forget();
        }
    }

    public void Dispose()
    {
        HideHorse3DView();
        UILoader.SafeUnload(ref uiHorse3DView);
    }
}
