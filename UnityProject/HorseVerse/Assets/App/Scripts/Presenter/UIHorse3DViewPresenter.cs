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
    private ObjectHorse3DView objHorse3DView = default;
    private CancellationTokenSource cts = default;

    private MasterHorseContainer masterHorseContainer = default;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private bool isIn = false;
    public UIHorse3DViewPresenter(IDIContainer container)
    {
        this.container = container;    
    }

    public async UniTask ShowHorse3DViewAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await UserDataRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);
        await HorseRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);

        //uiHorse3DView ??= await UILoader.Instantiate<UIHorse3DView>(token: cts.Token);
        //if (!isIn)
        //{
        //    uiHorse3DView.SetEntity(new UIHorse3DView.Entity()
        //    {
        //        horseLoader = new HorseLoader.Entity()
        //        {
        //            horse = MasterHorseContainer.MasterHorseIndexer[HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].MasterHorseId].ModelPath,
        //            color1 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color1,
        //            color2 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color2,
        //            color3 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color3,
        //            color4 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color4,
        //        }
        //    });
        //    uiHorse3DView.transform.SetAsFirstSibling();
        //    uiHorse3DView.In().Forget();
        //    isIn = true;
        //    UserDataRepository.OnModelUpdate += UserDataRepositoryOnModelUpdate;
        //}
        objHorse3DView ??= await ObjectLoader.Instantiate<ObjectHorse3DView>("Object", ObjectHolder.Holder, token: cts.Token);
        if (!isIn)
        {
            objHorse3DView.SetEntity(new ObjectHorse3DView.Entity()
            {
                horseLoader = new HorseObjectLoader.Entity()
                {
                    horse = MasterHorseContainer.MasterHorseIndexer[HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].MasterHorseId].ModelPath,
                    color1 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color1,
                    color2 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color2,
                    color3 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color3,
                    color4 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color4,
                }
            });
            objHorse3DView.transform.SetAsFirstSibling();
            objHorse3DView.In().Forget();
            isIn = true;
            UserDataRepository.OnModelUpdate += UserDataRepositoryOnModelUpdate;
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
        //return uiHorse3DView?.Out() ?? UniTask.CompletedTask;
        return objHorse3DView?.Out() ?? UniTask.CompletedTask;
    }

    private void UserDataRepositoryOnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.after.CurrentHorseNftId != model.before.CurrentHorseNftId)
        {
            UpdateMode().Forget();
        }
    }

    private async UniTask UpdateMode()
    {
        var userHorse = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId];
        var masterHorse = MasterHorseContainer.MasterHorseIndexer[userHorse.MasterHorseId];
        //uiHorse3DView.entity.horseLoader = new HorseLoader.Entity()
        //{
        //    horse = masterHorse.ModelPath,
        //    color1 = userHorse.Color1,
        //    color2 = userHorse.Color2,
        //    color3 = userHorse.Color3,
        //    color4 = userHorse.Color4,
        //};
        //uiHorse3DView.horseLoader.SetEntity(uiHorse3DView.entity.horseLoader);
        //await uiHorse3DView.In();

        objHorse3DView.entity.horseLoader = new HorseObjectLoader.Entity()
        {
            horse = masterHorse.ModelPath,
            color1 = userHorse.Color1,
            color2 = userHorse.Color2,
            color3 = userHorse.Color3,
            color4 = userHorse.Color4,
        };
        objHorse3DView.horseLoader.SetEntity(objHorse3DView.entity.horseLoader);
        await objHorse3DView.In();
    }

    public void Dispose()
    {
        HideHorse3DViewAsync().Forget();
        //UILoader.SafeRelease(ref uiHorse3DView);
        ObjectLoader.SafeRelease("Object", ref objHorse3DView);
    }
}
