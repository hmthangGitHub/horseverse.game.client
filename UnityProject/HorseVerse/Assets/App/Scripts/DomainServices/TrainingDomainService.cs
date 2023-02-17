using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.hverse.game.protogen;

public interface ITrainingDomainService
{
    UniTask SendHorseToTraining(long masterHorseId);
    UniTask OnDoneTraningPeriod(long masterHorseId);
    UniTask<StartTrainingResponse> StartTrainingData(long HorseId);
    UniTask<FinishTrainingResponse> GetTrainingRewardData(int distance, int coin);
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

    public UniTask<StartTrainingResponse> StartTrainingData(long HorseId)
    {
        throw new System.NotImplementedException();
    }

    public UniTask<FinishTrainingResponse> GetTrainingRewardData(int distance, int coin)
    {
        throw new System.NotImplementedException();
    }
}

public class LocalTraningDomainService : TrainingDomainServiceBase, ITrainingDomainService
{
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();

    public LocalTraningDomainService(IDIContainer container) : base(container) { }

    public async UniTask OnDoneTraningPeriod(long masterHorseId)
    {
        await UniTask.Delay(500);
        var model = new UserDataModel()
        {
            Coin = UserDataRepository.Current.Coin,
            Energy = UserDataRepository.Current.Energy,
            CurrentHorseNftId = masterHorseId,
            UserId = UserDataRepository.Current.UserId,
            UserName = UserDataRepository.Current.UserName,
            TraningTimeStamp = 0,
        };
        await UserDataRepository.UpdateModelAsync(new UserDataModel[] { model });
    }

    public async UniTask SendHorseToTraining(long masterHorseId)
    {
        await UniTask.Delay(500);
        var model = new UserDataModel()
        {
            Coin = UserDataRepository.Current.Coin,
            Energy = UserDataRepository.Current.Energy,
            CurrentHorseNftId = masterHorseId,
            UserId = UserDataRepository.Current.UserId,
            UserName = UserDataRepository.Current.UserName,
            TraningTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 10,
        };
        await UserDataRepository.UpdateModelAsync(new UserDataModel[] { model });
    }

    public async UniTask<StartTrainingResponse> StartTrainingData(long HorseId)
    {
        var response = await SocketClient.Send<StartTrainingRequest, StartTrainingResponse>(new StartTrainingRequest()
        {
            HorseId = HorseId,
        }, 5.0f);

        return response;
    }

    public async UniTask<FinishTrainingResponse> GetTrainingRewardData(int distance, int coin)
    {
        var trainingRewardsResponse = await SocketClient.Send<FinishTrainingRequest, FinishTrainingResponse>(new FinishTrainingRequest()
        {
            CoinNumber = coin,
            Distance = distance,
        }, 5.0f);

        return trainingRewardsResponse;
    }
}
