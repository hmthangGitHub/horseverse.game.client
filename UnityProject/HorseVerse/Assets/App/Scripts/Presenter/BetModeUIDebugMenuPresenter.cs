#if ENABLE_DEBUG_MODULE
using System;
using UnityEngine;

public class BetModeUIDebugMenuPresenter : IDisposable
{
    private IDIContainer Container { get; }
    private UIDebugMenuPresenter uiDebugMenuPresenter;
    private IReadOnlyBetMatchRepository betMatchRepository;
    private UIDebugMenuPresenter UIDebugMenuPresenter => uiDebugMenuPresenter ??= Container.Inject<UIDebugMenuPresenter>();
    private IReadOnlyBetMatchRepository BetMatchRepository => betMatchRepository ??= Container.Inject<IReadOnlyBetMatchRepository>();

    public BetModeUIDebugMenuPresenter(IDIContainer container)
    {
        Container = container;
    }
    
    public void UpdateMatchId()
    {
        UIDebugMenuPresenter.AddDebugMenu($"BetMode/MatchId : {BetMatchRepository.Current.BetMatchId}", () =>
        {
            GUIUtility.systemCopyBuffer = BetMatchRepository.Current.BetMatchId.ToString();
        });
    }

    public void Dispose()
    {
        UIDebugMenuPresenter.RemoveDebugMenu($"BetMode/MatchId : {BetMatchRepository.Current.BetMatchId}");
    }
}
#endif