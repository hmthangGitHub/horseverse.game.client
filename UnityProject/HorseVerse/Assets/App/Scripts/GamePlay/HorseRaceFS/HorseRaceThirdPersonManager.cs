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
    private readonly IDIContainer container;
    private string mapSettingsPath;
    private MapSettings mapSettings;
    private TargetGenerator targetGenerator;
    private bool isStarted;
    private HorseRaceThirdPersonListContainer horseRaceThirdPersonListContainer;
    private CancellationTokenSource cts;
    private UIHorseRacingController uiHorseRacingController;
    private UIHorseRacingPreGameTiming uiHorseRacingPreGameTiming;
    public Transform WarmUpTarget => horseRaceThirdPersonListContainer.WarmUpTarget;
    public IHorseRaceInGameStatus[] HorseControllers => HorseRaceThirdPersonBehaviours;
    private HorseRaceThirdPersonBehaviour[] HorseRaceThirdPersonBehaviours { get; set; }
    public float NormalizedRaceTime { get; private set; }
    public int PlayerHorseIndex { get; private set; }
    public event Action OnFinishTrackEvent = ActionUtility.EmptyAction.Instance;
    private UIHorseRacingTimingType.TimingType timingType;
    private HorseRaceFirstPersonPlayerController playerController;
    private HorseRaceThirdPersonBehaviour playerHorseRaceThirdPersonBehaviour;
    private UITouchDisablePresenter touchDisablePresenter;
    private HorseRaceThirdPersonInfo[] horseRaceThirdPersonInfo;
    private MasterHorseContainer masterHorseContainer;
    private UITouchDisablePresenter TouchDisablePresenter => touchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();

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
            x.StartRace(x.IsPlayer ? GetNormalizeSpeedBaseOnTimingType() : UnityEngine.Random.value);
        });
        uiHorseRacingController.SetEntity(new UIHorseRacingController.Entity()
        {
            cameraBtn = new UIComponentHoldImageBehavior.Entity()
            {
                onDown = () => ChangeBackCamera(true),
                onUp = () => ChangeBackCamera(false)
            },
            sprintBtn = new ButtonComponent.Entity(playerController.Sprint)
        });
        uiHorseRacingController.In().Forget();
    }

    private void ChangeBackCamera(bool isBackCamera)
    {
        playerHorseRaceThirdPersonBehaviour.EnableCamera(isBackCamera);
    }

    private float GetNormalizeSpeedBaseOnTimingType()
    {
        return timingType switch
        {
            UIHorseRacingTimingType.TimingType.None => 0.0f,
            UIHorseRacingTimingType.TimingType.Bad => UnityEngine.Random.Range(0.0f, 0.2f),
            UIHorseRacingTimingType.TimingType.Good => UnityEngine.Random.Range(0.2f, 0.4f),
            UIHorseRacingTimingType.TimingType.Great => UnityEngine.Random.Range(0.4f, 0.8f),
            UIHorseRacingTimingType.TimingType.Perfect => UnityEngine.Random.Range(0.8f, 1.0f),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void UpdateRaceTime()
    {
        if (!isStarted) return;
        uiHorseRacingController.SetSprintTime(playerHorseRaceThirdPersonBehaviour.CurrentSprintNormalizeTime);
        uiHorseRacingController.sprintHealingProgress.SetEntity(playerHorseRaceThirdPersonBehaviour.CurrentSprintHealingNormalizeTime);
        uiHorseRacingController.sprintBtn.SetInteractable(playerHorseRaceThirdPersonBehaviour.IsAbleToSprint);
        NormalizedRaceTime = HorseControllers.Max(x => x.CurrentRaceProgressWeight);
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
        HorseRaceThirdPersonBehaviours.ForEach(x => x.OnFinishRace += FinishTrackEvent);
    }

    private void FinishTrackEvent() => OnFinishTrackEvent.Invoke();

    private void UnsubscribeFinishTrackEvents()
    {
        HorseRaceThirdPersonBehaviours.ForEach(x => x.OnFinishRace -= FinishTrackEvent);
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
        DisposeUtility.SafeDisposeMonoBehaviour(ref targetGenerator);
        Time.timeScale = 1;
        
        OnFinishTrackEvent = ActionUtility.EmptyAction.Instance;
        timingType = default;
        playerController = default;
        playerHorseRaceThirdPersonBehaviour = default;
        touchDisablePresenter = default;
        mapSettings = default;
        horseRaceThirdPersonInfo = default;
    }
}
