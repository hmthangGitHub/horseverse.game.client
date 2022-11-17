#define MOCK_DATA
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataRepository : Repository<string, UserDataModel, UserDataModel>, IReadOnlyUserDataRepository, IUserDataRepository
{
    public UserDataRepository() : base(x => x.UserId, x => x, GetUserDataModels)
    {
    }

    private string currentModelId = "0";
    public string CurrentModelId => currentModelId;
    public UserDataModel Current => Models.ContainsKey(currentModelId) ? Models[currentModelId] : default;
    
    public void SetCurrentUserDataModelId(string _id)
    {
        currentModelId = _id;
    }

    private static async UniTask<IEnumerable<UserDataModel>> GetUserDataModels()
    {
#if MOCK_DATA
        await UniTask.CompletedTask;
        return new UserDataModel[] { new UserDataModel()
        {
            UserId = "0",
            Coin = 0,
            Energy = 0,
            MaxEnergy = 0,
            UserName = $"HorseVerse",
            MasterHorseId = 0,
            Level = 1,
            Exp = 0,
            NextLevelExp = 100
        } };
#endif
        return default;
    }
}

public interface IReadOnlyUserDataRepository : IReadOnlyRepository<string, UserDataModel>
{
    UserDataModel Current { get; }
    string CurrentModelId { get; }
    public void SetCurrentUserDataModelId(string _id);
}

public interface IUserDataRepository : IRepository<string, UserDataModel, UserDataModel> 
{
    UserDataModel Current { get; }
    string CurrentModelId { get; }
    public void SetCurrentUserDataModelId(string _id);
}