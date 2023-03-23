using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class HorseRaceThirdPersonManager : IHorseRaceManager
{
    private string mapSettingsPath;
    private MapSettings mapSettings;
    private TargetGenerator targetGenerator;
    private bool isStarted;
    private HorseRaceThirdPersonListContainer horseRaceThirdPersonListContainer;
    private CancellationTokenSource cts;
    private UIHorseRacingController uiHorseRacingController;
    private UIHorseRacingPreGameTiming uiHorseRacingPreGameTiming;
    public Transform WarmUpTarget => horseRaceThirdPersonListContainer.WarmUpTarget;
    public IHorseRaceInGameStatus[] HorseControllers => HorseRaceThirdPersonBehaviours;
    private HorseRaceThirdPersonBehaviour[] HorseRaceThirdPersonBehaviours { get; set; }
    public float NormalizedRaceTime { get; private set; }
    public int PlayerHorseIndex { get; private set; }
    public event Action OnFinishTrackEvent;
    private UIHorseRacingTimingType.TimingType timingType;
    
    public async UniTask WaitToStart()
    {
        var ucs = new UniTaskCompletionSource();
        void OnTiming(UIHorseRacingTimingType.TimingType timingType)
        {
            this.timingType = timingType;
            ucs.TrySetResult();
        }
        uiHorseRacingPreGameTiming.SetEntity(new UIHorseRacingPreGameTiming.Entity()
        {
            timingType = OnTiming
        });
        await uiHorseRacingPreGameTiming.In().AttachExternalCancellation(cts.Token);
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    public void PrepareToRace()
    {
        SetHorsesVisible(true);
    }

    public void StartRace()
    {
        isStarted = true;
        HorseRaceThirdPersonBehaviours.ForEach(x => x.StartRace(0.0f));
        UpdateRaceProgressAsync().Forget();
    }

    public void UpdateRaceTime()
    {
        // isStarted
    }

    private async UniTaskVoid UpdateRaceProgressAsync()
    {
        // while (!cts.IsCancellationRequested)
        // {
        //     // NormalizedRaceTime = HorseRaceThirdPersonBehaviours.Min(x => x.CurrentRaceProgressWeight);
        //     await UniTask.NextFrame(timing: PlayerLoopTiming.FixedUpdate, cancellationToken: cts.Token);
        // }
    }

    public async UniTask InitializeAsync(MasterHorseContainer masterHorseContainer,
                                         string mapSettingPath,
                                         int playerHorseIndex,
                                         HorseRaceThirdPersonInfo[] horseRaceThirdPersonInfo,
                                         CancellationToken token)
    {
        PlayerHorseIndex = playerHorseIndex;
        this.mapSettingsPath = mapSettingPath;
        await LoadMapSettingsAsync(token);
        await LoadUIAsync(token);
        
        horseRaceThirdPersonListContainer = Object.Instantiate((await Resources.LoadAsync<HorseRaceThirdPersonListContainer>("GamePlay/HorseRaceThirdPersonManager") as HorseRaceThirdPersonListContainer));
        HorseRaceThirdPersonBehaviours = await horseRaceThirdPersonInfo.Select((x, i) => LoadHorseController(i, masterHorseContainer, x, token));
        SetHorsesVisible(false);
        
        cts = new CancellationTokenSource();
    }

    private async UniTask LoadUIAsync(CancellationToken token)
    {
        uiHorseRacingPreGameTiming = await UILoader.Instantiate<UIHorseRacingPreGameTiming>(token: token);
        uiHorseRacingController = await UILoader.Instantiate<UIHorseRacingController>(token: token);
    }

    private void SetHorsesVisible(bool isVisible)
    {
        horseRaceThirdPersonListContainer.gameObject.SetActive(isVisible);
    }

    private async UniTask LoadMapSettingsAsync(CancellationToken token)
    {
        mapSettings = await PrimitiveAssetLoader.LoadAssetAsync<MapSettings>(mapSettingsPath, token);
        targetGenerator = Object.Instantiate(mapSettings.targetGenerator, Vector3.zero, Quaternion.identity);
    }

    private async UniTask<HorseRaceThirdPersonBehaviour> LoadHorseController(int initialLane, MasterHorseContainer masterHorseContainer, HorseRaceThirdPersonInfo horseRaceThirdPersonInfo, CancellationToken token)
    {
        var horse = await HorseMeshAssetLoader.InstantiateHorse(masterHorseContainer.GetHorseMeshInformation(horseRaceThirdPersonInfo.MeshInformation, HorseModelMode.RaceThirdPerson) , token);
        horse.transform.SetParent(horseRaceThirdPersonListContainer.HorsesContainer);
        var horseController = horse.GetComponent<HorseRaceThirdPersonBehaviour>();
        horseRaceThirdPersonListContainer.HorseGroup.AddMember(horseController.transform, 1, 0);
        horseController.HorseRaceThirdPersonData = new HorseRaceThirdPersonData()
        {
            TargetGenerator = targetGenerator,
            InitialLane = initialLane,
            IsPlayer = PlayerHorseIndex == initialLane,
            PredefineWayPoints = targetGenerator.GenerateRandomTargetsWithNoise(),
            HorseRaceThirdPersonStats = horseRaceThirdPersonInfo.HorseRaceThirdPersonStats
        };
        return horseController;
    }
    
    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        DisposeUtility.SafeDisposeMonoBehaviour(ref horseRaceThirdPersonListContainer);
        Object.Destroy(mapSettings);
        mapSettings = default;
        PrimitiveAssetLoader.UnloadAssetAtPath(mapSettingsPath);
        UILoader.SafeRelease(ref uiHorseRacingPreGameTiming);
        UILoader.SafeRelease(ref uiHorseRacingController);
        DisposeUtility.SafeDisposeMonoBehaviour(ref targetGenerator);
        Time.timeScale = 1;
    }
}
