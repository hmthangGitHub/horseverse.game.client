using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class BetModeRaceResultPresenter : IDisposable
{
    private IDIContainer Container { get; }
    private UIBetModeResult uiBetModeResult;
    private UIResultTimeAndPricePool uiPricePoolAndTime;
    private UIBetReward uiBetReward;
    private RaceMatchData raceMatchData;
    private RaceMatchData RaceMatchData => raceMatchData ??= Container.Inject<RaceMatchData>();
    private MasterHorseContainer masterHorseContainer;
    private CancellationTokenSource cts;

    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();

    public BetModeRaceResultPresenter(IDIContainer container)
    {
        this.Container = container;
    }

    public async UniTask ShowResultAsynnc()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await ShowBetModeResultAsync();
        await ShowRewardAsync();
    }

    private async UniTask ShowRewardAsync()
    {
        var ucs = new UniTaskCompletionSource();
        uiBetReward ??= await UILoader.Instantiate<UIBetReward>(token: cts.Token);
        uiBetReward.SetEntity(new UIBetReward.Entity()
        {
            reward = UnityEngine.Random.Range(100, 1000),
            outerBtn = new ButtonComponent.Entity(() => 
            {
                ucs.TrySetResult();
            })
        });
        uiBetReward.In().Forget();
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    private async UniTask ShowBetModeResultAsync()
    {
        var ucs = new UniTaskCompletionSource();
        uiBetModeResult ??= await UILoader.Instantiate<UIBetModeResult>();
        var tops = RaceMatchData.GetTops();
        uiBetModeResult.SetEntity(new UIBetModeResult.Entity()
        {
            betModeResultList = new UIComponentBetModeResultList.Entity()
            {
                entities = RaceMatchData.horseRaceTimes.Select((x, i) => new UIComponentBetModeResult.Entity()
                {
                    horseName = MasterHorseContainer.MasterHorseIndexer[x.masterHorseId].Name,
                    no = tops[i] + 1,
                }).ToArray()
            },
            nextBtn = new ButtonComponent.Entity(() =>
            {
                ucs.TrySetResult();
            })
        });
        await uiBetModeResult.In();
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        UILoader.SafeRelease(ref uiBetModeResult);
        UILoader.SafeRelease(ref uiPricePoolAndTime);
        UILoader.SafeRelease(ref uiBetReward);
    }
}