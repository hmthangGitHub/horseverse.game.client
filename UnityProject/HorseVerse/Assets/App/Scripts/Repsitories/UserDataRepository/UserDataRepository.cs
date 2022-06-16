#define MOCK_DATA
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataRepository : Repository<string, UserDataModel, UserDataModel>, IReadOnlyUserDataRepository
{
    public UserDataRepository() : base(x => x.UserId, x => x, GetUserDataModels)
    {
    }

    public UserDataModel Current => Models.First().Value;

    private static async UniTask<IEnumerable<UserDataModel>> GetUserDataModels()
    {
#if MOCK_DATA
        await UniTask.CompletedTask;
        return new UserDataModel[] { new UserDataModel()
        {
            UserId = System.Guid.NewGuid().ToString(),
            Coin = UnityEngine.Random.Range(100, 1000),
            Energy = UnityEngine.Random.Range(100, 1000),
            MaxEnergy = UnityEngine.Random.Range(100,1000),
            UserName = $"HorseVerse {UnityEngine.Random.Range(1, 1000)}",
            MasterHorseId = 10000001
        } }; 
#endif
    }
}

public interface IReadOnlyUserDataRepository : IReadOnlyRepository<string, UserDataModel>
{
    UserDataModel Current { get; }
}