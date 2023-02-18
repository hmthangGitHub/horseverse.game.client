using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using UnityEngine;

public class RacingHistoryRepository : Repository<long , RacingHistory, RacingHistory>, IReadOnlyRacingHistoryRepository
{
    public RacingHistoryRepository(ISocketClient socketClient) : base(x => x.MatchId, x => x, () => GetData(socketClient))
    {
    }

    private static async UniTask<IEnumerable<RacingHistory>> GetData(ISocketClient socketClient)
    {
        var historyResponse = await socketClient.Send<GetRaceHistoryRequest, GetRaceHistoryResponse>(new GetRaceHistoryRequest());
        return historyResponse.Records.Select(x => new RacingHistory()
        {
            Rank = x.Rank,
            HorseIndex = x.Lane,
            MatchId = x.RoomId,
            TimeStamp = x.TimeStartRace / 1000,
            ChestRewardNumber = (int)(x.Rewards.FirstOrDefault(reward => reward.Type == RewardType.Chest)?.Amount ?? 0),
            CoinRewardNumber = (int)(x.Rewards.FirstOrDefault(reward => reward.Type == RewardType.Chip)?.Amount ?? 0),
            NftHorseId = x.HorseInfo.NftId,
        });
    }
}

public class LocalRacingHistoryRepository : Repository<long , RacingHistory, RacingHistory>, IReadOnlyRacingHistoryRepository
{
    public LocalRacingHistoryRepository(IReadOnlyHorseRepository horseRepository) : base(x => x.MatchId, 
        x => x,
        () => GetData(horseRepository))
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
