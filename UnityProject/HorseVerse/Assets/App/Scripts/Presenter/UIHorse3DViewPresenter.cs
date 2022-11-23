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
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private IReadOnlyHorseRepository horseRepository = null;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();

    private UIHorse3DView uiHorse3DView = default;
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
        await HorseRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);

        uiHorse3DView ??= await UILoader.Instantiate<UIHorse3DView>(token : cts.Token);
        if (!isIn)
        {
            uiHorse3DView.SetEntity(new UIHorse3DView.Entity()
            {
                horseLoader = new HorseLoader.Entity()
                {
                    horse = MasterHorseContainer.MasterHorseIndexer[HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].MasterHorseId].ModelPath
                }
            });
            uiHorse3DView.transform.SetAsFirstSibling();
            uiHorse3DView.In().Forget();
            isIn = true;
            UserDataRepository.OnModelUpdate += UserDataRepositoryOnModelUpdate;

            await UniTask.WaitUntil(() => uiHorse3DView.horseLoader.Horse != null);
            if (uiHorse3DView.horseLoader.Horse != null)
            {
                Debug.Log("SET COLOR");
                var uHorse = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId];
                setColor(uiHorse3DView.horseLoader.Horse, uHorse.Color1, uHorse.Color2, uHorse.Color3, uHorse.Color4);
            }
        }
    }

    public async UniTask HideHorse3DViewAsync()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UserDataRepository.OnModelUpdate -= UserDataRepositoryOnModelUpdate;
        isIn = false;
        await GetOutTask();
    }

    private UniTask GetOutTask()
    {
        return uiHorse3DView?.Out() ?? UniTask.CompletedTask;
    }

    private void UserDataRepositoryOnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.after.CurrentHorseNftId != model.before.CurrentHorseNftId)
        {
            Debug.Log("UPDATE MODEL");
            UpdateMode().Forget();
        }
    }

    private async UniTask UpdateMode()
    {
        uiHorse3DView.entity.horseLoader = new HorseLoader.Entity()
        {
            horse = MasterHorseContainer.MasterHorseIndexer[HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].MasterHorseId].ModelPath
        };
        uiHorse3DView.horseLoader.SetEntity(uiHorse3DView.entity.horseLoader);
        await uiHorse3DView.In();
        await UniTask.WaitUntil(() => uiHorse3DView.horseLoader.Horse != null);
        if(uiHorse3DView.horseLoader.Horse != null)
        {
            Debug.Log("SET COLOR");
            var uHorse = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId];
            setColor(uiHorse3DView.horseLoader.Horse, uHorse.Color1, uHorse.Color2, uHorse.Color3, uHorse.Color4);
        }
    }

    public void Dispose()
    {
        HideHorse3DViewAsync().Forget();
        UILoader.SafeRelease(ref uiHorse3DView);
    }

    private void setColor(GameObject horse, Color c1, Color c2, Color c3, Color c4)
    {
        if (horse != null)
        {
            HorseObjectData data = horse.GetComponent<HorseObjectData>();
            if (data != null)
            {
                HorseObjectPresenter.SetColor(data, c1, c2, c3, c4);
            }
        }
    }
}
