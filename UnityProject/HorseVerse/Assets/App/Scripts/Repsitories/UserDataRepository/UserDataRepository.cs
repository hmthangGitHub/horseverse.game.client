using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using io.hverse.game.protogen;
using UnityEngine;

public class UserDataRepository : Repository<long, PlayerInfo, UserDataModel>, IReadOnlyUserDataRepository, IUserDataRepository
{
    public UserDataRepository() : base(x => x.UserId, FromPlayerInfoToUserDataModel, GetUserDataModels)
    {
    }

    private static UserDataModel FromPlayerInfoToUserDataModel(PlayerInfo x)
    {
        return new UserDataModel()
        {
            Coin = x.Chip,
            Energy = x.Energy,
            CurrentHorseNftId = x.CurrentHorse.NftId,
            UserId = x.Id,
            UserName = x.Name,
            Exp = 0,
            Level = 1,
            NextLevelExp = 1,
            TraningTimeStamp = 0,
            DailyRacingNumberLeft = x.FreeRacingNumber
        };
    }

    public UserDataModel Current { get { if (Models.Count == 0) return default; return Models.First().Value; } }
    
    public async UniTask UpdateCoin(long coin)
    {
        var newModel = Current.Clone();
        newModel.Coin = coin;
        await UpdateModelAsync(new[] { newModel });
    }

    public async UniTask UpdateHorse(long nftHorseId)
    {
        var newModel = Current.Clone();
        newModel.CurrentHorseNftId = nftHorseId;
        await UpdateModelAsync(new[] { newModel });
    }
    
    private static UniTask<IEnumerable<PlayerInfo>> GetUserDataModels()
    {
        return UniTask.FromResult(Enumerable.Empty<PlayerInfo>());
    }
}

public interface IReadOnlyUserDataRepository : IReadOnlyRepository<long, UserDataModel>
{
    UserDataModel Current { get; }
}

public interface IUserDataRepository : IRepository<long, PlayerInfo, UserDataModel> 
{
    UserDataModel Current { get; }
    UniTask UpdateCoin(long coin);
    UniTask UpdateHorse(long nftHorseId);
}