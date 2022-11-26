using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading;
using io.hverse.game.protogen;

public interface IQuickRaceDomainService
{
    UniTask ChangeHorse(long horseNtfId);
    UniTask<RaceMatchData> FindMatch();
    UniTask CancelFindMatch();
}

public class QuickRaceDomainServiceBase
{
    protected IDIContainer Container { get;}
    private IUserDataRepository userDataRepository;
    protected IUserDataRepository UserDataRepository => userDataRepository ??= Container.Inject<IUserDataRepository>();

    protected QuickRaceDomainServiceBase(IDIContainer container)
    {
        this.Container = container;
    }
}

public class QuickRaceDomainService : QuickRaceDomainServiceBase, IQuickRaceDomainService
{
    private ISocketClient socketClient;
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();

    public QuickRaceDomainService(IDIContainer container) : base(container){}

    public UniTask CancelFindMatch()
    {
        return UniTask.CompletedTask;
    }

    public async UniTask ChangeHorse(long horseNtfId)
    {
        await UniTask.Delay(500);
        var model = new UserDataModel()
        {
            Coin = UserDataRepository.Current.Coin,
            Energy = UserDataRepository.Current.Energy,
            CurrentHorseNftId = horseNtfId,
            MaxEnergy = UserDataRepository.Current.MaxEnergy,
            UserId = UserDataRepository.Current.UserId,
            UserName = UserDataRepository.Current.UserName,
            Exp = UserDataRepository.Current.Exp,
            Level = UserDataRepository.Current.Level,
            NextLevelExp = UserDataRepository.Current.NextLevelExp,
            TraningTimeStamp = UserDataRepository.Current.TraningTimeStamp,
        };
        await UserDataRepository.UpdateModelAsync(new[] { model });
    }

    public async UniTask<RaceMatchData> FindMatch()
    {
        var response = await SocketClient.Send<RaceScriptRequest, RaceScriptResponse>(new RaceScriptRequest());
        return new RaceMatchData()
        {
            HorseRaceTimes = GetHorseRaceTimes(response.RaceScript, MasterHorseContainer),
            MasterMapId = 10001002,
            Mode = RaceMode.QuickMode
        };
    }

    public static HorseRaceTime[] GetHorseRaceTimes(RaceScript responseRaceScript, MasterHorseContainer masterHorseContainer)
    {
        return responseRaceScript.Phases.SelectMany(x =>
                x.HorseStats.Select((stat, i) => (stat: stat, horseIndex: i, start: x.Start, end: x.End)))
            .GroupBy(x => x.horseIndex)
            .Select(x => new HorseRaceTime()
            {
                delayTime = x.First().stat.DelayTime,
                raceSegments = x.Select(info => new RaceSegment()
                {
                    currentLane = info.stat.LaneStart,
                    toLane = info.stat.LaneEnd,
                    time = info.stat.Time,
                    percentage = (float)(info.end) / responseRaceScript.TotalLength
                }).ToArray(),
                masterHorseId = masterHorseContainer.MasterHorseIndexer.Keys.First()
            }).ToArray();
    }
}

public class LocalQuickRaceDomainService : QuickRaceDomainServiceBase, IQuickRaceDomainService
{
    public LocalQuickRaceDomainService(IDIContainer container) : base(container) { }

    public async UniTask CancelFindMatch()
    {
        await UniTask.CompletedTask;
    }

    public async UniTask ChangeHorse(long horseNtfId)
    {
        await UniTask.Delay(500);
        var model = new UserDataModel()
        {
            Coin = UserDataRepository.Current.Coin,
            Energy = UserDataRepository.Current.Energy,
            CurrentHorseNftId = horseNtfId,
            MaxEnergy = UserDataRepository.Current.MaxEnergy,
            UserId = UserDataRepository.Current.UserId,
            UserName = UserDataRepository.Current.UserName,
            Exp = UserDataRepository.Current.Exp,
            Level = UserDataRepository.Current.Level,
            NextLevelExp = UserDataRepository.Current.NextLevelExp,
            TraningTimeStamp = UserDataRepository.Current.TraningTimeStamp,
        };
        await UserDataRepository.UpdateModelAsync(new UserDataModel[] { model });
    }

    public async UniTask<RaceMatchData> FindMatch()
    {
        HorseRaceTime[] GetAllMasterHorseIds()
        {
            return Container.Inject<MasterHorseContainer>().MasterHorseIndexer.Keys
                            .Shuffle()
                            .Append(UserDataRepository.Current.CurrentHorseNftId)
                            .Shuffle()
                            .Take(8)
                            .Select(x => new HorseRaceTime()
                            {
                                masterHorseId = x,
                                raceSegments = GenerateRandomSegment()
                            })
                            .ToArray();
        }

        return new RaceMatchData()
        {
            HorseRaceTimes = GetAllMasterHorseIds(),
            MasterMapId = 10001002,
            Mode = RaceMode.QuickMode
        };
    }

    private RaceSegment[] GenerateRandomSegment()
    {
        return Enumerable.Range(0, 3)
            .Select(x => GenerateRandomSegment(x, 2.0f))
            .ToArray();
    }

    private RaceSegment GenerateRandomSegment(int id, float averageTime)
    {
        // int numberSegment = 10;
        return new RaceSegment()
        {
            // id = id,
            // currentLane = UnityEngine.Random.Range(0, 8),
            // toLane = UnityEngine.Random.Range(0, 8),
            // waypoints = Enumerable.Range(1, numberSegment)
            // .Select(x => new WayPoints()
            // {
            //     percentage = x * 0.1f,
            //     time = averageTime + UnityEngine.Random.Range(-0.25f, 0.25f),
            // }).ToArray(),
        };
    }
}