#define DEVELOPMENT
using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class HorseRaceManager : MonoBehaviour, IHorseRaceManager
{
    public RaceModeCameras raceCamera;
    public TargetGenerator targetGenerator;

    public GameObject mainCamera;
    public CameraBlendingAnimation cameraBlendingAnimation;

    private HorseMeshInformation[] horseControllerModelPaths;
    private string mapSettingsPath;

    public float offSet = -5;
    public float offsetPerLane = -1.0f;

    private HorseController[] HorseControllerInternal { get; set; }
    public IHorseRaceInGameStatus[] HorseControllers => HorseControllerInternal;

    public Transform horsePosition;
    public CinemachineTargetGroup horseGroup;
    public Transform followTarget;
    private float raceTime;
    public float NormalizedRaceTime { get; set; }
    public int PlayerHorseIndex { get; private set; }
    public event Action OnFinishTrackEvent = ActionUtility.EmptyAction.Instance;

    private RacingTrackController racingTrackController;
    private bool isStartedRace;
    private bool isHorsesLoaded;
    private CancellationToken token;
    public Transform WarmUpTarget => this.horseGroup.transform;
    private float currentRaceTime = 0.0f;

    public async UniTask InitializeAsync(HorseMeshInformation[] horseMeshControllerPaths,
                                         string mapSettingPath,
                                         int playerHorseIndex,
                                         HorseRaceInfo[] horseRaceTimes,
                                         CancellationToken token)
    {
        this.PlayerHorseIndex = playerHorseIndex;
        this.mapSettingsPath = mapSettingPath;
        this.horseControllerModelPaths = horseMeshControllerPaths;
        this.token = token;
        await LoadMapSettings(token);
        await LoadHorses(horseMeshControllerPaths, token);
        SetHorseControllerStat(horseRaceTimes, playerHorseIndex);
        raceTime = GetMinimumRaceTime(horseRaceTimes);
        isHorsesLoaded = true;
    }


    public void PrepareToRace()
    {
        HorseControllerInternal.ForEach(x => { x.gameObject.SetActive(true); });
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
        return mapSettings.raceModeCamera.RandomElement();
    }

    private async UniTask LoadHorses(HorseMeshInformation[] horseControllerPaths, CancellationToken token)
    {
        HorseControllerInternal = await UniTask.WhenAll(horseControllerPaths.Select(x => LoadHorseController(x, token)));
        HorseControllerInternal.ForEach(x => x.gameObject.SetActive(false));
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
            HorseControllerInternal.ForEach(x => x.StartRaceAsync().Forget());
        }
    }

    public async UniTask WaitToStart()
    {
        mainCamera.SetActive(true);
        raceCamera.gameObject.SetActive(true);
        racingTrackController?.PlayStartAnimation();
        await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);
    }

    private void SetHorseControllerStat(HorseRaceInfo[] horseRaceTimes,
                                        int playerHorseIndex)
    {
        horseRaceTimes.OrderBy(x => x.RaceSegments.First().ToLane)
            .ForEach((x, i) =>
            {
                var index = i;
                var horseController = HorseControllerInternal[i];
                horseController.SetHorseData(new HorseInGameData()
                {
                    TargetGenerator = targetGenerator,
                    CurrentOffset = offSet + i * offsetPerLane,
                    IsPlayer = playerHorseIndex == i,
                    InitialLane = i,
                    OnFinishTrack = OnFinishTrack,
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

    private void OnFinishTrack()
    {
        OnFinishTrackEvent.Invoke();
    }

    private void Update()
    {
        UpdateFollowTarget();
        UpdateRaceTime();
    }

    private void UpdateFollowTarget()
    {
        if (isHorsesLoaded)
        {
            var firstHorse = isStartedRace
                ? HorseControllerInternal.OrderByDescending(x => x.CurrentRaceProgressWeight)
                                         .First()
                                         .transform
                : HorseControllerInternal[HorseControllers.Length / 2]
                    .transform;
            followTarget.position
                = Vector3.Lerp(followTarget.position, firstHorse.transform.position, Time.deltaTime * 15.0f);
        }
    }

    private void UpdateRaceTime()
    {
        if (!isStartedRace) return;
        currentRaceTime += Time.deltaTime;
        NormalizedRaceTime = Mathf.Clamp01(currentRaceTime / raceTime);
    }

    public void Dispose()
    {
        SubscribeToVcamsFading(false);
        
        this.horseControllerModelPaths?.Select(x => x.horseModelPath)
            .ToList()
            .ForEach(PrimitiveAssetLoader.UnloadAssetAtPath);
        PrimitiveAssetLoader.UnloadAssetAtPath(mapSettingsPath);
        Time.timeScale = 1;
        racingTrackController = default;
    }
}
