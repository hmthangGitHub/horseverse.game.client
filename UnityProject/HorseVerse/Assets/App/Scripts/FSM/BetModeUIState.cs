// #define MOCK_DATA
using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System;

public class BetModeUIState : InjectedBHState
{
    private UIBetModePresenter uiBetModePresenter;
    private UIHeaderPresenter uiHeaderPresenter;
    private UILoadingPresenter uiLoadingPresenter;
    private UILoadingPresenter UILoadingPresenter => uiLoadingPresenter ??= Container.Inject<UILoadingPresenter>();
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private UIBackGroundPresenter uiBackGroundPresenter;
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();
    
    public override void Enter()
    {
        base.Enter();
#if MOCK_DATA
        Container.Bind(new LocalBetHistoryRepository());
#else
        Container.Bind(new BetHistoryRepository(Container.Inject<ISocketClient>()));
#endif
        Container.Bind(new BetModeHistoryPresenter(Container));
        OnEnterStateAsync().Forget();
    }

    public override void AddStates()
    {
        base.AddStates();
        AddState<BetModeUIEntryState>();
        AddState<BetModeHistoryState>();
        SetInitialState<BetModeUIEntryState>();
    }

    private async UniTaskVoid OnEnterStateAsync()
    {
        uiBetModePresenter = new UIBetModePresenter(Container);
        uiBetModePresenter.OnBack += OnBackToMainMenu;
        uiBetModePresenter.OnToRaceMode += OnToRaceMode;
        uiBetModePresenter.OnTimeOut += OnTimeOut;
        uiBetModePresenter.OnViewHistory += OnViewHistory;
        
        await UIBackGroundPresenter.ShowBackGroundAsync();
        await uiBetModePresenter.ShowUIBetModeAsync();
    }

    private void OnViewHistory()
    {
        ChangeState<BetModeHistoryState>();
    }

    private void OnTimeOut()
    {
        ((RootFSM)SuperMachine).ChangeToChildStateRecursive<BetModeState>();
    }

    private void OnToRaceMode()
    {
        OnToRaceModeAsync().Forget();
    }
    
    private async UniTask OnToRaceModeAsync()
    {
        await UILoadingPresenter.ShowLoadingAsync();
        UIHeaderPresenter.ReleaseHeaderUI();
        this.Machine.ChangeState<HorseRaceActionState>();
    }

    private void OnBackToMainMenu()
    {
        this.GetMachine<BetModeState>().GetMachine<InitialState>().ChangeState<MainMenuState>();
    }

    public override void Exit()
    {
        base.Exit();
        uiBetModePresenter.OnBack -= OnBackToMainMenu;
        uiBetModePresenter.OnToRaceMode -= OnToRaceMode;
        uiBetModePresenter.OnTimeOut -= OnBackToMainMenu;
        uiBetModePresenter.OnViewHistory -= OnViewHistory;
        uiBetModePresenter?.Dispose();
        uiBetModePresenter = default;
        
#if MOCK_DATA
        Container.RemoveAndDisposeIfNeed<LocalBetHistoryRepository>();
#else
        Container.RemoveAndDisposeIfNeed<BetHistoryRepository>();
#endif
        Container.RemoveAndDisposeIfNeed<BetModeHistoryPresenter>();
    }
}