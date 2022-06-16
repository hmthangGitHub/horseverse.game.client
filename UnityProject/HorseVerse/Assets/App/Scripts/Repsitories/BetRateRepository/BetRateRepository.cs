#define MOCK_DATA
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BetRateRepository : Repository<(int first, int second), BetRateModel, BetRateModel>, IBetRateRepository
{
    public BetRateRepository() : base(x => (x.First, x.Second), x => x, GetBetRateModel)
    {
    }

    public float TotalBetAmouth => Models.Sum(x => x.Value.TotalBet);

    private static async UniTask<IEnumerable<BetRateModel>> GetBetRateModel()
    {
#if MOCK_DATA
        await UniTask.CompletedTask;
        var horseNumber = 8;
        var betRateModels = new List<BetRateModel>();
        for (int i = 0; i < horseNumber; i++)
        {
            betRateModels.Add(new BetRateModel()
            {
                First = i + 1,
                Second = 0,
                Rate = UnityEngine.Random.Range(1.0f, 4.0f),
                TotalBet = UnityEngine.Random.Range(0, 2000)
            });
        }

        for (int i = 0; i < horseNumber; i++)
        {
            for (int j = 0; j < horseNumber; j++)
            {
                betRateModels.Add(new BetRateModel()
                {
                    First = i + 1,
                    Second = j + 1,
                    Rate = UnityEngine.Random.Range(1.0f, 4.0f),
                    TotalBet = UnityEngine.Random.Range(0, 2000)
                });
            }
        }
        return betRateModels; 
#endif
    }
}

public interface IReadOnlyBetRateRepository : IReadOnlyRepository<(int first, int second), BetRateModel>
{
    float TotalBetAmouth { get; }
}
public interface IBetRateRepository : IRepository<(int first, int second), BetRateModel, BetRateModel>, IReadOnlyBetRateRepository { }