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
    private UIFlashScreenAnimation uiFlashScreen;
    private UILoading uiLoading;

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
    private HorseIntroCameraPresenter horseIntroCameraPresenter;
    private int numberOfHorseFinishTheRace = 0;
    private IReadOnlyHorseRepository horseRepository;
    private HorseRaceStatusPresenter horseRaceStatusPresenter;

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
        await LoadUIAsync();
        raceModeHorseIntroPresenter = await RaceModeHorseIntroPresenter.InstantiateAsync(Container, targetGenerator.StartPosition, Quaternion.identity, cts.Token);
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
        await horseIntroCameraPresenter.ShowFreeCamera();
        await uiLoading.In();
        horseIntroCameraPresenter.HideFreeCamera();
        
        horseRaceManager.EnablePostProcessing(true);
        await (raceModeHorseIntroPresenter.ShowHorsesInfoIntroAsync(HorseRaceContext.RaceScriptData.HorseRaceInfos)
                                                           .AttachExternalCancellation(token),
            uiLoading.Out());
        await uiLoading.In();
        
        DisposeUtility.SafeDispose(ref raceModeHorseIntroPresenter);
        horseRaceManager.EnablePostProcessing(false);
        horseRaceManager.PrepareWarmUp();
        
        await (horseIntroCameraPresenter.ShowWarmUpCamera(horseRaceManager.WarmUpTarget), 
            uiLoading.Out());
        await uiLoading.In();
        horseIntroCameraPresenter.HideWarmUpCamera();

        await (horseRaceManager.WaitToStart(), UniTask.Create(async () =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cts.Token);
            await uiLoading.Out();
        }));
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

    private async UniTask LoadUIAsync()
    {
        uiFlashScreen ??= await UILoader.Instantiate<UIFlashScreenAnimation>(token: cts.Token);
        uiLoading ??= await UILoader.Instantiate<UILoading>(token: cts.Token);
        uiLoading.SetEntity(new UILoading.Entity(){ loadingHorse = false});
        horseIntroCameraPresenter ??= await HorseIntroCameraPresenter.InstantiateAsync(mapSettings.freeCamera, mapSettings.warmUpCamera, cts.Token);
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
        horseRaceStatusPresenter.OnSkip += OnShowResult;
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
            OnShowResult();
        }
    }

    private void OnShowResult()
    {
        statusCts.SafeCancelAndDispose();
        if(horseRaceManager != default) horseRaceManager.OnFinishTrackEvent -= OnFinishTrack;
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
        if(horseRaceStatusPresenter != default) horseRaceStatusPresenter.OnSkip -= OnShowResult;
        
        DisposeUtility.SafeDispose(ref statusCts);
        DisposeUtility.SafeDispose(ref cts);
        DisposeUtility.SafeDispose(ref horseRaceManager);
        DisposeUtility.SafeDispose(ref horseRaceStatusPresenter);
        DisposeUtility.SafeDispose(ref horseIntroCameraPresenter);
        DisposeUtility.SafeDisposeMonoBehaviour(ref targetGenerator);

        UILoader.SafeRelease(ref uiFlashScreen);
        UILoader.SafeRelease(ref uiLoading);
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

        masterHorseContainer = default;
        
        DisposeUtility.SafeDispose(ref raceModeHorseIntroPresenter);
        AudioManager.Instance.StopSound();
        Time.timeScale = 1.0f;
    }
}
