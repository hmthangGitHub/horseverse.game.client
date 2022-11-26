using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataRepository : Repository<long, UserDataModel, UserDataModel>, IReadOnlyUserDataRepository, IUserDataRepository
{
    public UserDataRepository() : base(x => x.UserId, x => x, GetUserDataModels)
    {
    }

    public UserDataModel Current => Models.First().Value;
    public async UniTask UpdateCoin(long coin)
    {
        var newModel = Current.Clone();
        newModel.Coin = coin;
        await UpdateModelAsync(new[] { newModel });
    }

    private static UniTask<IEnumerable<UserDataModel>> GetUserDataModels()
    {
        return UniTask.FromResult(Enumerable.Empty<UserDataModel>());
    }
}

public interface IReadOnlyUserDataRepository : IReadOnlyRepository<long, UserDataModel>
{
    UserDataModel Current { get; }
}

public interface IUserDataRepository : IRepository<long, UserDataModel, UserDataModel> 
{
    UserDataModel Current { get; }
    UniTask UpdateCoin(long coin);
}