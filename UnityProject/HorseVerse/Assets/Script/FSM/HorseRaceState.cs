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
    private Scene racing;
    private int[] cachePositions;
    private bool IsGameStart = false;
    private Scene racingScene;

    public override async void Enter()
    {
        base.Enter();
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
        horseRaceManager.StartRace(horseIdInLanes, this.SuperMachine.GetPreviousState<HorsePickingState>().HorseId);
        horseRaceManager.OnFinishTrackEvent += OnFinishTrack;
        SetEntityUIHorseRace(horseIdInLanes, horseRaceManager.horseControllers.Min(x => x.currentTimeToFinish));
    }

    private void OnFinishTrack()
    {
        uiSpeedController.Out().Forget();
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
                playerIndex = horseRaceManager.playerHorseId,
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
                    Debug.Log($"positions[{i}] " + "positions[i].Lane" + positions[i].Lane);
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
    }
}
