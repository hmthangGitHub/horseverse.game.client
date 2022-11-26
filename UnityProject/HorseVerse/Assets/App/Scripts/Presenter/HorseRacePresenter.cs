using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class HorseRacePresenter : IDisposable
{
    private HorseRaceManager horseRaceManager;
    private UIHorseRaceStatus uiHorseRaceStatus;
    private UISpeedController uiSpeedController;
    private UIFlashScreenAnimation uiFlashScreen;

    private int[] cachePositions;
    private IReadOnlyUserDataRepository userDataRepository;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= Container.Inject<IReadOnlyUserDataRepository>();
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();
    private int playerHorseIndex;
    public event Action OnToBetModeResultState = ActionUtility.EmptyAction.Instance;
    public event Action OnToQuickRaceModeResultState = ActionUtility.EmptyAction.Instance;

    private RaceMatchData raceMatchData;
    private RaceMatchData RaceMatchData => raceMatchData ??= Container.Inject<RaceMatchData>();
    private UIBackGroundPresenter uiBackGroundPresenter;
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();

    private MasterMapContainer masterMapContainer;
    private MasterMap masterMap;
    private MapSettings mapSettings;
    private TargetGenerator targetGenerator;
    private CancellationTokenSource cts;
    private RaceModeHorseIntroPresenter raceModeHorseIntroPresenter;
    private int numberOfHorseFinishTheRace = 0;

    private IDIContainer Container { get; }

    public HorseRacePresenter(IDIContainer container)
    {
        Container = container;
    }

    public async UniTask LoadAssetAsync()
    {
        await GetMasterMap();
        await GetMapSettings();
        await InitHorseRaceAsync();
        await LoadUI();
        raceModeHorseIntroPresenter = new RaceModeHorseIntroPresenter(Container);
        await raceModeHorseIntroPresenter.LoadUIAsync();
    }

    private async UniTask GetMapSettings()
    {
        mapSettings = await PrimitiveAssetLoader.LoadAssetAsync<MapSettings>(masterMap.MapSettings, default);
        targetGenerator = Object.Instantiate(mapSettings.targetGenerator, Vector3.zero, Quaternion.identity);
    }

    public async UniTask PlayIntro()
    {
        await horseRaceManager.ShowFreeCamera();
        horseRaceManager.EnablePostProcessing(true);
        await (horseRaceManager.cameraBlendingAnimation.FadeOutAnimationAsync(),
               raceModeHorseIntroPresenter.ShowHorsesInfoIntroAsync(RaceMatchData.HorseRaceTimes.Select(x => x.masterHorseId).ToArray(), 
                                                                    targetGenerator.StartPosition, 
                                                                    Quaternion.identity));
        raceModeHorseIntroPresenter.Dispose();
        raceModeHorseIntroPresenter = default;
        horseRaceManager.EnablePostProcessing(false);
        await horseRaceManager.cameraBlendingAnimation.FadeInAnimationAsync();
        
        await (horseRaceManager.ShowWarmUpCameraThenWait(),
               horseRaceManager.cameraBlendingAnimation.FadeOutAnimationAsync());
    }

    private async UniTask GetMasterMap()
    {
        masterMapContainer = await MasterLoader.LoadMasterAsync<MasterMapContainer>();
        masterMap = masterMapContainer.MasterMapIndexer[RaceMatchData.MasterMapId];
    }

    private async UniTask InitHorseRaceAsync()
    {
        horseRaceManager ??= Object.Instantiate((await Resources.LoadAsync<HorseRaceManager>("GamePlay/HorseRaceManager") as HorseRaceManager));
        playerHorseIndex = RaceMatchData.HorseRaceTimes.ToList().FindIndex(x => x.masterHorseId == UserDataRepository.Current.CurrentHorseNftId);
        await horseRaceManager.InitializeAsync(RaceMatchData.HorseRaceTimes.Select(x => MasterHorseContainer.MasterHorseIndexer[x.masterHorseId].RaceModeModelPath).ToArray(),
                                               masterMap.MapSettings,
                                               masterMap.MapPath,
                                               playerHorseIndex,
                                               RaceMatchData.HorseRaceTimes.Select(x => x.raceSegments.Sum(segment => segment.time)).ToArray(),
                                               1,
                                               RaceMatchData.HorseRaceTimes,
                                               default);
    }

    private async UniTask LoadUI()
    {
        uiHorseRaceStatus ??= await UILoader.Instantiate<UIHorseRaceStatus>();
        uiSpeedController ??= await UILoader.Instantiate<UISpeedController>();
        uiFlashScreen ??= await UILoader.Instantiate<UIFlashScreenAnimation>();
    }

    public void StartGame()
    {
        int[] horseIdInLanes = RandomHorseInLanes();
        SetEntityHorseRaceManager(horseIdInLanes);
        SetEntityUISpeedController();
        StartUpdateRaceHorseStatus().Forget();
    }

    private async UniTaskVoid StartUpdateRaceHorseStatus()
    {
        cts = new CancellationTokenSource();
        while (true)
        {
            await UniTask.WaitForFixedUpdate(cancellationToken : cts.Token);
            UpdateRaceStatus();
        }
    }

    private void SetEntityHorseRaceManager(int[] horseIdInLanes)
    {
        horseRaceManager.StartRace();
        horseRaceManager.OnFinishTrackEvent += OnFinishTrack;
        SetEntityUIHorseRaceStatus(horseIdInLanes, horseRaceManager.RaceTime);
    }

    private void OnFinishTrack()
    {
        OnFinishTrackAsync().Forget();
    }

    private async UniTask OnFinishTrackAsync()
    {
        numberOfHorseFinishTheRace++;
        await FlashScreenAsync();
        if (numberOfHorseFinishTheRace == 2)
        {
            horseRaceManager.OnFinishTrackEvent -= OnFinishTrack;
            uiSpeedController.Out().Forget();
            uiHorseRaceStatus.Out().Forget();
            await UIBackGroundPresenter.ShowBackGroundAsync();
            if (RaceMatchData.Mode == RaceMode.QuickMode)
            {
                OnToQuickRaceModeResultState();
            }
            else
            {
                OnToBetModeResultState();
            }
        }
    }

    private async UniTask FlashScreenAsync()
    {
        var currentTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), ignoreTimeScale: true);
        await uiFlashScreen.In();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), ignoreTimeScale: true);
        
        Time.timeScale = currentTimeScale;
        await uiFlashScreen.Out();
    }

    private async UniTaskVoid FreezeTimeWhenFinishTrack()
    {
        var currentTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        await UniTask.Delay((int)(1.0f * 1000), ignoreTimeScale: true);
        Time.timeScale = currentTimeScale;
    }

    private void UpdateRaceStatus()
    {
        var positions = horseRaceManager.horseControllers.OrderByDescending(x => x.CurrentRaceProgressWeight)
                                                             .Select(x => x)
                                                             .ToArray();
        for (int i = 0; i < positions.Length; i++)
        {
            if (cachePositions[i] != positions[i].InitialLane)
            {
                uiHorseRaceStatus.playerList.ChangePosition(positions[i].InitialLane, i);
                cachePositions[i] = positions[i].InitialLane;
            }
        }
    }

    private void SetEntityUISpeedController()
    {
        uiSpeedController.SetEntity(new UISpeedController.Entity()
        {
            fast = new ButtonComponent.Entity(() =>
            {
                Time.timeScale = 2.0f;
            }),
            normal = new ButtonComponent.Entity(() =>
            {
                Time.timeScale = 1.0f;
            }),
            pause = new ButtonComponent.Entity(() =>
            {
                Time.timeScale = 0.0f;
            }),
            skip = new ButtonComponent.Entity(() =>
            {
                this.horseRaceManager.Skip();
                this.uiHorseRaceStatus.Skip();
            })
        });
        uiSpeedController.In().Forget();
    }

    private void SetEntityUIHorseRaceStatus(int[] playerList, float timeToFinish)
    {
        cachePositions = Enumerable.Repeat(-1, playerList.Length).ToArray();
        uiHorseRaceStatus.SetEntity(new UIHorseRaceStatus.Entity()
        {
            playerList = new HorseRaceStatusPlayerList.Entity()
            {
                horseIdInLane = playerList,
                playerId = playerHorseIndex,
            },
            finishTime = timeToFinish
        });
        uiHorseRaceStatus.In().Forget();
    }

    private int[] RandomHorseInLanes()
    {
        return Enumerable.Range(0, RaceMatchData.HorseRaceTimes.Length).ToArray();
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        horseRaceManager.Dispose();
        Object.Destroy(horseRaceManager?.gameObject);
        horseRaceManager = null;
        Object.Destroy(targetGenerator);
        targetGenerator = null;

        UILoader.SafeRelease(ref uiSpeedController);
        UILoader.SafeRelease(ref uiHorseRaceStatus);
        UILoader.SafeRelease(ref uiFlashScreen);
        MasterLoader.SafeRelease(ref masterMapContainer);

        if(mapSettings != default)
        {
            PrimitiveAssetLoader.UnloadAssetAtPath(masterMap.MapSettings);
            mapSettings = default;
        }

        masterMap = default;
        raceMatchData = default;

        OnToBetModeResultState = ActionUtility.EmptyAction.Instance;
        OnToQuickRaceModeResultState = ActionUtility.EmptyAction.Instance;

        cachePositions = default;
        masterHorseContainer = default;
        
        raceModeHorseIntroPresenter?.Dispose();
        raceModeHorseIntroPresenter = default;
    }
}
