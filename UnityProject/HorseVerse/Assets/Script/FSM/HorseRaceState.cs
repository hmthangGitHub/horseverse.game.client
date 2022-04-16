using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HorseRaceState : BState
{
    private HorseRaceManager horseRaceManager;
    private UIHorseRaceStatus uIHorseRaceStatus;
    private UISpeedController uiSpeedController;
    private UIRaceResultSelf uiRaceResultSelf;
    private UIRaceResultList uiRaceResultList;
    private Scene racing = default;
    private int[] cachePositions;
    private bool IsGameStart = false;
    private Scene racingScene;
    private int playerHorseId;

    public override async void Enter()
    {
        base.Enter();
        playerHorseId = this.SuperMachine.GetPreviousState<HorsePickingState>().HorseId;
        await InitUI();
        StartGame();
    }

    private void StartGame()
    {
        IsGameStart = true;
        int[] horseIdInLanes = RandomHorseInLanes();
        SetEntityHorseRaceManager(horseIdInLanes);
        SetEntityUISpeedController();
    }

    private void SetEntityHorseRaceManager(int[] horseIdInLanes)
    {
        horseRaceManager.StartRace(horseIdInLanes, playerHorseId);
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
                ToMainState();
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

    private void ToMainState()
    {
        this.SuperMachine.ChangeState<HorsePickingState>();
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
                playerId = playerHorseId,
            },
            finishTime = timeToFinish
        });
        uIHorseRaceStatus.In().Forget();
    }

    private async UniTask InitUI()
    {
        horseRaceManager ??= GameObject.Instantiate<HorseRaceManager>((await Resources.LoadAsync<HorseRaceManager>("HorseRaceManager") as HorseRaceManager));
        uIHorseRaceStatus ??= await UILoader.Load<UIHorseRaceStatus>();
        uiSpeedController ??= await UILoader.Load<UISpeedController>();
        uiRaceResultSelf ??= await UILoader.Load<UIRaceResultSelf>();
        uiRaceResultList ??= await UILoader.Load<UIRaceResultList>();
        await LoadRacingScene();
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
        if (IsGameStart)
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

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Exit()
    {
        base.Exit();
        IsGameStart = false;
        GameObject.Destroy(horseRaceManager.gameObject);
        horseRaceManager = default;
        GameObject.Destroy(uiSpeedController.gameObject);
        uiSpeedController = default;
        GameObject.Destroy(uiRaceResultSelf.gameObject);
        uiRaceResultSelf = default;
        GameObject.Destroy(uiRaceResultList.gameObject);
        uiRaceResultList = default;
        GameObject.Destroy(uIHorseRaceStatus.gameObject);
        uIHorseRaceStatus = default;
        SceneManager.UnloadSceneAsync(racingScene).ToUniTask().Forget();
    }
}
