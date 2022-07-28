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
            Exp = UserDataRepository.Current.Exp,
            Level = UserDataRepository.Current.Level,
            NextLevelExp = UserDataRepository.Current.NextLevelExp,
            TraningTimeStamp = UserDataRepository.Current.TraningTimeStamp,
        };
        await UserDataRepository.UpdateDataAsync(new UserDataModel[] { model });
    }

    public async UniTask<RaceMatchData> FindMatch()
    {
        HorseRaceTime[] GetAllMasterHorseIds()
        {
            return container.Inject<MasterHorseContainer>().MasterHorseIndexer.Keys
                            .Shuffle()
                            .Append(UserDataRepository.Current.MasterHorseId)
                            .Shuffle()
                            .Take(8)
                            .Select(x => new HorseRaceTime()
                            {
                                masterHorseId = x,
                                time = 15 + UnityEngine.Random.Range(-1.0f, 1.0f),
                                raceSegments = GenerateRandomSegment()
                            })
                            .ToArray();
        }

        await UniTask.Delay(1000);
        return new RaceMatchData()
        {
            horseRaceTimes = GetAllMasterHorseIds(),
            masterMapId = 10001002,
            mode = RaceMode.QuickMode
        };
    }

    private RaceSegment[] GenerateRandomSegment()
    {
        return Enumerable.Range(0, 4)
            .Select(x => GenerateRandomSegment(x, 2.0f))
            .ToArray();
    }

    private RaceSegment GenerateRandomSegment(int id, float averageTime)
    {
        int numberSegment = 10;
        return new RaceSegment()
        {
            id = id,
            currentLane = UnityEngine.Random.Range(0, 8),
            toLane = UnityEngine.Random.Range(0, 8),
            waypoints = Enumerable.Range(1, numberSegment)
            .Select(x => new WayPoints()
            {
                percentage = x * 0.1f,
                time = averageTime + UnityEngine.Random.Range(-0.25f, 0.25f),
            }).ToArray(),
        };
    }
}