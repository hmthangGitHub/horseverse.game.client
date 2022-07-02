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
    public FreeCamera freeCamera;
    public RaceModeCameras raceCamera;
    public PathCreation.PathCreator path;

    private string[] horseControllerModelPaths;
    private string mapSettingsPath;

    public Transform[] transforms;
    public float[] timeToFinish;

    public uint laneNumber = 10;
    public float offSet = -5;
    public float offsetPerLane = -1.0f;

    public HorseController[] horseControllers = default;

    public Transform horsePosition;
    public CinemachineTargetGroup horseGroup;
    public float RaceTime => horseControllers.Min(x => x.currentTimeToFinish);

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
                                         int[] top,
                                         CancellationToken token)
    {
        this.horseControllerModelPaths = horseControllerPaths;
        this.mapSettingsPath = mapSettingsPath;
        this.playerHorseIndex = playerHorseIndex;

        await LoadHorses(horseControllerPaths, token);
        await LoadMapSettings(token);
        CalculateRaceStat(top);
        SetHorseControllerStat(top);
    }

    private async UniTask LoadMapSettings(CancellationToken token)
    {
        var mapSettings = await PrimitiveAssetLoader.LoadAssetAsync<MapSettings>(mapSettingsPath, token);
        freeCamera = Instantiate<FreeCamera>(mapSettings.freeCamera, Vector3.zero, Quaternion.identity, transform);
        raceCamera = Instantiate<RaceModeCameras>(mapSettings.raceModeCamera, Vector3.zero, Quaternion.identity, transform);
        path = Instantiate<PathCreation.PathCreator>(mapSettings.path, Vector3.zero,Quaternion.identity, transform);
        raceCamera.SetHorseGroup(this.horseGroup);
    }

    private async Task LoadHorses(string[] horseControllerPaths, CancellationToken token)
    {
        horseControllers = await UniTask.WhenAll(horseControllerPaths.Select(x => LoadHorseController(x, token)));
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

        horseControllers.FirstOrDefault(x => x.top == 1).OnFinishTrackEvent += OnFinishTrack;
    }

    private void CalculateRaceStat(int[] top)
    {
        raceLength = path.path.length * path.transform.lossyScale.x;
        averageTimeToFinish = raceLength / averageSpeed * totalLap;
        timeToFinish = new float[top.Length];
        //top = top.OrderBy(x => UnityEngine.Random.Range(-1.0f, 1.0f)).ToArray();
        for (int i = 0; i < timeToFinish.Length; i++)
        {
            timeToFinish[i] = averageTimeToFinish + UnityEngine.Random.Range(timeOffSet.x, timeOffSet.y);
        }
        timeToFinish = timeToFinish.OrderBy(x => x).ToArray();
    }

    private void SetHorseControllerStat(int[] top)
    {
        for (int i = 0; i < horseControllers.Length; i++)
        {
            HorseController horseController = horseControllers[i];
            horseController.pathCreator = this.path;
            horseController.currentOffset = offSet + i * offsetPerLane;
            horseController.top = top[i];
            horseController.timeToFinish = timeToFinish[top[i] - 1];
            horseController.currentTimeToFinish = horseController.timeToFinish;
            horseController.averageTimeToFinish = averageTimeToFinish;
            horseController.lap = totalLap;
            horseController.IsPlayer = playerHorseIndex == i;
            horseController.Lane = i;
            horseController.StartRace();
        }
    }

    private void OnFinishTrack()
    {
        OnFinishTrackEvent?.Invoke();
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
    }
}
