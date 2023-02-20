using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

internal class BetModeSummaryResultPresenter : IDisposable
{
    private CancellationTokenSource cts;
    private UIBetModeResult uiBetModeResult;
    private HorseRaceContext horseRaceContext;
    private IBetModeDomainService betModeDomainService = default;

    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    private IBetModeDomainService BetModeDomainService => betModeDomainService ??= Container.Inject<IBetModeDomainService>();
    private IDIContainer Container { get;}

    BetMatchFullDataContext betMatchFullDataContext = default;

    public BetModeSummaryResultPresenter(IDIContainer container)
    {
        this.Container = container;
    }

    public async UniTask ShowSummaryResultAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        
        this.betMatchFullDataContext = await BetModeDomainService.GetCurrentBetMatchRawData();

        uiBetModeResult ??= await UILoader.Instantiate<UIBetModeResult>();

        await ShowBetModeResultAsync();
        await ShowBetModeMyResultAsync();

        await uiBetModeResult.Out();
    }

    private async UniTask ShowBetModeResultAsync()
    {
        var ucs = new UniTaskCompletionSource();

        var horseRaceWithOrdered = HorseRaceContext.RaceScriptData.HorseRaceInfos
                                                .Select((horseRaceInfo, index) => (horseRaceInfo, lane: horseRaceInfo.RaceSegments.First().ToLane))
                                                .OrderBy(x => x.horseRaceInfo.RaceSegments.Sum(segment => segment.Time) + x.horseRaceInfo.DelayTime)
                                                .ToArray();
        uiBetModeResult.SetEntity(new UIBetModeResult.Entity()
        {
            betModeResultPanel = new UIBetModeResultPanel.Entity()
            {
                betModeResultList = new UIComponentBetModeResultList.Entity()
                {
                    entities = horseRaceWithOrdered
                    .Select((x, i) => new UIComponentBetModeResult.Entity()
                    {
                        horseName = x.horseRaceInfo.Name,
                        time = x.horseRaceInfo.RaceSegments.Sum(segment => segment.Time) + x.horseRaceInfo.DelayTime,
                        no = i + 1,
                        horseNumber = x.lane - 1
                    }).ToArray()
                },
            },
            betModeMyResultPanel = new UIBetModeMyResultPanel.Entity()
            {
                betModeMyResultList = new UIComponentBetModeMyResultList.Entity()
                {
                    entities = betMatchFullDataContext.Record
                    .Select(x => new UIComponentBetModeMyResult.Entity()
                    {
                        rate = x.rate,
                        isDoubleBet = x.doubleBet,
                        horseNumberFirst = x.pool_1, // Convert from server value to client value
                        horseNumberSecond = x.pool_2,
                        result = x.winMoney,
                        spend = x.betMoney,
                    }).ToArray()
                },
                horseNameFirst = horseRaceWithOrdered[0].horseRaceInfo.Name,
                horseNumberFirst = horseRaceWithOrdered[0].horseRaceInfo.RaceSegments[0].ToLane,
                horseNameSecond = horseRaceWithOrdered[1].horseRaceInfo.Name,
                horseNumberSecond = horseRaceWithOrdered[1].horseRaceInfo.RaceSegments[0].ToLane
            },
            nextBtn = new ButtonComponent.Entity(() =>
            {
                ucs.TrySetResult();
            })
        });
        await uiBetModeResult.In();
        await uiBetModeResult.showResultPanel();
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    public async UniTask ShowBetModeMyResultAsync()
    {
        var ucs = new UniTaskCompletionSource();
        if (uiBetModeResult != default)
        {
            uiBetModeResult.nextBtn.SetEntity(() =>
            {
                ucs.TrySetResult();
            });
        }
        await uiBetModeResult.showMyResultPanel();
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiBetModeResult);
        
        horseRaceContext = default;
    }
}