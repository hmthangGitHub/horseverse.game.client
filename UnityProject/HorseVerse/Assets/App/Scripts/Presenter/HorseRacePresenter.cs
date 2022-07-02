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
    private const string mapPath = "Maps/racing_scene_stadium";
    private HorseRaceManager horseRaceManager;
    private UIHorseRaceStatus uIHorseRaceStatus;
    private UISpeedController uiSpeedController;
    private UIRaceResultSelf uiRaceResultSelf;
    private UIRaceResultList uiRaceResultList;

    private int[] cachePositions;
    private IReadOnlyUserDataRepository userDataRepository;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= Container.Inject<IReadOnlyUserDataRepository>();
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();
    private int playerHorseIndex;
    public event Action OnBackToMainState = ActionUtility.EmptyAction.Instance;

    private IDIContainer Container { get; }

    public HorseRacePresenter(IDIContainer container)
    {
        Container = container;
    }

    public async UniTask LoadAssetAsync()
    {
        await InitHorseRaceAsync();
        await LoadUI();
        await LoadRacingScene(default);
    }

    private async UniTask InitHorseRaceAsync()
    {
        horseRaceManager ??= GameObject.Instantiate<HorseRaceManager>((await Resources.LoadAsync<HorseRaceManager>("GamePlay/HorseRaceManager") as HorseRaceManager));
        var masterHorseIds = GetAllMasterHorseIds();
        var playerHorseIndex = masterHorseIds.ToList().FindIndex(x => x == UserDataRepository.Current.MasterHorseId);
        await horseRaceManager.InitializeAsync(masterHorseIds.Select(x => MasterHorseContainer.MasterHorseIndexer[x].RaceModeModelPath)
                                                             .ToArray(),
                                               "",
                                               playerHorseIndex,
                                               GetHorseTopPosition(),
                                               default);
    }

    private static int[] GetHorseTopPosition()
    {
        return Enumerable.Range(1, 8).Shuffle().ToArray();
    }

    private long[] GetAllMasterHorseIds()
    {
        return MasterHorseContainer.MasterHorseIndexer.Keys.Shuffle()
                                                           .Append(UserDataRepository.Current.MasterHorseId)
                                                           .Shuffle()
                                                           .Take(8)
                                                           .ToArray();
    }

    private async UniTask LoadRacingScene(CancellationToken token)
    {
        await SceneAssetLoader.LoadSceneAsync(mapPath, true, token : token);
    }

    private async UniTask LoadUI()
    {
        uIHorseRaceStatus ??= await UILoader.Instantiate<UIHorseRaceStatus>();
        uiSpeedController ??= await UILoader.Instantiate<UISpeedController>();
        uiRaceResultSelf ??= await UILoader.Instantiate<UIRaceResultSelf>();
        uiRaceResultList ??= await UILoader.Instantiate<UIRaceResultList>();
    }

    public void StartGame()
    {
        int[] horseIdInLanes = RandomHorseInLanes();
        SetEntityHorseRaceManager(horseIdInLanes);
        SetEntityUISpeedController();
    }

    private void SetEntityHorseRaceManager(int[] horseIdInLanes)
    {
        horseRaceManager.StartRace();
        horseRaceManager.OnFinishTrackEvent += OnFinishTrack;
        SetEntityUIHorseRaceStatus(horseIdInLanes, horseRaceManager.RaceTime);
    }

    private void SetEntityResultList()
    {
        uiRaceResultList.SetEntity(new UIRaceResultList.Entity()
        {
            horseList = new UIComponentHorseResultList.Entity()
            {
                entities = GetResultList()
            },
            closeBtn = new ButtonComponent.Entity(() =>
            {
                OnBackToMainState.Invoke();
            })
        });
        uiRaceResultList.In().Forget();
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
        uiRaceResultSelf.SetEntity(new UIRaceResultSelf.Entity()
        {
            name = "My name",
            speech = "The plates will still shift and the clouds will still spew. The sun will slowly rise and the moon will follow too.",
            btnTapAnyWhere = new ButtonComponent.Entity(() =>
            {
                uiRaceResultSelf.Out().Forget();
                SetEntityResultList();
            }),
            top = horseRaceManager.horseControllers.First(x => x.IsPlayer).top
        });
        uiRaceResultSelf.In().Forget();
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
                uIHorseRaceStatus.playerList.ChangePosition(positions[i].Lane, i);
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
                this.uIHorseRaceStatus.Skip();
            })
        });
        uiSpeedController.In().Forget();
    }

    private void SetEntityUIHorseRaceStatus(int[] playerList, float timeToFinish)
    {
        cachePositions = Enumerable.Repeat(-1, playerList.Length).ToArray();
        uIHorseRaceStatus.SetEntity(new UIHorseRaceStatus.Entity()
        {
            playerList = new HorseRaceStatusPlayerList.Entity()
            {
                horseIdInLane = playerList,
                playerId = playerHorseIndex,
            },
            finishTime = timeToFinish
        });
        uIHorseRaceStatus.In().Forget();
    }

    private int[] RandomHorseInLanes()
    {
        var randomizeHorsePath = MasterHorseContainer.MasterHorseIndexer.Select(x => x.Value.ModelPath).Shuffle().ToList();
        var horseIdInLanes = MasterHorseContainer.MasterHorseIndexer.Select(x => randomizeHorsePath.FindIndex(path => path == x.Value.ModelPath)).ToArray();
        return horseIdInLanes;
    }

    public void Dispose()
    {
        GameObject.Destroy(horseRaceManager?.gameObject);
        horseRaceManager = null;

        UILoader.SafeRelease(ref uiSpeedController);
        UILoader.SafeRelease(ref uiRaceResultSelf);
        UILoader.SafeRelease(ref uiRaceResultList);
        UILoader.SafeRelease(ref uIHorseRaceStatus);

        SceneAssetLoader.UnloadAssetAtPath(mapPath);
    }
}
