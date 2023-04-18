using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public class UIHorse3DViewPresenter : IDisposable
{
    private const int PlatformNumber = 2;
    private IDIContainer container = default;
    private IUserDataRepository userDataRepository = null;
    private IUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IUserDataRepository>();
    private IReadOnlyHorseRepository horseRepository = null;
    private UITouchDisablePresenter uiTouchDisablePresenter = null;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private UITouchDisablePresenter UITouchDisablePresenter => uiTouchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();

    private UIHorse3DView uiHorse3DView = default;
    private ObjectHorse3DView objHorse3DView = default;
    private CancellationTokenSource cts = default;

    private MasterHorseContainer masterHorseContainer = default;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private bool isIn = false;
    private bool isShowPlatform;
    private UniTaskCompletionSource changeHorseTask;
    public event Action OnTouchHorseEvent = ActionUtility.EmptyAction.Instance;
    private int platformIndex = -1;
    private long currentHorseID = -1;
    public UIHorse3DViewPresenter(IDIContainer container)
    {
        this.container = container;    
    }

    public async UniTask ShowHorse3DViewAsync( int backgroundType = 0, bool isShowPlatform = false, bool isRotateEnable = true, MainMenuCameraType.CameraType cameraType = MainMenuCameraType.CameraType.MainMenu)
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
                    //horse = MasterHorseContainer.FromTypeToMasterHorse(HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].HorseType).ModelPath,
                    horse = MasterHorseContainer.MasterHorseIndexer[HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].HorseMasterID].ModelPath,
                    //color1 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color1,
                    //color2 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color2,
                    //color3 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color3,
                    //color4 = HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Color4,
                    backgroundType = backgroundType,
                },
                horseTouchAction = () => OnTouchHorseEvent.Invoke()
            });
            
            objHorse3DView.transform.SetAsFirstSibling();
            objHorse3DView.In(backgroundType).Forget();
            isIn = true;
            UserDataRepository.OnModelUpdate += UserDataRepositoryOnModelUpdate;
        }

        this.isShowPlatform = isShowPlatform;
        SetPlatformIndex();
        objHorse3DView.horseLoader.SetBackgroundType(backgroundType);
        objHorse3DView.mainMenuCameraType.SetCameraType(cameraType);
        objHorse3DView.SetRotateEnable(isRotateEnable);
    }

    private void SetPlatformIndex()
    {
        if (platformIndex < 0)
        { 
            platformIndex = UnityEngine.Random.Range(0, PlatformNumber);
        }
        objHorse3DView.SetPlatformIndex(isShowPlatform ? platformIndex : -1);
    }
    
    public void SetRotateEnable(bool isRotateEnable)
    {
        objHorse3DView.SetRotateEnable(isRotateEnable);
    }

    public void ChangeCameraType(MainMenuCameraType.CameraType cameraType)
    {
        objHorse3DView.mainMenuCameraType.SetCameraType(cameraType);
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
        var masterHorse = MasterHorseContainer.MasterHorseIndexer[userHorse.HorseMasterID];

        objHorse3DView.entity.horseLoader = new HorseObjectLoader.Entity()
        {
            horse = masterHorse.ModelPath,
            //color1 = userHorse.Color1,
            //color2 = userHorse.Color2,
            //color3 = userHorse.Color3,
            //color4 = userHorse.Color4,
            backgroundType = backgroundType,
        };
        objHorse3DView.horseLoader.SetEntity(objHorse3DView.entity.horseLoader);
        await objHorse3DView.In(backgroundType);
        platformIndex = -1;
        SetPlatformIndex();
        changeHorseTask?.TrySetResult();
    }

    public void Dispose()
    {
        HideHorse3DViewAsync().Forget();
        ObjectLoader.SafeRelease("Object", ref objHorse3DView);
    }

    public async UniTaskVoid ChangeHorseOnSwipe(int direction)
    {
        if (HorseRepository.Models.Count == 0 || HorseRepository.Models.Count == 1) return;
        var nextHorse = GetNextHorse(direction);
        if (nextHorse == currentHorseID) return;

        await UITouchDisablePresenter.ShowTillFinishTaskAsync(UniTask.Create(async () =>
        {
            var nftHorseId = GetNextHorse(direction);
            if (nftHorseId == UserDataRepository.Current.CurrentHorseNftId) return;
            await objHorse3DView.PlayHorizontalAnimation(direction);
            changeHorseTask = new UniTaskCompletionSource();
            UserDataRepository.UpdateHorse(nftHorseId)
                              .Forget();
            await changeHorseTask.Task.AttachExternalCancellation(cts.Token);
            changeHorseTask = default;
            await objHorse3DView.InFromOppositeDirectionAnimation(direction);
        }));
    }

    private long GetNextHorse(int direction)
    {
        var allHorsesNftIds = HorseRepository.Models.Keys.ToList();
        var currentHorseIndex = allHorsesNftIds.FindIndex(x => x == UserDataRepository.Current.CurrentHorseNftId);
        currentHorseIndex = (currentHorseIndex + direction + allHorsesNftIds.Count) % allHorsesNftIds.Count;
        return allHorsesNftIds[currentHorseIndex];
    }
}
