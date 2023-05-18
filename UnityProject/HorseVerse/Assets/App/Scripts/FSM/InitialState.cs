using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class InitialState : InjectedBHState, IDisposable
{
    private UIHeaderPresenter uiHeaderPresenter;
    private StartUpStatePresenter StartUpStatePresenter => Container.Inject<StartUpStatePresenter>();
    public override void Enter()
    {
        OnEnterStateAsync().Forget();
    }

    private async UniTaskVoid OnEnterStateAsync()
    {
        this.Container.Bind(await MasterLoader.LoadMasterAsync<MasterHorseContainer>());
        this.Container.Bind(new AudioPresenter(Container));
        this.Container.Bind(new HorseRepository(Container));
        this.Container.Bind(new BetRateRepository());
        this.Container.Bind(new UserDataRepository());
        this.Container.Bind(new UILoadingPresenter());
        uiHeaderPresenter = new UIHeaderPresenter(Container);
        this.Container.Bind(uiHeaderPresenter);
        this.Container.Bind(new UIBackGroundPresenter(Container));
        this.Container.Bind(new UIHorse3DViewPresenter(Container));
        this.Container.Bind(new UIHorseInfo3DViewPresenter(Container)); //Use for show other horse
        this.Container.Bind(new HorseDetailEntityFactory(Container));
        this.Container.Bind(new QuickRaceDomainService(Container));
        this.Container.Bind(new TrainingDomainService(Container));
        this.Container.Bind(new HorseSumaryListEntityFactory(Container));
        var masterErrorCodeContainer = Container.Inject<MasterErrorCodeContainer>();
#if UNITY_WEBGL || WEB_SOCKET
        this.Container.Bind(WebSocketClient.Initialize(new ProtobufMessageParser(), ErrorCodeConfiguration.Initialize(masterErrorCodeContainer)));
#else
        this.Container.Bind(TCPSocketClient.Initialize(new ProtobufMessageParser(), ErrorCodeConfiguration.Initialize(masterErrorCodeContainer)));
#endif
        this.Container.Bind(await UITouchDisablePresenter.InstantiateAsync(Container));
        this.Container.Bind(PingDomainService.Instantiate(Container));
        this.Container.Bind(new FeaturePresenter(Container));
        this.Container.Bind(new HorseRaceContext());
        this.Container.Bind(new HorseRaceInfoFactory(Container));
        this.Container.Bind(RetryHandler.Instantiate(Container));
        this.Container.Bind(LoginDomainService.Instantiate(Container));
        this.Container.Bind(new NotificationPresenter(Container));

        uiHeaderPresenter.OnLogOut += OnLogOut;
        base.Enter();

    }

    private void OnLogOut()
    {
#if MULTI_ACCOUNT
        var indexToken = PlayerPrefs.GetString(GameDefine.TOKEN_CURRENT_KEY_INDEX, "");
        PlayerPrefs.DeleteKey(GameDefine.TOKEN_STORAGE + indexToken);
        PlayerPrefs.DeleteKey(GameDefine.TOKEN_CURRENT_KEY_INDEX);
#else
        PlayerPrefs.DeleteKey(GameDefine.TOKEN_STORAGE);
#endif
        AudioManager.Instance?.StopMusic();
        StartUpStatePresenter.Reboot();
    }

    public override void AddStates()
    {
        base.AddStates();

        AddState<LoadingState>();
        AddState<LoginState>();
        AddState<RacingState>();
        AddState<HorseRaceActionState>();
        AddState<BetModeState>();
        AddState<MainMenuState>();
        AddState<TrainingState>();
        AddState<StableState>();
        SetInitialState<LoadingState>();
    }

    public override void Exit()
    {
        base.Exit();
        Dispose();
    }

    public void Dispose()
    {
        uiHeaderPresenter.OnLogOut -= OnLogOut;
        this.Container.RemoveAndDisposeIfNeed<HorseRaceInfoFactory>();
        this.Container.RemoveAndDisposeIfNeed<HorseRaceContext>();
        this.Container.RemoveAndDisposeIfNeed<FeaturePresenter>();
        this.Container.RemoveAndDisposeIfNeed<NotificationPresenter>();
        this.Container.RemoveAndDisposeIfNeed<LoginDomainService>();
        this.Container.RemoveAndDisposeIfNeed<RetryHandler>();
        this.Container.RemoveAndDisposeIfNeed<PingDomainService>();
        this.Container.RemoveAndDisposeIfNeed<UITouchDisablePresenter>();
#if UNITY_WEBGL || WEB_SOCKET
        this.Container.RemoveAndDisposeIfNeed<WebSocketClient>();
#else
        this.Container.RemoveAndDisposeIfNeed<TCPSocketClient>();
#endif
        this.Container.RemoveAndDisposeIfNeed<AudioPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UILoadingPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIHeaderPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIBackGroundPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIHorse3DViewPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIHorseInfo3DViewPresenter>();
        this.Container.RemoveAndDisposeIfNeed<HorseDetailEntityFactory>();
        this.Container.RemoveAndDisposeIfNeed<QuickRaceDomainService>();
        this.Container.RemoveAndDisposeIfNeed<TrainingDomainService>();
        this.Container.RemoveAndDisposeIfNeed<MasterHorseContainer>();
        this.Container.RemoveAndDisposeIfNeed<HorseRepository>();
        this.Container.RemoveAndDisposeIfNeed<BetRateRepository>();
        this.Container.RemoveAndDisposeIfNeed<UserDataRepository>();
        this.Container.RemoveAndDisposeIfNeed<HorseSumaryListEntityFactory>();
        this.Container.RemoveAndDisposeIfNeed<MasterHorseContainer>();
        MasterLoader.Unload<MasterHorseContainer>();
        uiHeaderPresenter = default;
    }
}