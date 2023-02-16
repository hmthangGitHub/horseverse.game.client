using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

internal class RacingModeSummaryResultPresenter : IDisposable
{
    private CancellationTokenSource cts;
    private UIRaceModeResult uiRaceModeResult;
    private HorseRaceContext horseRaceContext;
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    private IReadOnlyHorseRepository horseRepository;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= Container.Inject<IReadOnlyHorseRepository>();
    private IDIContainer Container { get;}
    
    public RacingModeSummaryResultPresenter(IDIContainer container)
    {
        this.Container = container;
    }

    public async UniTask ShowSummaryResultAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        
        var ucs = new UniTaskCompletionSource();
        uiRaceModeResult ??= await UILoader.Instantiate<UIRaceModeResult>();

        uiRaceModeResult.SetEntity(new UIRaceModeResult.Entity()
        {
            betModeResultList = new UIComponentBetModeResultList.Entity()
            {
                entities = HorseRaceContext.RaceScriptData.HorseRaceInfos
                                             .Select((horseRaceInfo, index) => (horseRaceInfo , index))
                                             .OrderBy(x => x.horseRaceInfo.RaceSegments.Sum(segment => segment.Time) + x.horseRaceInfo.DelayTime)
                                             .Select((x, i) =>
                                             {
                                                 var rank = i + 1;
                                                 return new UIComponentBetModeResult.Entity()
                                                 {
                                                     horseName = x.horseRaceInfo.Name,
                                                     time = x.horseRaceInfo.RaceSegments.Sum(segment => segment.Time) + x.horseRaceInfo.DelayTime,
                                                     no = rank,
                                                     horseNumber = x.index,
                                                     rewardGroupVisible = HorseRaceContext.GameMode == HorseGameMode.Race,
                                                     rewardGroup = UIComponentRaceRewardGroupFactory.CreateRewardGroup(rank, HorseRaceContext.RaceMatchDataContext.RacingRoomType),
                                                     isSelfHorse = HorseRepository.Models.ContainsKey(x.horseRaceInfo.NftHorseId)
                                                 };
                                             }).ToArray()
            },
            rewardTitle = true,
            nextBtn = new ButtonComponent.Entity(() =>
            {
                ucs.TrySetResult();
            })
        });
        await uiRaceModeResult.In();
        await ucs.Task.AttachExternalCancellation(cts.Token);
        await uiRaceModeResult.Out();
    }
    
    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiRaceModeResult);
        
        horseRaceContext = default;
    }
}