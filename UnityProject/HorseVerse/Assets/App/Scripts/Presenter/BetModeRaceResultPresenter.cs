using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class BetModeRaceResultPresenter : IDisposable
{
    private CancellationTokenSource cts;
    private IDIContainer Container { get; }
    
    private UIBetModeResult uiBetModeResult;
    private UIResultTimeAndPricePool uiPricePoolAndTime;
    private UIBetReward uiBetReward;
    
    private HorseRaceContext horseRaceContext;
    private UserDataRepository userDataRepository;
    private BetModeSummaryResultPresenter betModeSummaryResultPresenter;

    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    private UserDataRepository UserDataRepository => userDataRepository ??= Container.Inject<UserDataRepository>();

    public BetModeRaceResultPresenter(IDIContainer container)
    {
        this.Container = container;
        betModeSummaryResultPresenter = new BetModeSummaryResultPresenter(Container);
    }

    public async UniTask ShowResultAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await betModeSummaryResultPresenter.ShowSummaryResultAsync();
        await ShowRewardAsync();
        await UserDataRepository.UpdateCoin(UserDataRepository.Current.Coin + HorseRaceContext.BetMatchDataContext.TotalBetWin);
    }

    private async UniTask ShowRewardAsync()
    {
        var ucs = new UniTaskCompletionSource();

        uiBetReward ??= await UILoader.Instantiate<UIBetReward>(token: cts.Token);
        uiBetReward.SetEntity(new UIBetReward.Entity()
        {
            reward = HorseRaceContext.BetMatchDataContext.TotalBetWin,
            outerBtn = new ButtonComponent.Entity(() => 
            {
                ucs.TrySetResult();
            })
        });
        uiBetReward.In().Forget();
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        
        horseRaceContext = default;
        
        UILoader.SafeRelease(ref uiBetModeResult);
        UILoader.SafeRelease(ref uiPricePoolAndTime);
        UILoader.SafeRelease(ref uiBetReward);
        DisposeUtility.SafeDispose(ref betModeSummaryResultPresenter);
    }
}