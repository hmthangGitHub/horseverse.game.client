using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataRepository : Repository<int, UserDataModel, UserDataModel>, IReadOnlyUserDataRepository, IUserDataRepository
{
    public UserDataRepository() : base(x => x.UserId, x => x, GetUserDataModels)
    {
    }

    public UserDataModel Current => Models.First().Value;

    private static UniTask<IEnumerable<UserDataModel>> GetUserDataModels()
    {
        return UniTask.FromResult(Enumerable.Empty<UserDataModel>());
    }
}

public interface IReadOnlyUserDataRepository : IReadOnlyRepository<int, UserDataModel>
{
    UserDataModel Current { get; }
}

public interface IUserDataRepository : IRepository<int, UserDataModel, UserDataModel> 
{
    UserDataModel Current { get; }
}