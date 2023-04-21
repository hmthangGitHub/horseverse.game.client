using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class HorseRaceThirdPersonManager : IHorseRaceManager
{
    private const float MPerSecondToKmPerHour = 3.6f;
    private readonly IDIContainer container;
    private string mapSettingsPath;
    private MapSettings mapSettings;
    private TargetGenerator targetGenerator;
    private bool isStarted;
    private HorseRaceThirdPersonListContainer horseRaceThirdPersonListContainer;
    private CancellationTokenSource cts;
    private UIHorseRacingController uiHorseRacingController;
    private UIHorseRacingPreGameTiming uiHorseRacingPreGameTiming;
    private UIHorseRacingThirdPersonResult uiHorseRacingThirdPersonResult;
    
    private UIHorseRacingTimingType.TimingType timingType;
    private HorseRaceFirstPersonPlayerController playerController;
    private HorseRaceThirdPersonBehaviour playerHorseRaceThirdPersonBehaviour;
    private UITouchDisablePresenter touchDisablePresenter;
    private HorseRaceThirdPersonInfo[] horseRaceThirdPersonInfo;
    private MasterHorseContainer masterHorseContainer;
    public Transform WarmUpTarget => horseRaceThirdPersonListContainer.WarmUpTarget;
    public IHorseRaceInGameStatus[] HorseControllers => HorseRaceThirdPersonBehaviours;
    private HorseRaceThirdPersonBehaviour[] HorseRaceThirdPersonBehaviours { get; set; }
    public float NormalizedRaceTime { get; private set; }
    public int PlayerHorseIndex { get; private set; }
    public event Action OnHorseFinishTrackEvent = ActionUtility.EmptyAction.Instance;
    public event Action OnShowResult = ActionUtility.EmptyAction.Instance;
    private UITouchDisablePresenter TouchDisablePresenter => touchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private float totalSecond;

    public HorseRaceThirdPersonManager(IDIContainer container)
    {
        this.container = container;
    }
    
    public async UniTask WaitToStart()
    {
        var ucs = new UniTaskCompletionSource();
        await TouchDisablePresenter.Delay(0.5f);
        void OnTiming(UIHorseRacingTimingType.TimingType timingType)
        {
            this.timingType = timingType;
            ucs.TrySetResult();
        }
        uiHorseRacingPreGameTiming.SetEntity(new UIHorseRacingPreGameTiming.Entity()
        {
            timingType = OnTiming
        });
        await uiHorseRacingPreGameTiming.In().AttachExternalCancellation(cts.Token);
        uiHorseRacingPreGameTiming.StartTiming();
        await ucs.Task.AttachExternalCancellation(cts.Token);
        await uiHorseRacingPreGameTiming.Out().AttachExternalCancellation(cts.Token);
    }

    public void PrepareToRace()
    {
        SetHorsesVisible(true);
    }

    public void StartRace()
    {
        isStarted = true;
        HorseRaceThirdPersonBehaviours.ForEach(x =>
        {
            var horseTimmingType = x.IsPlayer
                ? this.timingType
                : (UIHorseRacingTimingType.TimingType)UnityEngine.Random.Range((int)UIHorseRacingTimingType.TimingType.Great,
                    (int)UIHorseRacingTimingType.TimingType.Perfect + 1);
            x.StartRace(GetNormalizeSpeedBaseOnTimingType(horseTimmingType), timingType == UIHorseRacingTimingType.TimingType.Perfect);
        });
        uiHorseRacingController.SetEntity(new UIHorseRacingController.Entity()
        {
            cameraBtn = new UIComponentHoldImageBehavior.Entity()
            {
                onDown = () => ChangeBackCamera(true),
                onUp = () => ChangeBackCamera(false)
            },
            sprintBtn = new ButtonComponent.Entity(playerController.Sprint),
            sprintCharge = new UIHorseRacingSprintCharge.Entity()
            {
                progress = 0.0f,
                chargeNumber = playerHorseRaceThirdPersonBehaviour.SprintChargeNumber
            },
            currentLap = 1,
            currentPosition = 8,
            maxLap = 3,
            maxPosition = 8,
        });
        uiHorseRacingController.In().Forget();
    }

    private void ChangeBackCamera(bool isBackCamera)
    {
        playerHorseRaceThirdPersonBehaviour.EnableCamera(isBackCamera);
    }

    private float GetNormalizeSpeedBaseOnTimingType(UIHorseRacingTimingType.TimingType type)
    {
        return type switch
        {
            UIHorseRacingTimingType.TimingType.None => 0.7f,
            UIHorseRacingTimingType.TimingType.Bad => 0.7f,
            UIHorseRacingTimingType.TimingType.Good => 1.0f,
            UIHorseRacingTimingType.TimingType.Great => 1.5f,
            UIHorseRacingTimingType.TimingType.Perfect => 2.0f,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void UpdateRaceTime()
    {
        if (!isStarted) return;
        uiHorseRacingController.SetSprintTime(playerHorseRaceThirdPersonBehaviour.CurrentSprintNormalizeTime);
        uiHorseRacingController.sprintCharge.SetProgress(playerHorseRaceThirdPersonBehaviour.CurrentChargeNormalize);
        uiHorseRacingController.sprintBtn.SetInteractable(playerHorseRaceThirdPersonBehaviour.IsAbleToSprint);
        uiHorseRacingController.SetCurrentLap(playerHorseRaceThirdPersonBehaviour.CurrentLap);
        uiHorseRacingController.SetCurrentPosition(GetCurrentPlayerPosition());
        uiHorseRacingController.speed.SetEntity(Mathf.Round(playerHorseRaceThirdPersonBehaviour.CurrentForwardSpeed * MPerSecondToKmPerHour));
        totalSecond += Time.deltaTime;
        uiHorseRacingController.totalSecond.SetEntity((int)totalSecond);
    }

    private int GetCurrentPlayerPosition()
    {
        return HorseControllers.OrderByDescending(x => x.CurrentRaceProgressWeight)
                               .Select((x,i) => (horse: x, position: i + 1))
                               .First(x => x.horse == playerHorseRaceThirdPersonBehaviour)
                               .position;
    }

    public async UniTask InitializeAsync(MasterHorseContainer masterHorseContainer,
                                         string mapSettingPath,
                                         int playerHorseIndex,
                                         HorseRaceThirdPersonInfo[] horseRaceThirdPersonInfo,
                                         CancellationToken token)
    {
        PlayerHorseIndex = playerHorseIndex;
        this.mapSettingsPath = mapSettingPath;
        await LoadMapSettingsAsync(token);
        await LoadUIAsync(token);
        await LoadAllHorsesAsync(masterHorseContainer, horseRaceThirdPersonInfo, token);
        SubscribeFinishTrackEvents();
        cts = new CancellationTokenSource();
    }

    private void SubscribeFinishTrackEvents()
    {
        playerHorseRaceThirdPersonBehaviour.OnFinishRace += FinishTrackEvent;
    }

    private void FinishTrackEvent()
    {
        FinishTrackEventAsync().Forget();
    }

    private async UniTaskVoid FinishTrackEventAsync()
    {
        var ucs = new UniTaskCompletionSource();
        uiHorseRacingThirdPersonResult.SetEntity(new UIHorseRacingThirdPersonResult.Entity()
        {
            position = GetCurrentPlayerPosition(),
            raceTime = (int)totalSecond,
            outerBtn = new ButtonComponent.Entity(() =>
            {
                ucs.TrySetResult();
            }),
            realm = new UIComponentRealmIntro.Entity()
            {
                realm = "FORGOTTEN REALM",
                track = "The Old Observatory"
            },
            timeOut = 10.0f,
        });
        HorseRaceThirdPersonBehaviours.ForEach(x => x.SetVisibleLane(false));
        await uiHorseRacingController.Out().AttachExternalCancellation(cts.Token);
        await uiHorseRacingThirdPersonResult.In().AttachExternalCancellation(cts.Token);
        await ucs.Task.AttachExternalCancellation(cts.Token);
        await uiHorseRacingThirdPersonResult.Out().AttachExternalCancellation(cts.Token);
        OnShowResult.Invoke();
    }

    private void UnsubscribeFinishTrackEvents()
    {
        playerHorseRaceThirdPersonBehaviour.OnFinishRace -= FinishTrackEvent;
    }

    private async UniTask LoadAllHorsesAsync(MasterHorseContainer masterHorseContainer,
                                             HorseRaceThirdPersonInfo[] horseRaceThirdPersonInfo,
                                             CancellationToken token)
    {
        await InstantiateHorseListAsync(horseRaceThirdPersonInfo, token);
        GetPlayerHorseControllerAndBehaviour();
        SetHorsesVisible(false);
    }

    private void GetPlayerHorseControllerAndBehaviour()
    {
        playerHorseRaceThirdPersonBehaviour = HorseRaceThirdPersonBehaviours.First(x => x.IsPlayer);
        playerController = playerHorseRaceThirdPersonBehaviour.GetComponentInChildren<HorseRaceFirstPersonPlayerController>();
        horseRaceThirdPersonListContainer.HorseRaceFirstPersonPlayerController = playerController;
    }

    private async UniTask InstantiateHorseListAsync(HorseRaceThirdPersonInfo[] horseRaceThirdPersonInfo,
                                                 CancellationToken token)
    {
        this.horseRaceThirdPersonInfo = horseRaceThirdPersonInfo;
        horseRaceThirdPersonListContainer = Object.Instantiate(
            (await Resources.LoadAsync<HorseRaceThirdPersonListContainer>("GamePlay/HorseRaceThirdPersonManager") as
                HorseRaceThirdPersonListContainer));
        HorseRaceThirdPersonBehaviours = await horseRaceThirdPersonInfo.Select((x, i) => LoadHorseController(i, x, token));
    }

    private async UniTask LoadUIAsync(CancellationToken token)
    {
        uiHorseRacingPreGameTiming = await UILoader.Instantiate<UIHorseRacingPreGameTiming>(token: token);
        uiHorseRacingController = await UILoader.Instantiate<UIHorseRacingController>(token: token);
        uiHorseRacingThirdPersonResult = await UILoader.Instantiate<UIHorseRacingThirdPersonResult>(token: token);
    }

    private void SetHorsesVisible(bool isVisible)
    {
        horseRaceThirdPersonListContainer.gameObject.SetActive(isVisible);
    }

    private async UniTask LoadMapSettingsAsync(CancellationToken token)
    {
        mapSettings = await PrimitiveAssetLoader.LoadAssetAsync<MapSettings>(mapSettingsPath, token);
        targetGenerator = Object.Instantiate(mapSettings.targetGenerator, Vector3.zero, Quaternion.identity);
    }

    private async UniTask<HorseRaceThirdPersonBehaviour> LoadHorseController(int initialLane, HorseRaceThirdPersonInfo horseRaceThirdPersonInfo, CancellationToken token)
    {
        var horse = await HorseMeshAssetLoader.InstantiateHorse(MasterHorseContainer.GetHorseMeshInformation(horseRaceThirdPersonInfo.MeshInformation, HorseModelMode.RaceThirdPerson) , token);
        horse.transform.SetParent(horseRaceThirdPersonListContainer.HorsesContainer);
        var horseController = horse.GetComponent<HorseRaceThirdPersonBehaviour>();
        horseRaceThirdPersonListContainer.HorseGroup.AddMember(horseController.transform, 1, 0);
        horseController.HorseRaceThirdPersonData = new HorseRaceThirdPersonData()
        {
            TargetGenerator = targetGenerator,
            InitialLane = initialLane,
            IsPlayer = PlayerHorseIndex == initialLane,
            PredefineWayPoints = targetGenerator.GenerateRandomTargetsWithNoise(initialLane),
            HorseRaceThirdPersonStats = horseRaceThirdPersonInfo.HorseRaceThirdPersonStats,
            Camera = horseRaceThirdPersonListContainer.CamreraTransform,
            Name = horseRaceThirdPersonInfo.Name,
        };
        return horseController;
    }
    
    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UnsubscribeFinishTrackEvents();
        DisposeUtility.SafeDisposeMonoBehaviour(ref horseRaceThirdPersonListContainer);
        HorseRaceThirdPersonBehaviours = default;
        
        horseRaceThirdPersonInfo.ForEach(x =>
        {
            PrimitiveAssetLoader.UnloadAssetAtPath(MasterHorseContainer
                                                   .GetHorseMeshInformation(x.MeshInformation, HorseModelMode.RaceThirdPerson).horseModelPath);
        });
        PrimitiveAssetLoader.UnloadAssetAtPath(mapSettingsPath);
        UILoader.SafeRelease(ref uiHorseRacingPreGameTiming);
        UILoader.SafeRelease(ref uiHorseRacingController);
        UILoader.SafeRelease(ref uiHorseRacingThirdPersonResult);
        DisposeUtility.SafeDisposeMonoBehaviour(ref targetGenerator);
        Time.timeScale = 1;
        
        OnHorseFinishTrackEvent = ActionUtility.EmptyAction.Instance;
        OnShowResult = ActionUtility.EmptyAction.Instance;
        timingType = default;
        playerController = default;
        playerHorseRaceThirdPersonBehaviour = default;
        touchDisablePresenter = default;
        mapSettings = default;
        horseRaceThirdPersonInfo = default;
    }
}
