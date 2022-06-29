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
        uiHorse3DView ??= await UILoader.Instantiate<UIHorse3DView>(token : cts.Token);
        if (!isIn)
        {
            uiHorse3DView.SetEntity(new UIHorse3DView.Entity()
            {
                horseLoader = new HorseLoader.Entity()
                {
                    horse = MasterHorseContainer.MasterHorseIndexer[UserDataRepository.Current.MasterHorseId].ModelPath
                }
            });
            uiHorse3DView.transform.SetAsFirstSibling();
            uiHorse3DView.In().Forget();
            isIn = true;
            UserDataRepository.OnModelUpdate += UserDataRepositoryOnModelUpdate;
        }
    }

    public void HideHorse3DView()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UserDataRepository.OnModelUpdate -= UserDataRepositoryOnModelUpdate;
        uiHorse3DView?.Out().Forget();
        isIn = false;
    }

    private void UserDataRepositoryOnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.after.MasterHorseId != model.before.MasterHorseId)
        {
            uiHorse3DView.entity.horseLoader = new HorseLoader.Entity()
            {
                horse = MasterHorseContainer.MasterHorseIndexer[UserDataRepository.Current.MasterHorseId].ModelPath
            };
            uiHorse3DView.horseLoader.SetEntity(uiHorse3DView.entity.horseLoader);
        }
    }

    public void Dispose()
    {
        HideHorse3DView();
        UILoader.SafeRelease(ref uiHorse3DView);
    }
}
