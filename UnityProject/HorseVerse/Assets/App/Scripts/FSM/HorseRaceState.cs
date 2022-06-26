using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HorseRaceState : InjectedBState
{
    private HorseRaceManager horseRaceManager;
    private UIHorseRaceStatus uIHorseRaceStatus;
    private UISpeedController uiSpeedController;
    private UIRaceResultSelf uiRaceResultSelf;
    private UIRaceResultList uiRaceResultList;
    private Scene racing = default;
    private int[] cachePositions;
    private bool isGameStart = false;
    private bool isLoadedUI = false;
    private Scene racingScene;
    private IReadOnlyUserDataRepository userDataRepository;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= Container.Inject<IReadOnlyUserDataRepository>();
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();
    private int playerHorseIndex;

    public override async void Enter()
    {
        base.Enter();
        playerHorseIndex = MasterHorseContainer.DataList
                                               .ToList()
                                               .FindIndex(x => x.MasterHorseId == UserDataRepository.Current.MasterHorseId);
        await LoadResources();
        isLoadedUI = true;
        var uiLoadingPresenter = this.Container.Inject<UILoadingPresenter>();
        uiLoadingPresenter.HideLoading();
    }

    private void StartGame()
    {
        isGameStart = true;
        int[] horseIdInLanes = RandomHorseInLanes();
        SetEntityHorseRaceManager(horseIdInLanes);
        SetEntityUISpeedController();
    }

    private void SetEntityHorseRaceManager(int[] horseIdInLanes)
    {
        horseRaceManager.StartRace(horseIdInLanes, playerHorseIndex);
        horseRaceManager.OnFinishTrackEvent += OnFinishTrack;
        SetEntityUIHorseRace(horseIdInLanes, horseRaceManager.horseControllers.Min(x => x.currentTimeToFinish));
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
            top = horseRaceManager.horseControllers.Find(x => x.IsPlayer).top
        });
        uiRaceResultSelf.In().Forget();
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
                ToMainState().Forget();
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

    private async UniTaskVoid ToMainState()
    {
        this.Container.Inject<UILoadingPresenter>().ShowLoadingAsync().Forget();
        await UniTask.Delay(1000);
        this.SuperMachine.GetState<StartUpState>().GetState<InitialState>().ChangeState<MainMenuState>();
    }

    private static int[] RandomHorseInLanes()
    {
        var randomizeHorsePath = HorseMasterDataContainer.HorseModelPaths.Shuffle().ToList();
        var horseIdInLanes = HorseMasterDataContainer.HorseModelPaths.Select(x => randomizeHorsePath.FindIndex(path => path == x)).ToArray();
        return horseIdInLanes;
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

    private void SetEntityUIHorseRace(int[] playerList, float timeToFinish)
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

    private async UniTask LoadResources()
    {
        await LoadUI();
        await LoadRacingScene();
    }

    private async UniTask LoadUI()
    {
        horseRaceManager ??= GameObject.Instantiate<HorseRaceManager>((await Resources.LoadAsync<HorseRaceManager>("GamePlay/HorseRaceManager") as HorseRaceManager));
        uIHorseRaceStatus ??= await UILoader.Initiate<UIHorseRaceStatus>();
        uiSpeedController ??= await UILoader.Initiate<UISpeedController>();
        uiRaceResultSelf ??= await UILoader.Initiate<UIRaceResultSelf>();
        uiRaceResultList ??= await UILoader.Initiate<UIRaceResultList>();
    }

    private async UniTask LoadRacingScene()
    {
        var operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        await operation.ToUniTask();
        racingScene = SceneManager.GetSceneAt(1);
        SceneManager.SetActiveScene(racingScene);
    }

    public override void PhysicsExecute()
    {
        base.PhysicsExecute();
        if (isGameStart)
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
    }

    public override void Execute()
    {
        base.ManualExecute();
        if (isLoadedUI && !isGameStart && (Input.touchCount >= 1 || Input.anyKey))
        {
            isGameStart = true;
            StartGame();
        }
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Exit()
    {
        base.Exit();
        isGameStart = false;
        isLoadedUI = false;

        GameObject.Destroy(horseRaceManager?.gameObject);
        horseRaceManager = null;

        UILoader.SafeRelease(ref uiSpeedController);
        UILoader.SafeRelease(ref uiRaceResultSelf);
        UILoader.SafeRelease(ref uiRaceResultList);
        UILoader.SafeRelease(ref uIHorseRaceStatus);

        SceneManager.UnloadSceneAsync(racingScene).ToUniTask().Forget();
    }
}
