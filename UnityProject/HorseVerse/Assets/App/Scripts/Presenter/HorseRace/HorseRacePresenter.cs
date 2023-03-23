using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public partial class HorseRacePresenter : IDisposable
{
    private IHorseRaceManager horseRaceManager;
    private UIFlashScreenAnimation uiFlashScreen;
    private UILoading uiLoading;
    private Scene mapScene;
    public event Action OnToBetModeResultState = ActionUtility.EmptyAction.Instance;
    public event Action OnToQuickRaceModeResultState = ActionUtility.EmptyAction.Instance;
    private HorseRaceContext horseRaceContext;
    private IHorseRaceManagerFactory horseRaceManagerFactory;
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
    private HorseRaceStatusPresenter horseRaceStatusPresenter;

    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    private IHorseRaceManagerFactory HorseRaceManagerFactory => horseRaceManagerFactory ??= Container.Inject<IHorseRaceManagerFactory>();
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
        await LoadRacingScene();
        await LoadUIAsync();
        horseRaceManager = await HorseRaceManagerFactory.CreateHorseRaceManagerAsync(cts.Token);
        raceModeHorseIntroPresenter = await RaceModeHorseIntroPresenter.InstantiateAsync(Container, targetGenerator.StartPosition, Quaternion.identity, cts.Token);
        await UniTask.Delay(500, cancellationToken: token);
    }

    private async UniTask LoadRacingScene()
    {
        mapScene = await SceneAssetLoader.LoadSceneAsync(masterMap.MapPath, true, token : token);
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
        
        await (raceModeHorseIntroPresenter.ShowHorsesInfoIntroAsync(HorseRaceContext.HorseBriefInfos)
                                                           .AttachExternalCancellation(token),
            uiLoading.Out());
        await uiLoading.In();
        
        DisposeUtility.SafeDispose(ref raceModeHorseIntroPresenter);
        horseRaceManager.PrepareToRace();
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
        masterMapContainer = await MasterLoader.LoadMasterAsync<MasterMapContainer>(token : cts.Token);
        masterMap = masterMapContainer.MasterMapIndexer[HorseRaceContext.MasterMapId];
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
        SetEntityHorseRaceManager();
        SetHorseStatusAsync();
        AudioManager.Instance.PlaySoundHasLoop(AudioManager.HorseRunRacing);
    }

    private void SetHorseStatusAsync()
    {
        horseRaceStatusPresenter = new HorseRaceStatusPresenter(horseRaceManager, 
            horseRaceManager.HorseControllers,
            horseRaceManager.PlayerHorseIndex,
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
        
        horseRaceManager.OnFinishTrackEvent -= OnFinishTrack;
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

    public void FixedUpdate()
    {
        // horseRaceManager?.UpdateRaceTime();
        horseRaceStatusPresenter?.UpdateRaceStatus();
    }

    public void Dispose()
    {
#if ENABLE_DEBUG_MODULE
        RemoveDebuggerAction();
#endif
        if(mapScene != default)
        {
            SceneAssetLoader.UnloadAssetAtPath(masterMap.MapPath);
            mapScene = default;
        }
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

        DisposeUtility.SafeDispose(ref raceModeHorseIntroPresenter);
        AudioManager.Instance.StopSound();
        Time.timeScale = 1.0f;
    }
}
