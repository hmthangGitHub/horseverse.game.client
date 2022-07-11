using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HorseRacePresenter : IDisposable
{
    private HorseRaceManager horseRaceManager;
    private UIHorseRaceStatus uiHorseRaceStatus;
    private UISpeedController uiSpeedController;

    private int[] cachePositions;
    private IReadOnlyUserDataRepository userDataRepository;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= Container.Inject<IReadOnlyUserDataRepository>();
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();
    private int playerHorseIndex;
    public event Action OnBackToMainState = ActionUtility.EmptyAction.Instance;
    public event Action OnToBetModeResultState = ActionUtility.EmptyAction.Instance;
    public Action OnToQuickRaceModeResultState = ActionUtility.EmptyAction.Instance;

    private RaceMatchData raceMatchData;
    private RaceMatchData RaceMatchData => raceMatchData ??= Container.Inject<RaceMatchData>();

    private MasterMapContainer masterMapContainer;
    private MasterMap masterMap;
    private MapSettings mapSettings;
    private PathCreation.PathCreator path;
    private CancellationTokenSource cts;

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
        await LoadRacingScene(default);
    }

    private async UniTask GetMapSettings()
    {
        mapSettings = await PrimitiveAssetLoader.LoadAssetAsync<MapSettings>(masterMap.MapSettings, default);
        path = GameObject.Instantiate(mapSettings.path, Vector3.zero, Quaternion.identity);
    }

    public async UniTask PlayIntro()
    {
        await horseRaceManager.ShowFreeCamera();
        using (var raceModeHorseIntroPresenter = new RaceModeHorseIntroPresenter(Container))
        {
            await raceModeHorseIntroPresenter.ShowHorsesInfoIntroAsync(RaceMatchData.horseRaceTimes.Select(x => x.masterHorseId)
                                                                                                   .ToArray(), 
                                                                        path.path.GetPointAtTime(0), 
                                                                        Quaternion.identity);
        }
        await horseRaceManager.ShowWarmUpCamera();
    }

    private async UniTask GetMasterMap()
    {
        masterMapContainer = await MasterLoader.LoadMasterAsync<MasterMapContainer>();
        masterMap = masterMapContainer.MasterMapIndexer[RaceMatchData.masterMapId];
    }

    private async UniTask InitHorseRaceAsync()
    {
        horseRaceManager ??= GameObject.Instantiate<HorseRaceManager>((await Resources.LoadAsync<HorseRaceManager>("GamePlay/HorseRaceManager") as HorseRaceManager));
        var playerHorseIndex = RaceMatchData.horseRaceTimes.ToList().FindIndex(x => x.masterHorseId == UserDataRepository.Current.MasterHorseId);
        await horseRaceManager.InitializeAsync(RaceMatchData.horseRaceTimes.Select(x => MasterHorseContainer.MasterHorseIndexer[x.masterHorseId].RaceModeModelPath).ToArray(),
                                               masterMap.MapSettings,
                                               playerHorseIndex,
                                               RaceMatchData.horseRaceTimes.Select(x => x.time).ToArray(),
                                               1,
                                               default);
    }

    private async UniTask LoadRacingScene(CancellationToken token)
    {
        await SceneAssetLoader.LoadSceneAsync(masterMap.MapPath, true, token : token);
    }

    private async UniTask LoadUI()
    {
        uiHorseRaceStatus ??= await UILoader.Instantiate<UIHorseRaceStatus>();
        uiSpeedController ??= await UILoader.Instantiate<UISpeedController>();
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

    private UIComponentHorseResult.Entity[] GetResultList()
    {
        return horseRaceManager.horseControllers.Select(x => new UIComponentHorseResult.Entity()
        {
            isPlayer = x.IsPlayer,
            lane = x.Lane + 1,
            top = x.top,
            name = x.name,
            time = x.timeToFinish
        }).OrderBy(x => x.top)
          .ToArray();
    }

    private void OnFinishTrack()
    {
        Time.timeScale = 1.0f;
        uiSpeedController.Out().Forget();
        uiHorseRaceStatus.Out().Forget();
        if (RaceMatchData.mode == RaceMode.QuickMode)
        {
            OnToQuickRaceModeResultState();
        }
        else
        {
            OnToBetModeResultState();
        }
    }

    public void UpdateRaceStatus()
    {
        var positions = horseRaceManager.horseControllers.OrderByDescending(x => x.normalizePath)
                                                             .Select(x => x)
                                                             .ToArray();
        for (int i = 0; i < positions.Length; i++)
        {
            if (cachePositions[i] != positions[i].Lane)
            {
                uiHorseRaceStatus.playerList.ChangePosition(positions[i].Lane, i);
                cachePositions[i] = positions[i].Lane;
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
        return Enumerable.Range(0, RaceMatchData.horseRaceTimes.Length).ToArray();
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        GameObject.Destroy(horseRaceManager?.gameObject);
        horseRaceManager = null;
        GameObject.Destroy(path);
        path = null;

        UILoader.SafeRelease(ref uiSpeedController);
        UILoader.SafeRelease(ref uiHorseRaceStatus);

        MasterLoader.Unload<MasterMapContainer>();
        SceneAssetLoader.UnloadAssetAtPath(masterMap.MapPath);
        PrimitiveAssetLoader.UnloadAssetAtPath(masterMap.MapSettings);

        masterMapContainer = default;
        masterMap = default;
        raceMatchData = default;
        OnBackToMainState = ActionUtility.EmptyAction.Instance;
        cachePositions = default;
        masterHorseContainer = default;
    }
}
