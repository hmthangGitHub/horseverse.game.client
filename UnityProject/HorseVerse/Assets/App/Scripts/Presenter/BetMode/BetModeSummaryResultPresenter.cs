using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

internal class BetModeSummaryResultPresenter : IDisposable
{
    private CancellationTokenSource cts;
    private UIBetModeResult uiBetModeResult;
    private UIUserBetSumary uiUserBetSummary;
    private HorseRaceContext horseRaceContext;
    private IReadOnlyBetMatchRepository betMatchRepository = default;

    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    private IReadOnlyBetMatchRepository BetMatchRepository => betMatchRepository ??= Container.Inject<IReadOnlyBetMatchRepository>();
    private IDIContainer Container { get;}
    private UserBetSummaryPresenter userBetSummaryPresenter;

    public BetModeSummaryResultPresenter(IDIContainer container)
    {
        this.Container = container;
    }

    public async UniTask ShowSummaryResultAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();

        uiBetModeResult ??= await UILoader.Instantiate<UIBetModeResult>();
        uiUserBetSummary ??= await UILoader.Instantiate<UIUserBetSumary>();

        var horseRaceWithOrdered = HorseRaceContext.RaceScriptData.HorseRaceInfos
                                                   .Select((horseRaceInfo, index) => (horseRaceInfo, lane: horseRaceInfo.RaceSegments.First().ToLane))
                                                   .OrderBy(x => x.horseRaceInfo.RaceSegments.Sum(segment => segment.Time) + x.horseRaceInfo.DelayTime)
                                                   .ToArray();
        await ShowBetModeResultAsync(horseRaceWithOrdered);
        await ShowBetModeMyResultAsync(horseRaceWithOrdered);

        await uiBetModeResult.Out();
    }

    private async UniTask ShowBetModeResultAsync((HorseRaceInfo horseRaceInfo, int lane)[] horseRaceWithOrdered)
    {
        var ucs = new UniTaskCompletionSource();

        
        uiBetModeResult.SetEntity(new UIBetModeResult.Entity()
        {
            betModeResultPanel = new UIBetModeResultPanel.Entity()
            {
                betModeResultList = horseRaceWithOrdered
                                    .Select((x, i) => new UIComponentBetModeResult.Entity()
                                    {
                                        horseName = x.horseRaceInfo.Name,
                                        time = x.horseRaceInfo.RaceSegments.Sum(segment => segment.Time) + x.horseRaceInfo.DelayTime,
                                        no = i + 1,
                                        horseNumber = x.lane - 1
                                    }).ToArray(),
            },
            nextBtn = new ButtonComponent.Entity(() =>
            {
                ucs.TrySetResult();
            })
        });
        await uiBetModeResult.In();
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    private async UniTask ShowBetModeMyResultAsync((HorseRaceInfo horseRaceInfo, int lane)[] horseRaceWithOrdered)
    {
        userBetSummaryPresenter ??= new UserBetSummaryPresenter(Container);
        await userBetSummaryPresenter.ShowUserBetSummaryAsync(BetMatchRepository.Current.BetMatchId,
            BetMatchRepository.Current.BetMatchTimeStamp, new UIComponentHorseInfoHistory.Entity()
            {
                horseIndex = horseRaceWithOrdered[0].lane - 1,
                horseName = horseRaceWithOrdered[0].horseRaceInfo.Name,
                horseRank = 1
            }, new UIComponentHorseInfoHistory.Entity()
            {
                horseIndex = horseRaceWithOrdered[1].lane - 1,
                horseName = horseRaceWithOrdered[1].horseRaceInfo.Name,
                horseRank = 2
            });
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        DisposeUtility.SafeDispose(ref userBetSummaryPresenter);
        UILoader.SafeRelease(ref uiBetModeResult);
        
        horseRaceContext = default;
    }
}