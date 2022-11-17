#define MOCK_DATA
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorseRepository : Repository<long, HorseDataModel, HorseDataModel>, IReadOnlyHorseRepository, IHorseRepository
{
    private IDIContainer container;

    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();

    public HorseRepository(IDIContainer container) : base(x => x.MasterHorseId, x => x, () => GetHorseDatas(container))
    {
        this.container = container;
    }

    private static async UniTask<IEnumerable<HorseDataModel>> GetHorseDatas(IDIContainer container)
    {
#if MOCK_DATA
        //await UniTask.CompletedTask;
        //var masterHorseContainer = container.Inject<MasterHorseContainer>();
        //var horseCount = UnityEngine.Random.Range(1, masterHorseContainer.DataList.Length);
        //return masterHorseContainer.DataList
        //                    .Take(horseCount)
        //                    .Select(x => new HorseDataModel()
        //                    {
        //                        MasterHorseId = x.MasterHorseId,
        //                        Earning = UnityEngine.Random.Range(100, 10000),
        //                        PowerBonus = UnityEngine.Random.Range(0.0001f, 0.5f),
        //                        PowerRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
        //                        SpeedBonus = UnityEngine.Random.Range(0.0001f, 0.5f),
        //                        SpeedRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
        //                        TechnicallyBonus = UnityEngine.Random.Range(0.0001f, 0.5f),
        //                        TechnicallyRatio = UnityEngine.Random.Range(0.0001f, 0.5f)
        //                    });
#endif
        await UniTask.Delay(1000);
        ISocketClient socketClient = container.Inject<ISocketClient>();
        var response = await socketClient.Send<PlayerInventoryRequest, PlayerInventoryResponse>(new PlayerInventoryRequest(), 10.0f);
        if (response != null && response.PlayerInventory != null)
        {
            var horseInfos = response.PlayerInventory.HorseList;
            List<HorseDataModel> models = new List<HorseDataModel>();
            foreach(var horseInfo in horseInfos)
            {
                Debug.Log("Add Item " + horseInfo.NftId);
                var h = new HorseDataModel()
                {
                    MasterHorseId = horseInfo.NftId,
                    Earning = UnityEngine.Random.Range(100, 10000),
                    PowerBonus = UnityEngine.Random.Range(0.0001f, 0.5f),
                    PowerRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
                    SpeedBonus = UnityEngine.Random.Range(0.0001f, 0.5f),
                    SpeedRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
                    TechnicallyBonus = UnityEngine.Random.Range(0.0001f, 0.5f),
                    TechnicallyRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
                    Rarity = (int)horseInfo.Rarity,
                    Type = (int)horseInfo.HorseType,
                    Level = horseInfo.Level,
                    Color1 = getColorFromHexCode(horseInfo.Color1),
                    Color2 = getColorFromHexCode(horseInfo.Color2),
                    Color3 = getColorFromHexCode(horseInfo.Color3),
                    Color4 = getColorFromHexCode(horseInfo.Color4),
            };
                models.Add(h);
            }
            return models;
        }
        return default;
    }

    static Color getColorFromHexCode(string value)
    {
        var color = Color.white;
        ColorUtility.TryParseHtmlString(value, out color);
        return color;
    }

    public HorseDataModel Get(long id)
    {
        HorseDataModel horse = default;
        foreach(var item in Models)
        {
            Debug.Log("HH " + item.Value.MasterHorseId);
        }
        Models.TryGetValue(id, out horse);
        return horse;
    }
}

public interface IReadOnlyHorseRepository : IReadOnlyRepository<long, HorseDataModel> {
    public HorseDataModel Get(long id);
}
public interface IHorseRepository : IRepository<long, HorseDataModel, HorseDataModel> { 
    public HorseDataModel Get(long id); 
}
