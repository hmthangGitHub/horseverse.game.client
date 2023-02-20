using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;

public class RacingMatchFindingPresenter : IDisposable
{
    private HorseRaceContext horseRaceContext;
    private readonly IDIContainer container;
    
    private const int MaxNumberOfPlayer = 8;
    private CancellationTokenSource cts;
    private IReadOnlyUserDataRepository userDataRepository;
    private IQuickRaceDomainService quickRaceDomainService;
    private UITouchDisablePresenter uiTouchDisablePresenter;
    private UIRacingFindMatch uiRacingFindMatch;
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= container.Inject<HorseRaceContext>();
    private IQuickRaceDomainService QuickRaceDomainService => quickRaceDomainService ??= container.Inject<IQuickRaceDomainService>();
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private UITouchDisablePresenter UITouchDisablePresenter => uiTouchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();
    
    public event Action<RaceScriptData> OnFoundMatch = ActionUtility.EmptyAction<RaceScriptData>.Instance;
    public event Action OnCancelFindMatch = ActionUtility.EmptyAction.Instance;
    
    public RacingMatchFindingPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask FindMatchAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();

        await UITouchDisablePresenter.ShowTillFinishTaskAsync(InstantiateUI());
        uiRacingFindMatch.SetEntity(new UIRacingFindMatch.Entity()
        {
            findMatchPopUpVisible = true,
            numberConnectPlayer = 1,
            cancelBtn = new ButtonComponent.Entity(() => OnCancelFindMatchAsync().Forget())
        });
        await uiRacingFindMatch.In();
        
        await FindMatchInternalAsync();
    }

    private void OnConnectedPlayerChange(int connectedPlayerChange)
    {
        uiRacingFindMatch.entity.numberConnectPlayer = connectedPlayerChange;
        uiRacingFindMatch.numberConnectPlayer.SetEntity(uiRacingFindMatch.entity.numberConnectPlayer);
        uiRacingFindMatch.cancelBtn.SetInteractable(connectedPlayerChange < MaxNumberOfPlayer);
    }


    private async UniTask FindMatchInternalAsync()
    {
        QuickRaceDomainService.OnConnectedPlayerChange += OnConnectedPlayerChange;
        StartFindMatchTimerAsync().Forget();
        
        var raceMatchData = await QuickRaceDomainService.FindMatch(UserDataRepository.Current.CurrentHorseNftId, GetRacingMode());
        
        StopFindMatchTimer();
        QuickRaceDomainService.OnConnectedPlayerChange -= OnConnectedPlayerChange;
        
        uiRacingFindMatch.ShowStartGamePopUp();

        await uiRacingFindMatch.In();
        await UITouchDisablePresenter.Delay(2);
        await uiRacingFindMatch.Out();
        
        OnFoundMatch.Invoke(raceMatchData);
    }

    private RacingMode GetRacingMode()
    {
        return HorseRaceContext.RaceMatchDataContext.RaceMode switch
        {
            RaceMode.Rank => RacingMode.Rank,
            RaceMode.Tournament => RacingMode.Tournament,
            RaceMode.Traditional => RacingMode.Traditional,
            RaceMode.StableVsStable => RacingMode.Stable,
            _ => RacingMode.Traditional
        };
    }

    private async UniTask InstantiateUI()
    {
        uiRacingFindMatch = await UILoader.Instantiate<UIRacingFindMatch>(UICanvas.UICanvasType.PopUp, token: cts.Token);
    }

    private async UniTaskVoid StartFindMatchTimerAsync()
    {
        var timer = 0;
        while (true)
        {
            uiRacingFindMatch.SetWaitingTime(timer);
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cts.Token);
            timer++;
        }
    }
    
    private async UniTaskVoid OnCancelFindMatchAsync()
    {
        await QuickRaceDomainService.CancelFindMatch(UserDataRepository.Current.CurrentHorseNftId);
        StopFindMatchTimer();
        await uiRacingFindMatch.Out();
        OnCancelFindMatch();
    }

    private void StopFindMatchTimer()
    {
        cts.SafeCancelAndDispose();
        cts = default;
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiRacingFindMatch);
        QuickRaceDomainService.OnConnectedPlayerChange -= OnConnectedPlayerChange;
    }
}