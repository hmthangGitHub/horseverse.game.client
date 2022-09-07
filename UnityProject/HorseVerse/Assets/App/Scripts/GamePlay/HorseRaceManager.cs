#define DEVELOPMENT
using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HorseRaceManager : MonoBehaviour, IDisposable
{
    public FreeCamera freeCamera;
    public RaceModeCameras raceCamera;
    public TargetGenerator targetGenerator;
    public WarmUpCamera warmUpCamera;
    private Scene mapScene;
    private string mapPath;

    public GameObject mainCamera;
    public CameraBlendingAnimation cameraBlendingAnimation;
    private PathCreation.PathCreator Path => targetGenerator.SimplyPath;

    private string[] horseControllerModelPaths;
    private string mapSettingsPath;

    public float[] timeToFinish;

    public float offSet = -5;
    public float offsetPerLane = -1.0f;

    public HorseController[] horseControllers;

    public Transform horsePosition;
    public CinemachineTargetGroup horseGroup;
    public float RaceTime { get; private set; }

    public float averageSpeed = 24.0f;
    public float averageTimeToFinish;
    public Vector2 timeOffSet = new Vector2(-1.0f, 1.0f);

    public float raceLength;
    public int playerHorseIndex;
    private RacingTrackController racingTrackController;
    public event Action OnFinishTrackEvent = ActionUtility.EmptyAction.Instance;

    public async UniTask InitializeAsync(string[] horseControllerPaths,
                                         string mapSettingsPath,
                                         string mapPath,
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
        await LoadMapSettings(token);
        await LoadRacingScene(mapPath, token);
        await LoadHorses(horseControllerPaths, token);
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
            freeCamera.OnSkipFreeCamera -= OnSkipFreeCamera;
            freeCamera.gameObject.SetActive(false);
        }
        freeCamera.OnSkipFreeCamera += OnSkipFreeCamera;
        await ucs.Task;
        await cameraBlendingAnimation.FadeInAnimationAsync();
        mainCamera.SetActive(true);
    }

    public async UniTask ShowWarmUpCameraThenWait()
    {
        await ShowWarmUpCameraAsync();
        await WaitAndCountDownAsync();
    }

    private async UniTask ShowWarmUpCameraAsync()
    {
        warmUpCamera.gameObject.SetActive(true);
        horseControllers.ForEach(x => { x.gameObject.SetActive(true); });

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
        freeCamera = Instantiate(mapSettings.freeCamera, transform, true);

        raceCamera = Instantiate(GetRaceModeCamera(mapSettings), Vector3.zero, Quaternion.identity, transform);
        raceCamera.gameObject.SetActive(false);
        raceCamera.SetHorseGroup(this.horseGroup);
        var needFadingBlendingCameras = GetNeedFadingBlendingCameras();
        raceCamera.GetComponentsInChildren<ActivateCamera>(true).Where(x => needFadingBlendingCameras.Contains(x.activateCamera.name))
            .ForEach(x => 
            {
                x.OnBeginActivateCameraEvent += cameraBlendingAnimation.FadeInAnimationAsync;
                x.OnActivateCameraEvent += cameraBlendingAnimation.FadeOutAnimationAsync; 
            });
        
        targetGenerator = Instantiate(mapSettings.targetGenerator, Vector3.zero,Quaternion.identity, transform);
        warmUpCamera = Instantiate(mapSettings.warmUpCamera, Vector3.zero, Quaternion.identity, transform);

        horsePosition.position = targetGenerator.StartPosition;
        warmUpCamera.gameObject.SetActive(false);
        warmUpCamera.SetTargetGroup(this.horseGroup.transform);
    }

    private IEnumerable<string> GetNeedFadingBlendingCameras()
    {
        var needFadingBlendingCameras = mainCamera.GetComponent<CinemachineBrain>().m_CustomBlends.m_CustomBlends
                .Where(x => x.m_Blend.m_Style == CinemachineBlendDefinition.Style.Cut)
                .Select(x => x.m_To);
        return needFadingBlendingCameras;
    }

    private static RaceModeCameras GetRaceModeCamera(MapSettings mapSettings)
    {
#if DEVELOPMENT
        try
        {
            if (PlayerPrefs.GetInt(CameraInMapDebugPanel.MapNumberKey) >= 1)
            {
                return mapSettings.raceModeCamera[PlayerPrefs.GetInt(CameraInMapDebugPanel.MapNumberKey) - 1];
            }
        }
        catch (Exception)
        {
        }
#endif
        return mapSettings.raceModeCamera.RandomElement();
    }
    
    private async UniTask LoadRacingScene(string mapPath, CancellationToken token)
    {
        this.mapPath = mapPath;
        mapScene = await SceneAssetLoader.LoadSceneAsync(mapPath, true, token : token);
        racingTrackController = mapScene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<RacingTrackController>(true))
            .FirstOrDefault();
    }

    private async UniTask LoadHorses(string[] horseControllerPaths, CancellationToken token)
    {
        horseControllers = await UniTask.WhenAll(horseControllerPaths.Select(x => LoadHorseController(x, token)));
        horseControllers.ForEach(x => x.gameObject.SetActive(false));
    }

    private async UniTask<HorseController> LoadHorseController(string horseControllerPath, CancellationToken token)
    {
        var horsePrefab = await PrimitiveAssetLoader.LoadAssetAsync<GameObject>(horseControllerPath, token);
        var horseController = GameObject.Instantiate(horsePrefab.GetComponent<HorseController>(), horsePosition);
        horseGroup.AddMember(horseController.transform, 1, 0);
        return horseController;
    }

    public void StartRace()
    {
        horseControllers.ForEach(x => x.StartRace());
    }

    private async UniTask WaitAndCountDownAsync()
    {
        await cameraBlendingAnimation.FadeInAnimationAsync();
        freeCamera.gameObject.SetActive(false);
        raceCamera.gameObject.SetActive(true);
        racingTrackController.PlayStartAnimation();
        await (cameraBlendingAnimation.FadeOutAnimationAsync(),
                UniTask.Delay(TimeSpan.FromSeconds(3)));
    }

    private void CalculateRaceStat(float[] times, int totalLap)
    {
        raceLength = Path.path.length * Path.transform.lossyScale.x;
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
                TargetGenerator = targetGenerator,
                CurrentOffset = offSet + i * offsetPerLane,
                TopInRaceMatch = topInRaceMatch[i],
                IsPlayer = playerHorseIndex == i,
                InitialLane = i,
                OnFinishTrack = () => OnFinishTrack(topInRaceMatch[index]),
                PredefineTargets = CalculatePredefineTarget(horseRaceTimes[i])
            });
        }
    }

    private float GetMinimumRaceTime(HorseRaceTime[] horseRaceTimes)
    {
        return horseRaceTimes.Min(x => x.raceSegments.Sum(raceSegment => raceSegment.waypoints.Sum(y => y.time)));
    }

    private ((Vector3 , float)[], int finishIndex) CalculatePredefineTarget(HorseRaceTime horseRaceTime)
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
        if(mapScene != default)
        {
            SceneAssetLoader.UnloadAssetAtPath(mapPath);
            mapScene = default;
        }
        var needFadingBlendingCameras = GetNeedFadingBlendingCameras();
        raceCamera.GetComponentsInChildren<ActivateCamera>(true).Where(x => needFadingBlendingCameras.Contains(x.activateCamera.name))
            .ForEach(x => 
            {
                x.OnBeginActivateCameraEvent -= cameraBlendingAnimation.FadeInAnimationAsync;
                x.OnActivateCameraEvent -= cameraBlendingAnimation.FadeOutAnimationAsync; 
            });
        this.horseControllerModelPaths.ToList().ForEach(PrimitiveAssetLoader.UnloadAssetAtPath);
        PrimitiveAssetLoader.UnloadAssetAtPath(mapSettingsPath);
        Time.timeScale = 1;
        racingTrackController = default;
    }
}
