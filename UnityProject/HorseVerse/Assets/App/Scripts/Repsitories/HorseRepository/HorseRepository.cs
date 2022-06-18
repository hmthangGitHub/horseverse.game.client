#define MOCK_DATA
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorseRepository : Repository<long, HorseDataModel, HorseDataModel>, IReadOnlyHorseRepository, IHorseRepository
{
    private IDIContainer container;
    public HorseRepository(IDIContainer container) : base(x => x.MasterHorseId, x => x, () => GetHorseDatas(container))
    {
        this.container = container;
    }

    private static async UniTask<IEnumerable<HorseDataModel>> GetHorseDatas(IDIContainer container)
    {
#if MOCK_DATA
        await UniTask.CompletedTask;
        var masterHorseContainer = container.Inject<MasterHorseContainer>();
        var horseCount = UnityEngine.Random.Range(1, masterHorseContainer.DataList.Length);
        return masterHorseContainer.DataList
                            .Take(horseCount)
                            .Select(x => new HorseDataModel()
                            {
                                MasterHorseId = x.MasterHorseId,
                                Earning = UnityEngine.Random.Range(100, 10000),
                                PowerBonus = UnityEngine.Random.Range(0.0001f, 0.5f),
                                PowerRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
                                SpeedBonus = UnityEngine.Random.Range(0.0001f, 0.5f),
                                SpeedRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
                                TechnicallyBonus = UnityEngine.Random.Range(0.0001f, 0.5f),
                                TechnicallyRatio = UnityEngine.Random.Range(0.0001f, 0.5f)
                            });
#endif
    }
}

public interface IReadOnlyHorseRepository : IReadOnlyRepository<long, HorseDataModel> { }
public interface IHorseRepository : IRepository<long, HorseDataModel, HorseDataModel> { }
