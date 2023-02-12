using Cysharp.Threading.Tasks;

#if ENABLE_DEBUG_MODULE
public partial class HorseRacePresenter
{
    private BetModeUIDebugMenuPresenter betModeUIDebugMenuPresenter;
    private UIDebugMenuPresenter uiDebugMenuPresenter;
    
    private BetModeUIDebugMenuPresenter BetModeUIDebugMenuPresenter => betModeUIDebugMenuPresenter ??= Container.Inject<BetModeUIDebugMenuPresenter>();
    private UIDebugMenuPresenter UIDebugMenuPresenter => uiDebugMenuPresenter ??= Container.Inject<UIDebugMenuPresenter>();

    private void CreateDebuggerAction()
    {
        if (HorseRaceContext.GameMode == HorseGameMode.Bet)
        {
            BetModeUIDebugMenuPresenter.AddSkipRaceMenu(() => OnShowResult().Forget());
        }
        else
        {
            UIDebugMenuPresenter.AddDebugMenu("RaceMode/Skip",() => OnShowResult().Forget());
        }
    }

    private void RemoveDebuggerAction()
    {
        if (HorseRaceContext.GameMode == HorseGameMode.Bet)
        {
            BetModeUIDebugMenuPresenter.RemoveSkipRaceMenu();
        }
        else
        {
            UIDebugMenuPresenter.RemoveDebugMenu("RaceMode/Skip");
        }

        betModeUIDebugMenuPresenter = default;
        uiDebugMenuPresenter = default;
    }
}
#endif