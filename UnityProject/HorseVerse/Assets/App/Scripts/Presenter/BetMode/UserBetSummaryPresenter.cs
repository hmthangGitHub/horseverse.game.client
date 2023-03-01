using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

public class UserBetSummaryPresenter : IDisposable
{
    private readonly IDIContainer container;
    private UIUserBetSumary uiUserBetSummary;
    private IBetModeDomainService betModeDomainService;
    private IBetModeDomainService BetModeDomainService => betModeDomainService ??= container.Inject<BetModeDomainService>();

    private CancellationTokenSource cts;

    public UserBetSummaryPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ShowUserBetSummaryAsync(long matchId,
                                                   long time,
                                                   UIComponentHorseInfoHistory.Entity firstHorse,
                                                   UIComponentHorseInfoHistory.Entity secondHorse)
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        var ucs = new UniTaskCompletionSource();
        var userBets = await BetModeDomainService.GetCurrentBetMatchRawData(matchId).AttachExternalCancellation(cancellationToken: cts.Token);
        uiUserBetSummary ??= await UILoader.Instantiate<UIUserBetSumary>(token: cts.Token);
        uiUserBetSummary.SetEntity(new UIUserBetSumary.Entity()
        {
            time = time,
            matchId = matchId,
            firstHorse = firstHorse,
            secondHorse = secondHorse,
            userBetRecordList = userBets.Record.Select(x => new UIComponentBetModeMyResult.Entity()
            {
                rate = x.rate,
                result = x.winMoney,
                spend = x.betMoney,
                isDoubleBet = x.doubleBet,
                horseNumberFirst = x.pool_1,
                horseNumberSecond = x.pool_2,
            }).ToArray(),
            closeBtn = new ButtonComponent.Entity(UniTask.Action(async() =>
            {
                await uiUserBetSummary.Out();
                ucs.TrySetResult();
            }))
        });
        
        await uiUserBetSummary.In().AttachExternalCancellation(cts.Token);
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiUserBetSummary);
        betModeDomainService = default;
    }
}