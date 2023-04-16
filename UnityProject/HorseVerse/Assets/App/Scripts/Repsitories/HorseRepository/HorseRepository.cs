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
    private readonly IDIContainer container;
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
        var nftIds = new RepeatedField<long>();
        nftIds.AddRange(response.PlayerInventory.HorseList.Select(x => x.NftId));
        var horseBasicResponse = await socketClient.Send<PlayerHorseBasicRequest, PlayerHorseBasicResponse>(new PlayerHorseBasicRequest()
        {
            Id = { nftIds  }
        });
        
        var horseAttributeResponse = await socketClient.Send<PlayerHorseAttributeRequest, PlayerHorseAttributeResponse>(new PlayerHorseAttributeRequest()
        {
            Id = { nftIds  }
        });
        
        var horseRisingResponse = await socketClient.Send<PlayerHorseRisingRequest, PlayerHorseRisingResponse>(new PlayerHorseRisingRequest()
        {
            Id = {nftIds}
        });
        
        var horseHistoryResponse = await socketClient.Send<PlayerHorseHistoryRequest, PlayerHorseHistoryResponse>(new PlayerHorseHistoryRequest()
        {
            Id = {nftIds}
        });
        
        return response.PlayerInventory.HorseList.Select(horseInfo => new HorseDataModel()
       {
           HorseNtfId = horseInfo.NftId,
           Name = horseInfo.Name,
           Happiness = horseInfo.Happiness,
           Earning = UnityEngine.Random.Range(100, 10000),
           PowerBonus = UnityEngine.Random.Range(0.0001f, 0.5f),
           PowerRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
           SpeedBonus = UnityEngine.Random.Range(0.0001f, 0.5f),
           SpeedRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
           TechnicallyBonus = UnityEngine.Random.Range(0.0001f, 0.5f),
           TechnicallyRatio = UnityEngine.Random.Range(0.0001f, 0.5f),
           Rarity = (HorseRarity)horseInfo.Rarity,
           Type = horseInfo.HorseType,
           Level = horseInfo.Level,
           Color1 = GetColorFromHexCode(horseInfo.Color1),
           Color2 = GetColorFromHexCode(horseInfo.Color2),
           Color3 = GetColorFromHexCode(horseInfo.Color3),
           Color4 = GetColorFromHexCode(horseInfo.Color4),
           HorseBasic = horseBasicResponse.HorseBasicList.First(x => x.Id == horseInfo.NftId),
           HorseAttribute = horseAttributeResponse.HorseAttributeList.First(x => x.Id == horseInfo.NftId),
           HorseRising = horseRisingResponse.HorseRisingList.First(x => x.Id == horseInfo.NftId),
           HorseHistory = horseHistoryResponse.HorseHistoryList.First(x => x.Id == horseInfo.NftId),
       })
       .ToList();
    }

    public static Color GetColorFromHexCode(string value)
    {
        var color = Color.white;
        ColorUtility.TryParseHtmlString(value, out color);
        return color;
    }

    public HorseDataModel GetHorseDataModel(long ntfItemId)
    {
        return Models[ntfItemId];
    }

    public UniTask UpdateModelAsync(HorseBasic horseBasic)
    {
        var newModel = Models[horseBasic.Id].Clone();
        newModel.HorseBasic = horseBasic;
        return UpdateDataAsync(new []{newModel});
    }

    public UniTask UpdateModelAsync(HorseAttribute horseAttribute)
    {
        var newModel = Models[horseAttribute.Id].Clone();
        newModel.HorseAttribute = horseAttribute;
        return UpdateDataAsync(new []{newModel});
    }

    public UniTask UpdateModelAsync(HorseHistory horseHistory)
    {
        var newModel = Models[horseHistory.Id].Clone();
        newModel.HorseHistory = horseHistory;
        return UpdateDataAsync(new []{newModel});
    }

    public UniTask UpdateModelAsync(HorseRising horseRising)
    {
        var newModel = Models[horseRising.Id].Clone();
        newModel.HorseRising = horseRising;
        return UpdateDataAsync(new []{newModel});
    }
}

public interface IReadOnlyHorseRepository : IReadOnlyRepository<long, HorseDataModel> {
    public HorseDataModel GetHorseDataModel(long ntfItemId);
}

public interface IHorseRepository : IRepository<long, HorseDataModel, HorseDataModel>
{
    UniTask UpdateModelAsync(HorseBasic horseBasic);
    UniTask UpdateModelAsync(HorseAttribute horseAttribute);
    UniTask UpdateModelAsync(HorseHistory horseHistory);
    UniTask UpdateModelAsync(HorseRising horseRising);
}
