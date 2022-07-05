using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public interface IQuickRaceDomainService
{
    UniTask ChangeHorse(long masterHorseId);
    UniTask<RaceMatchData> FindMatch();
    UniTask CancelFindMatch();
}

public class QuickRaceDomainServiceBase
{
    protected IDIContainer container;
    protected IUserDataRepository userDataRepository;
    protected IUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IUserDataRepository>();

    public QuickRaceDomainServiceBase(IDIContainer container)
    {
        this.container = container;
    }
}

public class QuickRaceDomainService : QuickRaceDomainServiceBase, IQuickRaceDomainService
{
    public QuickRaceDomainService(IDIContainer container) : base(container){}

    public UniTask CancelFindMatch()
    {
        throw new NotImplementedException();
    }

    public UniTask ChangeHorse(long masterHorseId)
    {
        throw new NotImplementedException();
    }

    public UniTask<RaceMatchData> FindMatch()
    {
        throw new NotImplementedException();
    }
}

public class LocalQuickRaceDomainService : QuickRaceDomainServiceBase, IQuickRaceDomainService
{
    public LocalQuickRaceDomainService(IDIContainer container) : base(container) { }

    public async UniTask CancelFindMatch()
    {
        await UniTask.CompletedTask;
    }

    public async UniTask ChangeHorse(long masterHorseId)
    {
        await UniTask.Delay(500);
        var model = new UserDataModel()
        {
            Coin = UserDataRepository.Current.Coin,
            Energy = UserDataRepository.Current.Energy,
            MasterHorseId = masterHorseId,
            MaxEnergy = UserDataRepository.Current.MaxEnergy,
            UserId = UserDataRepository.Current.UserId,
            UserName = UserDataRepository.Current.UserName,
        };
        await UserDataRepository.UpdateDataAsync(new UserDataModel[] { model });
    }

    public async UniTask<RaceMatchData> FindMatch()
    {
        int[] GetHorseTopPosition()
        {
            return Enumerable.Range(1, 8).Shuffle().ToArray();
        }

        long[] GetAllMasterHorseIds()
        {
            return container.Inject<MasterHorseContainer>().MasterHorseIndexer.Keys
                            .Shuffle()
                            .Append(UserDataRepository.Current.MasterHorseId)
                            .Shuffle()
                            .Take(8)
                            .ToArray();
        }

        await UniTask.Delay(10000);
        return new RaceMatchData()
        {
            masterHorseIds = GetAllMasterHorseIds(),
            tops = GetHorseTopPosition(),
            masterMapId = 10001002
        };
    }
}