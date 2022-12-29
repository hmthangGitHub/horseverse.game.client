using System;
using Cysharp.Threading.Tasks;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using io.hverse.game.protogen;

public interface IBetModeDomainService
{
    UniTask CancelBetAsync();
    UniTask BetAsync((int first, int second)[] keys, int amouth);
    UniTask<RaceMatchData> GetCurrentBetModeRaceMatchData();
    UniTask<HorseBetInfo> GetCurrentBetModeHorseData();
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
                    MatchId = BetMatchRepository.Current.BetMatchId,
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
        var bettingDetailResponse = await SocketClient.Send<GetBetHistoryDetailRequest, GetBetHistoryDetailResponse>(new GetBetHistoryDetailRequest()
        {
            MatchId = BetMatchRepository.Current.BetMatchId
        }, 5.0f);

        return new RaceMatchData()
        {
            HorseRaceInfos = QuickRaceDomainService.GetHorseRaceInfos(BetMatchRepository.Current.RaceScript, MasterHorseContainer),
            MasterMapId = QuickRaceState.MasterMapId,
            Mode = RaceMode.Bet,
            BetMatchId = BetMatchRepository.Current.BetMatchId,
            TotalBetWin = bettingDetailResponse.Records.Sum(item => item.WinMoney)
        };
    }

    public async UniTask<HorseBetInfo> GetCurrentBetModeHorseData()
    {

        var horseResponse = await SocketClient.Send<GetHorseListRequest, GetHorseListResponse>(new GetHorseListRequest()
        {
            MatchId = BetMatchRepository.Current.BetMatchId
        }, 5.0f);
        HorseDataModel[] data = default;
        data = horseResponse.HorseInfos.Select(x =>
        {
            return new HorseDataModel()
            {
                HorseNtfId = x.NftId,
                Name = x.Name,
                Earning = UnityEngine.Random.Range(100, 10000),
                PowerBonus = x.Bms,
                PowerRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
                SpeedBonus = x.Mms,
                SpeedRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
                TechnicallyBonus = x.Acceleration,
                TechnicallyRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
                Rarity = (int)x.Rarity,
                Type = (int)x.HorseType,
                Level = x.Level,
                Color1 = HorseRepository.GetColorFromHexCode(x.Color1),
                Color2 = HorseRepository.GetColorFromHexCode(x.Color2),
                Color3 = HorseRepository.GetColorFromHexCode(x.Color3),
                Color4 = HorseRepository.GetColorFromHexCode(x.Color4),
                LastBettingRecord = x.LastBettingRecord,
                AverageBettingRecord = x.AverageBettingRecord,
                BestBettingRecord = x.BestBettingRecord,
                Rate = x.WinRate
            };
        }).ToArray();
        return new HorseBetInfo()
        {
            horseInfos = data
        };
    }
         
    public async UniTask CancelBetAsync()
    {
        var response = await SocketClient.Send<CancelBettingRequest, CancelBettingResponse>(new CancelBettingRequest()
        {
            MatchId = BetMatchRepository.Current.BetMatchId,
            IsCancelAll = true
        });
        await UserDataRepository.UpdateCoin(response.TotalPresentChip);
        var resetCoinModels = BetRateRepository.Models.Values.Where(x => x.TotalBet != 0)
            .Select(x => x.Clone())
            .ToArray();
        resetCoinModels.ForEach(x => x.TotalBet = 0);
        await BetRateRepository.UpdateModelAsync(resetCoinModels);
    }

    public async UniTask RequestBetData()
    {
        var response = await SocketClient.Send<GetCurrentBetMatchRequest, GetCurrentBetMatchResponse>(
            new GetCurrentBetMatchRequest()
            {
            });

        await BetMatchRepository.UpdateModelAsync(new[]
        {
            new BetMatchModel()
            {
                MatchStatus = response.MatchStatus,
                BetMatchId = response.MatchId,
                BetMatchTimeStamp = response.TimeToStart,
                TimeToNextMatch = response.TimeToNextMatch,
                RaceScript = response.RaceScript
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

    public async UniTask<HorseBetInfo> GetCurrentBetModeHorseData()
    {
        return new HorseBetInfo();
    }
}