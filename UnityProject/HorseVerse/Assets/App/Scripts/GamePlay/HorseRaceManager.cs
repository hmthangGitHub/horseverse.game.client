using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class HorseRaceManager : MonoBehaviour, IDisposable
{
    public float maximumDistance = 1.0f;
    public Vector2Int times;

    public FreeCamera freeCamera;
    public RaceModeCameras raceCamera;
    public TargetGenerator targetGenerator;
    public WarmUpCamera warmUpCamera;
    public PathCreation.PathCreator path => targetGenerator.SimplyPath;

    private string[] horseControllerModelPaths;
    private string mapSettingsPath;

    public float[] timeToFinish;

    public uint laneNumber = 10;
    public float offSet = -5;
    public float offsetPerLane = -1.0f;

    public HorseController[] horseControllers = default;

    public Transform horsePosition;
    public CinemachineTargetGroup horseGroup;
    public float RaceTime { get; private set; }

    public int totalLap = 1;
    public float timeToFinishAround = 0.0f;

    public float averageSpeed = 24.0f;
    public float averageTimeToFinish = 0.0f;
    public Vector2 timeOffSet = new Vector2(-1.0f, 1.0f);

    public float raceLength = 0.0f;
    public int playerHorseIndex = 0;
    public event Action OnFinishTrackEvent = ActionUtility.EmptyAction.Instance;

    public async UniTask InitializeAsync(string[] horseControllerPaths,
                                         string mapSettingsPath,
                                         int playerHorseIndex,
                                         float[] times,
                                         int totalLap,
                                         HorseRaceTime[] horseRaceTimes,
                                         CancellationToken token)
    {
        this.horseControllerModelPaths = horseControllerPaths;
        this.mapSettingsPath = mapSettingsPath;
        this.playerHorseIndex = playerHorseIndex;
        var tops = times.GetTopByTimes();
        await LoadHorses(horseControllerPaths, token);
        await LoadMapSettings(token);
        CalculateRaceStat(times, totalLap);
        SetHorseControllerStat(tops, horseRaceTimes);
        RaceTime = GetMinimumRaceTime(horseRaceTimes);
    }

    public async UniTask ShowFreeCamera()
    {
        freeCamera.gameObject.SetActive(true);
        var ucs = new UniTaskCompletionSource();

        void OnSkipFreeCamera()
        {
            ucs.TrySetResult();
        }
        freeCamera.OnSkipFreeCamera += OnSkipFreeCamera;
        await UniTask.WhenAny(ucs.Task, UniTask.Delay(5000));
        freeCamera.OnSkipFreeCamera -= OnSkipFreeCamera;
        freeCamera.gameObject.SetActive(false);
    }

    public async UniTask ShowWarmUpCamera()
    {
        warmUpCamera.gameObject.SetActive(true);
        horseControllers.ForEach(x =>
        {
            x.gameObject.SetActive(true);
        });

        var ucs = new UniTaskCompletionSource();

        void OnFinishWarmingUp()
        {
            warmUpCamera.OnFinishWarmingUp -= OnFinishWarmingUp;
            ucs.TrySetResult();
        }
        warmUpCamera.OnFinishWarmingUp += OnFinishWarmingUp;
        await ucs.Task;
        warmUpCamera.gameObject.SetActive(false);
    }

    private async UniTask LoadMapSettings(CancellationToken token)
    {
        var mapSettings = await PrimitiveAssetLoader.LoadAssetAsync<MapSettings>(mapSettingsPath, token);
        freeCamera = Instantiate<FreeCamera>(mapSettings.freeCamera, transform, true);
        raceCamera = Instantiate<RaceModeCameras>(mapSettings.raceModeCamera[UnityEngine.Random.Range(0, mapSettings.raceModeCamera.Length)],
                                                  Vector3.zero,
                                                  Quaternion.identity,
                                                  transform);
        targetGenerator = Instantiate<TargetGenerator>(mapSettings.targetGenerator, Vector3.zero,Quaternion.identity, transform);
        warmUpCamera = Instantiate<WarmUpCamera>(mapSettings.warmUpCamera, Vector3.zero, Quaternion.identity, transform);

        warmUpCamera.gameObject.SetActive(false);
        raceCamera.SetHorseGroup(this.horseGroup);
        warmUpCamera.SetTargetGroup(this.horseGroup.transform);
    }

    private async UniTask LoadHorses(string[] horseControllerPaths, CancellationToken token)
    {
        horseControllers = await UniTask.WhenAll(horseControllerPaths.Select(x => LoadHorseController(x, token)));
        horseControllers.ForEach(x => x.gameObject.SetActive(false));
    }

    private async UniTask<HorseController> LoadHorseController(string horseControllerPath, CancellationToken token)
    {
        var horsePrefab = await PrimitiveAssetLoader.LoadAssetAsync<GameObject>(horseControllerPath, token);
        var horseController = GameObject.Instantiate<HorseController>(horsePrefab.GetComponent<HorseController>(), horsePosition);
        horseGroup.AddMember(horseController.transform, 1, 0);
        return horseController;
    }

    public void StartRace()
    {
        freeCamera.gameObject.SetActive(false);
        raceCamera.gameObject.SetActive(true);
        horseControllers.ForEach(x => x.StartRace());
    }

    private void CalculateRaceStat(float[] times, int totalLap)
    {
        raceLength = path.path.length * path.transform.lossyScale.x;
        averageTimeToFinish = raceLength / averageSpeed * totalLap;
        timeToFinish = new float[times.Length];
        for (int i = 0; i < timeToFinish.Length; i++)
        {
            timeToFinish[i] = averageTimeToFinish + UnityEngine.Random.Range(timeOffSet.x, timeOffSet.y);
        }
        timeToFinish = timeToFinish.OrderBy(x => x).ToArray();
    }

    private void SetHorseControllerStat(int[] topInRaceMatch, HorseRaceTime[] horseRaceTimes)
    {
        for (int i = 0; i < horseControllers.Length; i++)
        {
            var index = i;
            HorseController horseController = horseControllers[i];
            horseController.SetHorseData(new HorseInGameData()
            {
                PathCreator = this.path,
                CurrentOffset = offSet + i * offsetPerLane,
                TopInRaceMatch = topInRaceMatch[i],
                IsPlayer = playerHorseIndex == i,
                InitialLane = i,
                OnFinishTrack = () => OnFinishTrack(topInRaceMatch[index]),
                PredefineTargets = CalculatePredifineTarget(horseRaceTimes[i])
            });
        }
    }

    private float GetMinimumRaceTime(HorseRaceTime[] horseRaceTimes)
    {
        return horseRaceTimes.Min(x => x.raceSegments.Sum(x => x.waypoints.Sum(y => y.time)));
    }

    private (Vector3 , float)[] CalculatePredifineTarget(HorseRaceTime horseRaceTime)
    {
        return targetGenerator.GenerateTargets(horseRaceTime.raceSegments);
    }

    private void OnFinishTrack(int topInGame)
    {
        OnFinishTrackEvent.Invoke();
    }

    public void Skip()
    {
        foreach (var item in horseControllers)
        {
            item.Skip();
        }
    }

    public void Dispose()
    {
        this.horseControllerModelPaths.ToList().ForEach(x => PrimitiveAssetLoader.UnloadAssetAtPath(x));
        PrimitiveAssetLoader.UnloadAssetAtPath(mapSettingsPath);
        Time.timeScale = 1;
    }
}
