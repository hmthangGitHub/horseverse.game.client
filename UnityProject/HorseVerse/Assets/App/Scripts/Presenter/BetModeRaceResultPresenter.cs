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
    
    private RaceScriptData raceScriptData;
    private ISocketClient socketClient;
    private MasterHorseContainer masterHorseContainer;
    private UserDataRepository userDataRepository;
    
    private RaceScriptData RaceScriptData => raceScriptData ??= Container.Inject<RaceScriptData>();
    private UserDataRepository UserDataRepository => userDataRepository ??= Container.Inject<UserDataRepository>();

    public BetModeRaceResultPresenter(IDIContainer container)
    {
        this.Container = container;
    }

    public async UniTask ShowResultAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await ShowBetModeResultAsync();
        await ShowRewardAsync();
        if (RaceScriptData.TotalBetWin >= 0)
        {
            await UserDataRepository.UpdateCoin(UserDataRepository.Current.Coin + RaceScriptData.TotalBetWin);
        }
    }

    private async UniTask ShowRewardAsync()
    {
        var ucs = new UniTaskCompletionSource();

        uiBetReward ??= await UILoader.Instantiate<UIBetReward>(token: cts.Token);
        uiBetReward.SetEntity(new UIBetReward.Entity()
        {
            reward = RaceScriptData.TotalBetWin,
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
        var tops = RaceScriptData.HorseRaceInfos.Select(x => x.RaceSegments.Sum(segment => segment.Time)).ToArray().GetTopByTimes();
        
        uiBetModeResult.SetEntity(new UIBetModeResult.Entity()
        {
            betModeResultList = new UIComponentBetModeResultList.Entity()
            {
                entities = RaceScriptData.HorseRaceInfos
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
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        
        raceScriptData = default;
        socketClient = default;
        masterHorseContainer = default;
        
        UILoader.SafeRelease(ref uiBetModeResult);
        UILoader.SafeRelease(ref uiPricePoolAndTime);
        UILoader.SafeRelease(ref uiBetReward);
    }
}