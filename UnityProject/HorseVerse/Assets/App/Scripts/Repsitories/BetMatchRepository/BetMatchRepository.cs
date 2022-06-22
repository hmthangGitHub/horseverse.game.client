#define MOCK_DATA
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BetMatchRepository : Repository<long, BetMatchModel, BetMatchModel>, IReadOnlyBetMatchRepository
{
    public BetMatchRepository() : base(x => x.BetMatchId, x => x, GetBetMatchModel)
    {
    }

    public BetMatchModel Current => Models.Values.First();

    private static async UniTask<IEnumerable<BetMatchModel>> GetBetMatchModel()
    {
#if MOCK_DATA
        await UniTask.CompletedTask;
        return new BetMatchModel[]
        {
            new BetMatchModel()
            {
                BetMatchId = UnityEngine.Random.Range(1, 100),
                BetMatchTimeStamp = (DateTimeOffset.Now.ToUniversalTime().ToUnixTimeSeconds() + 20),
            }
        }; 
#endif
    }
}

public interface IReadOnlyBetMatchRepository : IReadOnlyRepository<long, BetMatchModel>
{
    BetMatchModel Current { get; }
}
