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
    private HorseRaceStatusPresenter horseRaceStatusPresenter;

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
                                                FindIndex(x => HorseRepository.Models.ContainsKey(x.NftHorseId));
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
        var horseIdInLanes = RandomHorseInLanes();
        SetEntityHorseRaceManager();
        SetHorseStatusAsync(horseIdInLanes);
        AudioManager.Instance.PlaySoundHasLoop(AudioManager.HorseRunRacing);
    }

    private void SetHorseStatusAsync(int[] horseIdInLanes)
    {
        horseRaceStatusPresenter = new HorseRaceStatusPresenter(horseRaceManager.horseControllers,
            horseIdInLanes,
            playerHorseIndex, horseRaceManager.RaceTime,
            horseRaceContext.GameMode == HorseGameMode.Race && horseRaceContext.RaceMatchDataContext.IsReplay,
            HorseRaceContext.GameMode == HorseGameMode.Race);
        horseRaceStatusPresenter.Initialize().Forget();
        horseRaceStatusPresenter.OnSkip += ShowResult;
    }

    private void SetEntityHorseRaceManager()
    {
        horseRaceManager.StartRace();
        horseRaceManager.OnFinishTrackEvent += OnFinishTrack;
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

    private void ShowResult()
    {
        OnShowResult().Forget();
    }
    
    private async UniTask OnShowResult()
    {
        statusCts.SafeCancelAndDispose();
        if(horseRaceManager != default)
            horseRaceManager.OnFinishTrackEvent -= OnFinishTrack;
        if(uiHorseRaceStatus != default)
            uiHorseRaceStatus.Out().Forget();
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
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), ignoreTimeScale: true, cancellationToken: cts.Token);
        await uiFlashScreen.In().AttachExternalCancellation(cts.Token);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), ignoreTimeScale: true, cancellationToken: cts.Token);
        Time.timeScale = currentTimeScale;
        await uiFlashScreen.Out().AttachExternalCancellation(cts.Token);
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
        if(horseRaceStatusPresenter != default) horseRaceStatusPresenter.OnSkip -= ShowResult;
        
        DisposeUtility.SafeDispose(ref statusCts);
        DisposeUtility.SafeDispose(ref cts);
        DisposeUtility.SafeDispose(ref horseRaceManager);
        DisposeUtility.SafeDispose(ref horseRaceStatusPresenter);
        DisposeUtility.SafeDisposeMonoBehaviour(ref targetGenerator);

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
        Time.timeScale = 1.0f;
    }
}
