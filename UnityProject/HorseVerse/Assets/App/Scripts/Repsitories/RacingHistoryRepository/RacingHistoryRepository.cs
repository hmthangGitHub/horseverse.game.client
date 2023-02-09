using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RacingHistoryRepository : Repository<long , RacingHistory, RacingHistory>, IReadOnlyRacingHistoryRepository
{

    public RacingHistoryRepository(ISocketClient socketClient) : base(x => x.MatchId, x => x, () => GetData(socketClient))
    {
    }

    private static UniTask<IEnumerable<RacingHistory>> GetData(ISocketClient socketClient)
    {
        throw new NotImplementedException();
    }
}

public class LocalRacingHistoryRepository : Repository<long , RacingHistory, RacingHistory>, IReadOnlyRacingHistoryRepository
{

    public LocalRacingHistoryRepository(IReadOnlyHorseRepository horseRepository) : base(x => x.MatchId, x => x, () => GetData(horseRepository))
    {
    }

    private  static async UniTask<IEnumerable<RacingHistory>> GetData(IReadOnlyHorseRepository readOnlyHorseRepository)
    {
        await readOnlyHorseRepository.LoadRepositoryIfNeedAsync();
        return Enumerable.Range(0, UnityEngine.Random.Range(1, 10))
                         .Select((x, i) => new RacingHistory()
                         {
                             MatchId = i * 100 + UnityEngine.Random.Range(1, 100),
                             HorseIndex = UnityEngine.Random.Range(1, 8),
                             Rank = UnityEngine.Random.Range(1, 8),
                             TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + UnityEngine.Random.Range(-10000, 10000),
                             NftHorseId = readOnlyHorseRepository.Models.Keys.RandomElement(),
                             ChestRewardNumber = UnityEngine.Random.Range(0, 10000),
                             CoinRewardNumber = UnityEngine.Random.Range(0, 10000),
                         });
    }
}

public interface IReadOnlyRacingHistoryRepository : IReadOnlyRepository<long, RacingHistory>
{
}
