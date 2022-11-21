using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrainingDomainService
{
    UniTask SendHorseToTraining(long masterHorseId);
    UniTask OnDoneTraningPeriod(long masterHorseId);
}

public class TrainingDomainServiceBase
{
    protected IUserDataRepository userDataRepository;
    protected IDIContainer container;
    protected IUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IUserDataRepository>();
    public TrainingDomainServiceBase(IDIContainer container)
    {
        this.container = container;
    }
}

public class TrainingDomainService : TrainingDomainServiceBase, ITrainingDomainService
{
    public TrainingDomainService(IDIContainer container) : base(container) { }

    public UniTask OnDoneTraningPeriod(long masterHorseId)
    {
        throw new NotImplementedException();
    }

    public UniTask SendHorseToTraining(long masterHorseId)
    {
        throw new System.NotImplementedException();
    }
}

public class LocalTraningDomainService : TrainingDomainServiceBase, ITrainingDomainService
{
    public LocalTraningDomainService(IDIContainer container) : base(container) { }

    public async UniTask OnDoneTraningPeriod(long masterHorseId)
    {
        await UniTask.Delay(500);
        var model = new UserDataModel()
        {
            Coin = UserDataRepository.Current.Coin,
            Energy = UserDataRepository.Current.Energy,
            CurrentHorseNftId = masterHorseId,
            MaxEnergy = UserDataRepository.Current.MaxEnergy,
            UserId = UserDataRepository.Current.UserId,
            UserName = UserDataRepository.Current.UserName,
            TraningTimeStamp = 0,
        };
        await UserDataRepository.UpdateDataAsync(new UserDataModel[] { model });
    }

    public async UniTask SendHorseToTraining(long masterHorseId)
    {
        await UniTask.Delay(500);
        var model = new UserDataModel()
        {
            Coin = UserDataRepository.Current.Coin,
            Energy = UserDataRepository.Current.Energy,
            CurrentHorseNftId = masterHorseId,
            MaxEnergy = UserDataRepository.Current.MaxEnergy,
            UserId = UserDataRepository.Current.UserId,
            UserName = UserDataRepository.Current.UserName,
            TraningTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 10,
        };
        await UserDataRepository.UpdateDataAsync(new UserDataModel[] { model });
    }
}
