using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;

public class RacingHistoryPresenter : IDisposable
{
    private readonly IDIContainer container;
    private UIRacingHistory uiRacingHistory;
    private CancellationTokenSource cts;
    private RaceHistoryResultDetailPresenter raceHistoryResultDetailPresenter;
    private HorseRaceInfoFactory horseRaceInfoFactory;
    private ISocketClient socketClient;
    private IReadOnlyHorseRepository horseRepository;
    
    private IReadOnlyRacingHistoryRepository racingHistoryRepository;
    private IReadOnlyRacingHistoryRepository RacingHistoryRepository => racingHistoryRepository ??= container.Inject<IReadOnlyRacingHistoryRepository>();
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private HorseRaceInfoFactory HorseRaceInfoFactory => horseRaceInfoFactory ??= container.Inject<HorseRaceInfoFactory>();
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();
    public Action<RaceMatchData> OnReplay = ActionUtility.EmptyAction<RaceMatchData>.Instance; 
    
    public RacingHistoryPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTaskVoid ShowHistoryAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await RacingHistoryRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);
        uiRacingHistory ??= await UILoader.Instantiate<UIRacingHistory>(token: this.cts.Token);
        uiRacingHistory.SetEntity(new UIRacingHistory.Entity()
        {
            historyContainer = new UIComponentHistoryRecordList.Entity()
            {
                entities = RacingHistoryRepository.Models.Values.Select(x => new UIComponentHistoryRecord.Entity()
                {
                    time = x.TimeStamp,
                    chestNumber = x.ChestRewardNumber,
                    coinNumber = x.CoinRewardNumber,
                    horseIndex = x.HorseIndex - 1,
                    horseRank = new UIComponentHistoryRecordHorseRank.Entity()
                    {
                        rank = x.Rank
                    },
                    matchId = x.MatchId,
                    viewResultBtn = new ButtonComponent.Entity(() =>
                    {
                        raceHistoryResultDetailPresenter ??= new RaceHistoryResultDetailPresenter(container);
                        raceHistoryResultDetailPresenter.ShowResultDetail(x.MatchId).Forget();
                    }),
                    viewRaceScriptBtn = new ButtonComponent.Entity(UniTask.Action(async () =>
                    {
                        var response = await SocketClient.Send<GetRaceReplayRequest, GetRaceReplayResponse>(new GetRaceReplayRequest()
                        {
                            RoomId = x.MatchId
                        });
                        
                        await uiRacingHistory.Out();
                        
                        OnReplay.Invoke(new RaceMatchData()
                        {
                            HorseRaceInfos = HorseRaceInfoFactory.GetHorseRaceInfos(response.Script),
                        });
                    })),
                    horseName = HorseRepository.Models[x.NftHorseId].Name
                }).ToArray()
            }
        });

        uiRacingHistory.In().Forget();
    }
    
    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        DisposeUtility.SafeDispose(ref raceHistoryResultDetailPresenter);
        UILoader.SafeRelease(ref uiRacingHistory);
    }
}
