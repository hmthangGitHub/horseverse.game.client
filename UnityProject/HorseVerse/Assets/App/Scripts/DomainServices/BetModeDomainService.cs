using System;
using Cysharp.Threading.Tasks;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using io.hverse.game.protogen;

public interface IBetModeDomainService
{
    UniTask CancelBetAsync();
    UniTask BetAsync((int first, int second)[] keys, int amouth);
    UniTask<RaceMatchData> GetCurrentBetModeRaceMatchData();
    UniTask RequestBetData();
}

public class BetModeDomainService : BetModeDomainServiceBase, IBetModeDomainService, IDisposable
{
    private IUserDataRepository userDataRepository;
    private MasterHorseContainer masterHorseContainer;
    private ISocketClient socketClient;
    private IBetMatchRepository betMatchRepository;

    private IUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<UserDataRepository>();
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();
    private IBetMatchRepository BetMatchRepository => betMatchRepository ??= container.Inject<IBetMatchRepository>();

    private MasterHorseContainer MasterHorseContainer =>
        masterHorseContainer ??= container.Inject<MasterHorseContainer>();

    public BetModeDomainService(IDIContainer container) : base(container)
    {
    }

    public async UniTask BetAsync((int first, int second)[] keys, int amouth)
    {
        foreach (var bet in keys)
        {
            var response = await SocketClient.Send<SendBettingInfoRequest, SendBettingInfoResponse>(
                new SendBettingInfoRequest()
                {
                    BettingType = bet.second == default ? BettingType.Single : BettingType.Double,
                    MatchId = (int)BetMatchRepository.Current.BetMatchId,
                    AmountOfBet = amouth,
                    IndexOfHouse = bet.ToIndexOfHorse()
                });
            var model = BetRateRepository.Models[response.IndexOfHouse.ToKey()].Clone();
            model.TotalBet = response.TotalAmountOfBet;
            await BetRateRepository.UpdateModelAsync(new[] { model });
            await UserDataRepository.UpdateCoin(response.PlayerPresentChip);
        }
    }

    public async UniTask<RaceMatchData> GetCurrentBetModeRaceMatchData()
    {
        var matchRequest = await SocketClient.Send<GetTheMatchRequest, GetTheMatchResponse>(new GetTheMatchRequest()
        {
            MatchId = BetMatchRepository.Current.BetMatchId
        });

        var bettingDetailResponse = await SocketClient.Send<GetBettingMatchRequest, GetBettingMatchResponse>(new GetBettingMatchRequest()
        {
        });

        var totalWin = bettingDetailResponse.Records.First(x => x.MatchId == BetMatchRepository.Current.BetMatchId);
        
        return new RaceMatchData()
        {
            HorseRaceTimes = QuickRaceDomainService.GetHorseRaceTimes(matchRequest.RaceScript, MasterHorseContainer),
            MasterMapId = 10001002,
            Mode = RaceMode.BetMode,
            BetMatchId = BetMatchRepository.Current.BetMatchId,
            TotalBetWin = totalWin
        };
    }

    public async UniTask CancelBetAsync()
    {
        var response = await SocketClient.Send<CancelBettingRequest, CancelBettingResponse>(new CancelBettingRequest()
        {
            MatchId = BetMatchRepository.Current.BetMatchId,
            IsCancelAll = true
        });

        if (response.Result == ResultCode.Success)
        {
            await UserDataRepository.UpdateCoin(response.TotalPresentChip);
            var resetCoinModels = BetRateRepository.Models.Values.Where(x => x.TotalBet != 0)
                .Select(x => x.Clone())
                .ToArray();
            resetCoinModels.ForEach(x => x.TotalBet = 0);
            await BetRateRepository.UpdateModelAsync(resetCoinModels);
        }
        else
        {
            throw new Exception("Cancel bet failed");
        }
    }

    public async UniTask RequestBetData()
    {
        var response = await SocketClient.Send<GetInfoBettingRequest, GetInfoBettingResponse>(
            new GetInfoBettingRequest()
            {
            });

        await BetMatchRepository.UpdateModelAsync(new[]
        {
            new BetMatchModel()
            {
                MatchStatus = response.MatchStatus,
                BetMatchId = response.MatchId,
                BetMatchTimeStamp = response.TimeToStart,
                TimeToNextMatch = response.TimeToNextMatch
            }
        });

        await BetRateRepository.UpdateDataAsync(response.WinRateList.ToArray());
    }

    public void Dispose()
    {
        userDataRepository = default;
        masterHorseContainer = default;
        socketClient = default;
        betMatchRepository = default;
    }
}

public class LocalBetModeDomainService : BetModeDomainServiceBase, IBetModeDomainService
{
    public LocalBetModeDomainService(IDIContainer container) : base(container)
    {
    }

    public async UniTask BetAsync((int first, int second)[] keys, int amouth)
    {
        await UniTask.Delay(500);
        var betRates = keys.Select(key => BetRateRepository.Models[key])
            .ToList();
        betRates.ForEach(betRate => betRate.TotalBet += amouth);
        await BetRateRepository.UpdateModelAsync(betRates);
    }

    public UniTask RequestBetData()
    {
        throw new System.NotImplementedException();
    }

    public async UniTask CancelBetAsync()
    {
        await UniTask.Delay(500);
        var models = BetRateRepository.Models.Values.Select(x => new BetRateModel()
        {
            First = x.First,
            Second = x.Second,
            Rate = x.Rate,
            TotalBet = 0
        });
        await BetRateRepository.UpdateModelAsync(models);
    }


    public async UniTask<RaceMatchData> GetCurrentBetModeRaceMatchData()
    {
        return await new LocalQuickRaceDomainService(container).FindMatch();
    }
}