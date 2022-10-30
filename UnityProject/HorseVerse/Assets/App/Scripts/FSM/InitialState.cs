using RobustFSM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class InitialState : InjectedBHState, IDisposable
{
#if ENABLE_DEBUG_MODULE    
    private UIDebugMenuPresenter uiDebugMenuPresenter;
#endif
    
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
        this.Container.Bind(new HorseDetailEntityFactory(Container));
        this.Container.Bind(new LocalQuickRaceDomainService(this.Container));
        this.Container.Bind(new LocalTraningDomainService(Container));
        this.Container.Bind(new HorseSumaryListEntityFactory(Container));
        this.Container.Bind(TCPSocketClient.Initialize(new ProtobufMessageParser()));
        base.Enter();
        
        uiDebugMenuPresenter ??= new UIDebugMenuPresenter(Container);
        uiDebugMenuPresenter.InitializeAsync().Forget();
    }

    public override void AddStates()
    {
        base.AddStates();

        AddState<LoadingState>();

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
        this.Container.RemoveAndDisposeIfNeed<TCPSocketClient>();
        this.Container.RemoveAndDisposeIfNeed<UILoadingPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIHeaderPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIBackGroundPresenter>();
        this.Container.RemoveAndDisposeIfNeed<UIHorse3DViewPresenter>();
        this.Container.RemoveAndDisposeIfNeed<HorseDetailEntityFactory>();
        this.Container.RemoveAndDisposeIfNeed<LocalQuickRaceDomainService>();
        this.Container.RemoveAndDisposeIfNeed<LocalTraningDomainService>();
        this.Container.RemoveAndDisposeIfNeed<MasterHorseContainer>();
        this.Container.RemoveAndDisposeIfNeed<HorseRepository>();
        this.Container.RemoveAndDisposeIfNeed<BetRateRepository>();
        this.Container.RemoveAndDisposeIfNeed<UserDataRepository>();
        this.Container.RemoveAndDisposeIfNeed<HorseSumaryListEntityFactory>();
        this.Container.RemoveAndDisposeIfNeed<MasterHorseContainer>();
        MasterLoader.Unload<MasterHorseContainer>();
        uiDebugMenuPresenter.Dispose();
        uiDebugMenuPresenter = default;
    }
}