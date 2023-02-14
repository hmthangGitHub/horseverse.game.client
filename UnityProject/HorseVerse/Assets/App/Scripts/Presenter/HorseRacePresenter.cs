using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public partial class HorseRacePresenter : IDisposable
{
    private HorseRaceManager horseRaceManager;
    private UIHorseRaceStatus uiHorseRaceStatus;
    private UIFlashScreenAnimation uiFlashScreen;

    private int[] cachedPositions;
    private IReadOnlyUserDataRepository userDataRepository;
    private MasterHorseContainer masterHorseContainer;
    public event Action OnToBetModeResultState = ActionUtility.EmptyAction.Instance;
    public event Action OnToQuickRaceModeResultState = ActionUtility.EmptyAction.Instance;
    private int playerHorseIndex = -1;
    private HorseRaceContext horseRaceContext;
    private MasterMapContainer masterMapContainer;
    private MasterMap masterMap;
    private MapSettings mapSettings;
    private TargetGenerator targetGenerator;
    private CancellationTokenSource statusCts;
    private CancellationTokenSource cts;
    private readonly CancellationToken token;
    private RaceModeHorseIntroPresenter raceModeHorseIntroPresenter;
    private int numberOfHorseFinishTheRace = 0;
    private IReadOnlyHorseRepository horseRepository;

    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= Container.Inject<IReadOnlyUserDataRepository>();
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= Container.Inject<HorseRepository>();
    private IDIContainer Container { get; }

    public HorseRacePresenter(IDIContainer container)
    {
        Container = container;
        cts = new CancellationTokenSource();
        token = cts.Token;
    }

    public async UniTask LoadAssetAsync()
    {
        await GetMasterMap();
        await GetMapSettings();
        await InitHorseRaceAsync();
        await LoadUI();
        raceModeHorseIntroPresenter = new RaceModeHorseIntroPresenter(Container);
        await raceModeHorseIntroPresenter.LoadUIAsync();
        await UniTask.Delay(500, cancellationToken: token);
    }

    private async UniTask GetMapSettings()
    {
        mapSettings = await PrimitiveAssetLoader.LoadAssetAsync<MapSettings>(masterMap.MapSettings, default);
        targetGenerator = Object.Instantiate(mapSettings.targetGenerator, Vector3.zero, Quaternion.identity);
    }

    public async UniTask PlayIntro()
    {
#if ENABLE_DEBUG_MODULE
        CreateDebuggerAction();
#endif
        await horseRaceManager.ShowFreeCamera();
        horseRaceManager.EnablePostProcessing(true);
        await (horseRaceManager.cameraBlendingAnimation.FadeOutAnimationAsync(),
               raceModeHorseIntroPresenter.ShowHorsesInfoIntroAsync(HorseRaceContext.RaceScriptData.HorseRaceInfos.ToArray(), 
                                                                    targetGenerator.StartPosition, 
                                                                    Quaternion.identity)
                                          .AttachExternalCancellation(token));
        DisposeUtility.SafeDispose(ref raceModeHorseIntroPresenter);
        horseRaceManager.EnablePostProcessing(false);
        await horseRaceManager.cameraBlendingAnimation.FadeInAnimationAsync();
        
        await (horseRaceManager.ShowWarmUpCameraThenWait(),
               horseRaceManager.cameraBlendingAnimation.FadeOutAnimationAsync());
    }

    private async UniTask GetMasterMap()
    {
        masterMapContainer = await MasterLoader.LoadMasterAsync<MasterMapContainer>();
        masterMap = masterMapContainer.MasterMapIndexer[HorseRaceContext.RaceScriptData.MasterMapId];
    }

    private async UniTask InitHorseRaceAsync()
    {
        horseRaceManager ??= Object.Instantiate((await Resources.LoadAsync<HorseRaceManager>("GamePlay/HorseRaceManager") as HorseRaceManager));

        if (HorseRaceContext.GameMode == HorseGameMode.Race)
        {
            playerHorseIndex = HorseRaceContext.RaceScriptData.
                                                HorseRaceInfos.
                                                ToList().
                                                FindIndex(x => x.Name == HorseRepository.Models[UserDataRepository.Current.CurrentHorseNftId].Name);
        }
        
        await horseRaceManager.InitializeAsync(HorseRaceContext.RaceScriptData.HorseRaceInfos.Select(x => MasterHorseContainer.GetHorseMeshInformation(x.MeshInformation, HorseModelMode.Race)).ToArray(),
                                               masterMap.MapSettings,
                                               masterMap.MapPath,
                                               playerHorseIndex,
                                               HorseRaceContext.RaceScriptData.HorseRaceInfos.Select(x => x.RaceSegments.Sum(segment => segment.Time)).ToArray(),
                                               1,
                                               HorseRaceContext.RaceScriptData.HorseRaceInfos,
                                               token);
    }

    private async UniTask LoadUI()
    {
        uiHorseRaceStatus ??= await UILoader.Instantiate<UIHorseRaceStatus>();
        uiFlashScreen ??= await UILoader.Instantiate<UIFlashScreenAnimation>();
    }

    public void StartGame()
    {
        int[] horseIdInLanes = RandomHorseInLanes();
        SetEntityHorseRaceManager(horseIdInLanes);
        StartUpdateRaceHorseStatus().Forget();
        AudioManager.Instance.PlaySoundHasLoop(AudioManager.HorseRunRacing);
    }

    private async UniTaskVoid StartUpdateRaceHorseStatus()
    {
        statusCts = new CancellationTokenSource();
        using var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(statusCts.Token, token);
        while (!linkedToken.Token.IsCancellationRequested)
        {
            await UniTask.WaitForFixedUpdate(cancellationToken : linkedToken.Token);
            UpdateRaceStatus();
        }
    }

    private void SetEntityHorseRaceManager(int[] horseIdInLanes)
    {
        horseRaceManager.StartRace();
        horseRaceManager.OnFinishTrackEvent += OnFinishTrack;
        SetEntityUIHorseRaceStatus(horseIdInLanes, horseRaceManager.RaceTime);
    }

    private void OnFinishTrack()
    {
        OnFinishTrackAsync().Forget();
    }

    private async UniTask OnFinishTrackAsync()
    {
        if (numberOfHorseFinishTheRace < 2)
        {
            AudioManager.Instance.StopSound();
            await FlashScreenAsync();
            AudioManager.Instance.PlaySoundHasLoop(AudioManager.HorseRunRacing);
            numberOfHorseFinishTheRace++;
        }
        if (numberOfHorseFinishTheRace >= 2)
        {
            await OnShowResult();
        }
    }

    private async UniTask OnShowResult()
    {
        statusCts.SafeCancelAndDispose();
        horseRaceManager.OnFinishTrackEvent -= OnFinishTrack;
        uiHorseRaceStatus.Out()
                         .Forget();
        if (HorseRaceContext.GameMode == HorseGameMode.Race)
        {
            OnToQuickRaceModeResultState();
        }
        else
        {
            OnToBetModeResultState();
        }
    }

    private async UniTask FlashScreenAsync()
    {
        var currentTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), ignoreTimeScale: true);
        await uiFlashScreen.In();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), ignoreTimeScale: true);
        
        Time.timeScale = currentTimeScale;
        await uiFlashScreen.Out();
    }

    private void UpdateRaceStatus()
    {
        var horseControllersOrderByRank = horseRaceManager.horseControllers.OrderByDescending(x => x.CurrentRaceProgressWeight)
                                                             .Select(x => x)
                                                             .ToArray();
        for (var i = 0; i < horseControllersOrderByRank.Length; i++)
        {
            if (cachedPositions[i] != horseControllersOrderByRank[i].InitialLane)
            {
                uiHorseRaceStatus.playerList.ChangePosition(horseControllersOrderByRank[i].InitialLane, i);
                cachedPositions[i] = horseControllersOrderByRank[i].InitialLane;
            }
            
            if (i == 0) uiHorseRaceStatus.UpdateFirstRank(horseControllersOrderByRank[i].Name);
            if (i == 1) uiHorseRaceStatus.UpdateSecondRank(horseControllersOrderByRank[i].Name);
            if (horseControllersOrderByRank[i].IsPlayer) uiHorseRaceStatus.UpdateSelfRank(i);
        }
    }

    private void SetEntityUIHorseRaceStatus(int[] playerList, float timeToFinish)
    {
        cachedPositions = Enumerable.Repeat(-1, playerList.Length).ToArray();
        uiHorseRaceStatus.SetEntity(new UIHorseRaceStatus.Entity()
        {
            playerList = new HorseRaceStatusPlayerList.Entity()
            {
                horseIdInLane = playerList,
                playerId = playerHorseIndex,
            },
            finishTime = timeToFinish,
            selfRaceRankGroup = HorseRaceContext.GameMode == HorseGameMode.Race
        });
        uiHorseRaceStatus.In().Forget();
    }

    private int[] RandomHorseInLanes()
    {
        return Enumerable.Range(0,HorseRaceContext.RaceScriptData.HorseRaceInfos.Length).ToArray();
    }

    public void Dispose()
    {
#if ENABLE_DEBUG_MODULE
        RemoveDebuggerAction();
#endif
        DisposeUtility.SafeDispose(ref statusCts);
        DisposeUtility.SafeDispose(ref cts);
        DisposeUtility.SafeDispose(ref horseRaceManager);
        
        Object.Destroy(targetGenerator);
        targetGenerator = null;

        UILoader.SafeRelease(ref uiHorseRaceStatus);
        UILoader.SafeRelease(ref uiFlashScreen);
        MasterLoader.SafeRelease(ref masterMapContainer);

        if(mapSettings != default)
        {
            PrimitiveAssetLoader.UnloadAssetAtPath(masterMap.MapSettings);
            mapSettings = default;
        }

        masterMap = default;
        horseRaceContext = default;

        OnToBetModeResultState = ActionUtility.EmptyAction.Instance;
        OnToQuickRaceModeResultState = ActionUtility.EmptyAction.Instance;

        cachedPositions = default;
        masterHorseContainer = default;
        
        DisposeUtility.SafeDispose(ref raceModeHorseIntroPresenter);
        AudioManager.Instance.StopSound();
    }
}
