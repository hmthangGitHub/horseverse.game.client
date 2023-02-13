using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

internal class RaceSummaryResultPresenter : IDisposable
{
    private CancellationTokenSource cts;
    private UIBetModeResult uiBetModeResult;
    private HorseRaceContext horseRaceContext;
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    private IDIContainer Container { get;}
    
    public RaceSummaryResultPresenter(IDIContainer container)
    {
        this.Container = container;
    }

    public async UniTask ShowSummaryResultAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        
        var ucs = new UniTaskCompletionSource();
        uiBetModeResult ??= await UILoader.Instantiate<UIBetModeResult>();

        uiBetModeResult.SetEntity(new UIBetModeResult.Entity()
        {
            betModeResultList = new UIComponentBetModeResultList.Entity()
            {
                entities = HorseRaceContext.RaceScriptData.HorseRaceInfos
                                             .Select((horseRaceInfo, index) => (horseRaceInfo , index))
                                             .OrderBy(x => x.horseRaceInfo.RaceSegments.Sum(segment => segment.Time) + x.horseRaceInfo.DelayTime)
                                             .Select((x, i) => new UIComponentBetModeResult.Entity()
                                             {
                                                 horseName = x.horseRaceInfo.Name,
                                                 time = x.horseRaceInfo.RaceSegments.Sum(segment => segment.Time) + x.horseRaceInfo.DelayTime,
                                                 no = i + 1,
                                                 horseNumber = x.index
                                             }).ToArray()
            },
            nextBtn = new ButtonComponent.Entity(() =>
            {
                ucs.TrySetResult();
            })
        });
        await uiBetModeResult.In();
        await ucs.Task.AttachExternalCancellation(cts.Token);
        await uiBetModeResult.Out();
    }
    
    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiBetModeResult);
        
        horseRaceContext = default;
    }
}