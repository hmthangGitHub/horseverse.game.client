using RobustFSM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class InitialState : InjectedBHState, IDisposable
{
    public override void Enter()
    {
        OnEnterStateAsync().Forget();
    }

    private async UniTaskVoid OnEnterStateAsync()
    {
        this.Container.Bind(await MasterLoader.LoadMasterAsync<MasterHorseContainer>());
        this.Container.Bind(new HorseRepository(Container));
        this.Container.Bind(new BetRateRepository());
        this.Container.Bind(new UserDataRepository());
        this.Container.Bind(new UILoadingPresenter());
        this.Container.Bind(new UIHeaderPresenter(Container));
        this.Container.Bind(new UIBackGroundPresenter(Container));
        this.Container.Bind(new UIHorse3DViewPresenter(Container));
        this.Container.Bind(new UIHorseInfo3DViewPresenter(Container)); //Use for show other horse
        this.Container.Bind(new HorseDetailEntityFactory(Container));
        this.Container.Bind(new QuickRaceDomainService(Container));
        this.Container.Bind(new LocalTraningDomainService(Container));
        this.Container.Bind(new HorseSumaryListEntityFactory(Container));
        
#if UNITY_WEBGL || WEB_SOCKET
        this.Container.Bind(WebSocketClient.Initialize(new ProtobufMessageParser()));
#else
        this.Container.Bind(TCPSocketClient.Initialize(new ProtobufMessageParser()));
#endif
        this.Container.Bind(await UITouchDisablePresenter.InstantiateAsync(Container));
        base.Enter();

    }

    public override void AddStates()
    {
        base.AddStates();

        AddState<LoadingState>();
        AddState<LoginState>();
        AddState<QuickRaceState>();
        AddState<HorseRaceState>();
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
        this.Container.RemoveAndDisposeIfNeed<UITouchDisablePresenter>();
#if UNITY_WEBGL || WEB_SOCKET
        this.Container.RemoveAndDisposeIfNeed<WebSocketClient>();
#else
        this.Container.RemoveAndDisposeIfNeed<TCPSocketClient>();
#endif
        this.Container.RemoveAndDisposeIfNeed<UILoadingPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIHeaderPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIBackGroundPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIHorse3DViewPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIHorseInfo3DViewPresenter>();
        this.Container.RemoveAndDisposeIfNeed<HorseDetailEntityFactory>();
        this.Container.RemoveAndDisposeIfNeed<QuickRaceDomainService>();
        this.Container.RemoveAndDisposeIfNeed<LocalTraningDomainService>();
        this.Container.RemoveAndDisposeIfNeed<MasterHorseContainer>();
        this.Container.RemoveAndDisposeIfNeed<HorseRepository>();
        this.Container.RemoveAndDisposeIfNeed<BetRateRepository>();
        this.Container.RemoveAndDisposeIfNeed<UserDataRepository>();
        this.Container.RemoveAndDisposeIfNeed<HorseSumaryListEntityFactory>();
        this.Container.RemoveAndDisposeIfNeed<MasterHorseContainer>();
        MasterLoader.Unload<MasterHorseContainer>();
    }
}