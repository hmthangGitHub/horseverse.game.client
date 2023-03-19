#define DEVELOPMENT
using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class HorseRaceManager : MonoBehaviour, IDisposable
{
    public RaceModeCameras raceCamera;
    public TargetGenerator targetGenerator;
    private Scene mapScene;
    private string mapPath;

    public GameObject mainCamera;
    public CameraBlendingAnimation cameraBlendingAnimation;
    private PathCreation.PathCreator Path => targetGenerator.SimplyPath;

    private HorseMeshInformation[] horseControllerModelPaths;
    private string mapSettingsPath;

    public float[] timeToFinish;

    public float offSet = -5;
    public float offsetPerLane = -1.0f;

    public HorseController[] horseControllers;

    public Transform horsePosition;
    public CinemachineTargetGroup horseGroup;
    public Transform followTarget;
    public float RaceTime { get; private set; }
    public event Action OnFinishTrackEvent = ActionUtility.EmptyAction.Instance;

    public float averageSpeed = 24.0f;
    public float averageTimeToFinish;
    public Vector2 timeOffSet = new Vector2(-1.0f, 1.0f);

    public float raceLength;
    public int playerHorseIndex;
    private RacingTrackController racingTrackController;
    private bool isStartedRace;
    private bool isHorsesLoaded;
    private CancellationToken token;
    public Transform WarmUpTarget => this.horseGroup.transform;

    public async UniTask InitializeAsync(HorseMeshInformation[] horseMeshControllerPaths,
                                         string mapSettingPath,
                                         string mapPath,
                                         int playerHorseIndex,
                                         float[] times,
                                         int totalLap,
                                         HorseRaceInfo[] horseRaceTimes,
                                         CancellationToken token)
    {
        this.mapPath = mapPath;
        this.mapSettingsPath = mapSettingPath;
        this.horseControllerModelPaths = horseMeshControllerPaths;
        this.playerHorseIndex = playerHorseIndex;
        var tops = times.GetTopByTimes();
        await LoadMapSettings(token);
        await LoadRacingScene(token);
        await LoadHorses(horseMeshControllerPaths, token);
        CalculateRaceStat(times, totalLap);
        SetHorseControllerStat(tops, horseRaceTimes);
        RaceTime = GetMinimumRaceTime(horseRaceTimes);
        isHorsesLoaded = true;
        this.token = token;
    }

    public void PrepareWarmUp()
    {
        horseControllers.ForEach(x => { x.gameObject.SetActive(true); });
    }

    private async UniTask LoadMapSettings(CancellationToken token)
    {
        var mapSettings = await PrimitiveAssetLoader.LoadAssetAsync<MapSettings>(mapSettingsPath, token);

        raceCamera = Instantiate(GetRaceModeCamera(mapSettings), Vector3.zero, Quaternion.identity, transform);
        raceCamera.gameObject.SetActive(false);
        raceCamera.SetHorseGroup(this.followTarget.transform);

        SubscribeToVcamsFading(true);

        targetGenerator = Instantiate(mapSettings.targetGenerator, Vector3.zero,Quaternion.identity, transform);
        horsePosition.position = targetGenerator.StartPosition;
    }

    private void SubscribeToVcamsFading(bool enable)
    {
        if (raceCamera == default) return;
        
        var allVcams = raceCamera.vCameraContainer.GetComponentsInChildren<CinemachineVirtualCamera>(true)
                                 .Select(x => x.name)
                                 .ToList();

        var needFadingBlendingCameras = GetNeedFadingBlendingCameras()
            .Select(x => allVcams.FindIndex(vCam => vCam == x));
        var activateCameras = raceCamera.triggerContainer.GetComponentsInChildren<ActivateCamera>(true);
        activateCameras.Where((x, i) => needFadingBlendingCameras.Contains(i + 1 % activateCameras.Length))
                       .ForEach(x =>
                       {
                           if (enable)
                           {
                               x.OnBeginActivateCameraEvent += cameraBlendingAnimation.FadeInAnimationAsync;
                               x.OnActivateCameraEvent += cameraBlendingAnimation.FadeOutAnimationAsync;    
                           }
                           else
                           {
                               x.OnBeginActivateCameraEvent -= cameraBlendingAnimation.FadeInAnimationAsync;
                               x.OnActivateCameraEvent -= cameraBlendingAnimation.FadeOutAnimationAsync;    
                           }
                       });
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
    
    private async UniTask LoadRacingScene(CancellationToken token)
    {
        
        mapScene = await SceneAssetLoader.LoadSceneAsync(mapPath, true, token : token);
        racingTrackController = mapScene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<RacingTrackController>(true))
            .FirstOrDefault();
    }

    private async UniTask LoadHorses(HorseMeshInformation[] horseControllerPaths, CancellationToken token)
    {
        horseControllers = await UniTask.WhenAll(horseControllerPaths.Select(x => LoadHorseController(x, token)));
        horseControllers.ForEach(x => x.gameObject.SetActive(false));
    }

    private async UniTask<HorseController> LoadHorseController(HorseMeshInformation horseControllerPath, CancellationToken token)
    {
        var horse = await HorseMeshAssetLoader.InstantiateHorse(horseControllerPath, token);
        horse.transform.SetParent(horsePosition);
        var horseController = horse.GetComponent<HorseController>();
        horseGroup.AddMember(horseController.transform, 1, 0);
        return horseController;
    }

    public void StartRace()
    {
        if (isStartedRace == false)
        {
            isStartedRace = true;
            horseControllers.ForEach(x => x.StartRaceAsync().Forget());
        }
    }

    public void EnablePostProcessing(bool enable)
    {
        mainCamera.GetComponent<PostProcessVolume>().enabled = enable;
    }

    public async UniTask WaitToStart()
    {
        mainCamera.SetActive(true);
        raceCamera.gameObject.SetActive(true);
        racingTrackController?.PlayStartAnimation();
        await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);
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

    private void SetHorseControllerStat(int[] topInRaceMatch, HorseRaceInfo[] horseRaceTimes)
    {
        horseRaceTimes.OrderBy(x => x.RaceSegments.First().ToLane)
            .ForEach((x, i) =>
            {
                var index = i;
                var horseController = horseControllers[i];
                horseController.SetHorseData(new HorseInGameData()
                {
                    TargetGenerator = targetGenerator,
                    CurrentOffset = offSet + i * offsetPerLane,
                    TopInRaceMatch = topInRaceMatch[i],
                    IsPlayer = playerHorseIndex == i,
                    InitialLane = i,
                    OnFinishTrack = () => OnFinishTrack(topInRaceMatch[index]),
                    PredefineTargets = CalculatePredefineTarget(horseRaceTimes[i]),
                    MainCamera = mainCamera,
                    Delay = x.DelayTime,
                    Name = x.Name
                });
                horseController.gameObject.name = x.Name;
            });
    }

    private float GetMinimumRaceTime(HorseRaceInfo[] horseRaceTimes)
    {
        return horseRaceTimes.Min(x => x.RaceSegments.Sum(raceSegment => raceSegment.Time));
    }

    private ((Vector3 , Quaternion, float)[], int finishIndex) CalculatePredefineTarget(HorseRaceInfo horseRaceInfo)
    {
        return targetGenerator.GenerateTargets(horseRaceInfo.RaceSegments);
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

    private void Update()
    {
        if (isHorsesLoaded)
        {
            var firstHorse = isStartedRace
                ? horseControllers.OrderByDescending(x => x.CurrentRaceProgressWeight)
                                  .First()
                                  .transform
                : horseControllers[horseControllers.Length / 2]
                    .transform;
            followTarget.position = Vector3.Lerp(followTarget.position, firstHorse.transform.position, Time.deltaTime * 15.0f);
        }
    }
    
    public void Dispose()
    {
        if(mapScene != default)
        {
            SceneAssetLoader.UnloadAssetAtPath(mapPath);
            mapScene = default;
        }

        SubscribeToVcamsFading(false);
        
        this.horseControllerModelPaths?.Select(x => x.horseModelPath)
            .ToList()
            .ForEach(PrimitiveAssetLoader.UnloadAssetAtPath);
        PrimitiveAssetLoader.UnloadAssetAtPath(mapSettingsPath);
        Time.timeScale = 1;
        racingTrackController = default;
    }
}
