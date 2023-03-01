using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using UnityEngine;

public class BetHistoryRepository : Repository<long , BettingMatch, BetRecord>, IReadOnlyBetHistoryRepository
{
    public BetHistoryRepository(ISocketClient socketClient) : base(x => x.MatchId, x => new BetRecord()
    {
        MatchId = x.MatchId,
        TimeStamp = x.TimeToStart,
        FirstHorseIndex = x.FirstRankHorseLane,
        FirstHorseName = x.FirstRankHorseName,
        SecondHorseIndex = x.SecondRankHorseLane,
        SecondHorseName = x.SecondRankHorseName,
    }, () => GetData(socketClient))
    {
    }

    private static async UniTask<IEnumerable<BettingMatch>> GetData(ISocketClient socketClient)
    {
        var response = await socketClient.Send<BetHistoryRequest, BetHistoryResponse>(new BetHistoryRequest());
        return response.Records;
    }
}

public class LocalBetHistoryRepository : Repository<long , BetRecord, BetRecord>, IReadOnlyBetHistoryRepository
{
    public LocalBetHistoryRepository() : base(x => x.MatchId, 
        x => x,
        GetData)
    {
    }

    private  static async UniTask<IEnumerable<BetRecord>> GetData()
    {
        return Enumerable.Range(0, UnityEngine.Random.Range(1, 10))
                         .Select((x, i) => new BetRecord()
                         {
                             MatchId = i * 100 + UnityEngine.Random.Range(1, 100),
                             FirstHorseIndex = UnityEngine.Random.Range(1, 8),
                             FirstHorseName = "Name" + UnityEngine.Random.Range(1, 8),
                             TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + UnityEngine.Random.Range(-10000, 10000),
                             SecondHorseIndex = UnityEngine.Random.Range(1, 8),
                             SecondHorseName = "Name" + UnityEngine.Random.Range(1, 8)
                         });
    }
}

public interface IReadOnlyBetHistoryRepository : IReadOnlyRepository<long, BetRecord>
{
}
