using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;

public class RaceHistoryResultDetailPresenter : IDisposable
{
    private readonly IDIContainer container;
    private ISocketClient socketClient;
    private UITouchDisablePresenter uiTouchDisablePresenter;
    private IReadOnlyHorseRepository horseRepository;
    private CancellationTokenSource cts;
    private UIRaceModeResult uiRaceModeResult;
    private UIHeaderPresenter uiHeaderPresenter;

    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();
    private UITouchDisablePresenter UITouchDisablePresenter => uiTouchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= container.Inject<UIHeaderPresenter>();
    
    public RaceHistoryResultDetailPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ShowResultDetail(long matchId)
    {
        var ucs = new UniTaskCompletionSource();
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();

        UIHeaderPresenter.HideHeader();
        await UITouchDisablePresenter.ShowTillFinishTaskAsync(InstantiateUI());
        
        var response = await SocketClient.Send<GetRaceDetailRequest, GetRaceDetailResponse>(new GetRaceDetailRequest()
        {
            RoomId = matchId
        });
        
        uiRaceModeResult.SetEntity(new UIRaceModeResult.Entity()
        {
            betModeResultList = new UIComponentBetModeResultList.Entity()
            {
                entities = response.Records
                                   .OrderBy(x => x.RaceTime)
                                   .Select((x, i) =>
                                             {
                                                 var rank = i + 1;
                                                 return new UIComponentBetModeResult.Entity()
                                                 {
                                                     horseName = x.HorseName,
                                                     time = x.RaceTime,
                                                     no = rank,
                                                     horseNumber = x.Lane - 1,
                                                     rewardGroupVisible = true,
                                                     rewardGroup = new UIComponentRaceRewardGroup.Entity()
                                                     {
                                                         chestNumber = (int)(x.Rewards.FirstOrDefault(rewardInfo => rewardInfo.Type == RewardType.Chest)?.Amount ?? 0),
                                                         coinNumber = (int)(x.Rewards.FirstOrDefault(rewardInfo => rewardInfo.Type == RewardType.Chip)?.Amount ?? 0)
                                                     },
                                                     isSelfHorse = HorseRepository.Models.ContainsKey(x.HorseId)
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
        await UIHeaderPresenter.ShowHeaderAsync();
    }

    private async UniTask<UIRaceModeResult> InstantiateUI()
    {
        return uiRaceModeResult ??= await UILoader.Instantiate<UIRaceModeResult>();
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiRaceModeResult);
        socketClient = default;
        uiTouchDisablePresenter = default;
    }
}
