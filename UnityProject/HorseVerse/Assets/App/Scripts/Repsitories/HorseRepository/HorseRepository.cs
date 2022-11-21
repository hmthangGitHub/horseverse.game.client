using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using UnityEngine;

public class HorseRepository : Repository<long, HorseDataModel, HorseDataModel>, IReadOnlyHorseRepository, IHorseRepository
{
    private IDIContainer container;
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();

    public HorseRepository(IDIContainer container) : base(x => x.HorseNtfId, x => x, () => GetHorseDatas(container))
    {
        this.container = container;
    }

    private static async UniTask<IEnumerable<HorseDataModel>> GetHorseDatas(IDIContainer container)
    {
        var socketClient = container.Inject<ISocketClient>();
        var response = await socketClient.Send<PlayerInventoryRequest, PlayerInventoryResponse>(new PlayerInventoryRequest());
        return response.PlayerInventory.HorseList.Select(horseInfo => new HorseDataModel()
            {
                HorseNtfId = horseInfo.NftId,
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
                Color1 = GetColorFromHexCode(horseInfo.Color1),
                Color2 = GetColorFromHexCode(horseInfo.Color2),
                Color3 = GetColorFromHexCode(horseInfo.Color3),
                Color4 = GetColorFromHexCode(horseInfo.Color4),
                
            })
            .ToList();
    }

    static Color GetColorFromHexCode(string value)
    {
        var color = Color.white;
        ColorUtility.TryParseHtmlString(value, out color);
        return color;
    }

    public HorseDataModel GetHorseDataModel(long ntfItemId)
    {
        return Models[ntfItemId];
    }
}

public interface IReadOnlyHorseRepository : IReadOnlyRepository<long, HorseDataModel> {
    public HorseDataModel GetHorseDataModel(long ntfItemId);
}
public interface IHorseRepository : IRepository<long, HorseDataModel, HorseDataModel> { 
    public HorseDataModel GetHorseDataModel(long ntfItemId); 
}
