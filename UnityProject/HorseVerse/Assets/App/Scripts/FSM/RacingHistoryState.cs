using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RacingHistoryState : InjectedBState
{
    private RacingHistoryPresenter racingHistoryPresenter;
    private UIHeaderPresenter uiHeaderPresenter;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    
    public override void Enter()
    {
        base.Enter();
        OnEnterAsync().Forget();
    }

    private async UniTaskVoid OnEnterAsync()
    {
        UIHeaderPresenter.OnBack += OnBack;
        await UIHeaderPresenter.ShowHeaderAsync(title: "HISTORY");
#if MOCK_DATA
        Container.Bind(new LocalRacingHistoryRepository(Container.Inject<IReadOnlyHorseRepository>()));
#else
        Container.Bind(new RacingHistoryRepository(Container.Inject<ISocketClient>()));
#endif
        racingHistoryPresenter = new RacingHistoryPresenter(Container);
        racingHistoryPresenter.ShowHistoryAsync()
                              .Forget();
    }

    private void OnBack()
    {
        Machine.ChangeState<RaceModeChoosingState>();
    }

    public override void Exit()
    {
        base.Exit();
        UIHeaderPresenter.OnBack -= OnBack;
#if MOCK_DATA
        Container.RemoveAndDisposeIfNeed<LocalRacingHistoryRepository>();
#else
        Container.RemoveAndDisposeIfNeed<RacingHistoryRepository>();
#endif
        DisposeUtility.SafeDispose(ref racingHistoryPresenter);
    }
}
