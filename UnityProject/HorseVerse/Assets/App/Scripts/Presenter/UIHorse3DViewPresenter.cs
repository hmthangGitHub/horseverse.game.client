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

    public async UniTask ShowHorse3DViewAsync(int backgroundType = 0)
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await UserDataRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);
        await UniTask.WaitUntil(()=> UserDataRepository.Current != default).AttachExternalCancellation(cts.Token);
        await HorseRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);

        objHorse3DView ??= await ObjectLoader.Instantiate<ObjectHorse3DView>("Object", ObjectHolder.Holder, token: cts.Token);
        if (!isIn)
        {
            objHorse3DView.SetEntity(new ObjectHorse3DView.Entity()
            {
                horseLoader = new HorseObjectLoader.Entity()
                {
                    horse = MasterHorseContainer.FromTypeToMasterHorse(HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Type).ModelPath,
                    color1 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color1,
                    color2 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color2,
                    color3 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color3,
                    color4 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color4,
                    backgroundType = backgroundType,
                }
            });
            objHorse3DView.transform.SetAsFirstSibling();
            objHorse3DView.In(backgroundType).Forget();
            if(backgroundType == 0)
                MainMenuCameraController.Instance.SetPosition(1);
            else
                MainMenuCameraController.Instance.SetPosition(0);
            isIn = true;
            UserDataRepository.OnModelUpdate += UserDataRepositoryOnModelUpdate;
        }
        else
        {
            objHorse3DView.horseLoader.SetBackgroundType(backgroundType);
            if (backgroundType == 0)
                MainMenuCameraController.Instance.SetPosition(1);
            else
                MainMenuCameraController.Instance.SetPosition(0);
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
        return objHorse3DView?.Out() ?? UniTask.CompletedTask;
    }

    private void UserDataRepositoryOnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.after.CurrentHorseNftId != model.before.CurrentHorseNftId)
        {
            UpdateMode(objHorse3DView.entity.horseLoader.backgroundType).Forget();
        }
    }

    private async UniTask UpdateMode(int backgroundType = 0)
    {
        var userHorse = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId];
        var masterHorse = MasterHorseContainer.FromTypeToMasterHorse(userHorse.Type);

        objHorse3DView.entity.horseLoader = new HorseObjectLoader.Entity()
        {
            horse = masterHorse.ModelPath,
            color1 = userHorse.Color1,
            color2 = userHorse.Color2,
            color3 = userHorse.Color3,
            color4 = userHorse.Color4,
            backgroundType = backgroundType,
        };
        objHorse3DView.horseLoader.SetEntity(objHorse3DView.entity.horseLoader);
        await objHorse3DView.In(backgroundType);
        if (backgroundType == 0)
            MainMenuCameraController.Instance.SetPosition(1);
        else
            MainMenuCameraController.Instance.SetPosition(0);
    }

    public void Dispose()
    {
        HideHorse3DViewAsync().Forget();
        ObjectLoader.SafeRelease("Object", ref objHorse3DView);
    }
}
