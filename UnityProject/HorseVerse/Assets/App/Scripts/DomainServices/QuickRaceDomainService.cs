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
            HorseRaceInfos = GetHorseRaceInfos(response.RaceScript, MasterHorseContainer),
            MasterMapId = QuickRaceState.MasterMapId,
            Mode = RaceMode.Race
        };
    }

    public static HorseRaceInfo[] GetHorseRaceInfos(RaceScript responseRaceScript, MasterHorseContainer masterHorseContainer)
    {
        return responseRaceScript.Phases.SelectMany(x =>
                x.HorseStats.Select((stat, i) => (stat: stat, horseIndex: i, start: x.Start, end: x.End)))
            .GroupBy(x => x.horseIndex)
            .Select(x =>
            {
                var horseInfo = responseRaceScript.HorseInfos[x.Key];
                var masterHorseId = 10000000 + (int)horseInfo.HorseType; //10000001; // TODO get from server
                return new HorseRaceInfo()
                {
                    DelayTime = x.First()
                                 .stat.DelayTime,
                    RaceSegments = x.Select(info => new RaceSegmentTime()
                                    {
                                        currentLane = info.stat.LaneStart,
                                        ToLane = info.stat.LaneEnd,
                                        Time = info.stat.Time,
                                        Percentage = (float)(info.end) / responseRaceScript.TotalLength
                                    })
                                    .ToArray(),
                    MeshInformation = new MasterHorseMeshInformation()
                    {
                        masterHorseId = masterHorseId,
                        color1 = HorseRepository.GetColorFromHexCode(horseInfo.Color1),
                        color2 = HorseRepository.GetColorFromHexCode(horseInfo.Color2),
                        color3 = HorseRepository.GetColorFromHexCode(horseInfo.Color3),
                        color4 = HorseRepository.GetColorFromHexCode(horseInfo.Color4),
                    },
                    Name = horseInfo.Name,
                    PowerBonus = horseInfo.Bms,
                    PowerRatio = 1,
                    SpeedBonus = horseInfo.Mms,
                    SpeedRatio = 1,
                    TechnicallyBonus = horseInfo.Acceleration,
                    TechnicallyRatio = 1,
                    Rarity = (int)horseInfo.Rarity,
                    Type = (int)horseInfo.HorseType,
                    Level = horseInfo.Level,
                };
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
        HorseRaceInfo[] GetAllMasterHorseIds()
        {
            return Container.Inject<MasterHorseContainer>().MasterHorseIndexer.Keys
                            .Shuffle()
                            .Append(UserDataRepository.Current.CurrentHorseNftId)
                            .Shuffle()
                            .Take(8)
                            .Select(x => new HorseRaceInfo()
                            {
                                // masterHorseId = x,
                                RaceSegments = GenerateRandomSegment()
                            })
                            .ToArray();
        }

        return new RaceMatchData()
        {
            HorseRaceInfos = GetAllMasterHorseIds(),
            MasterMapId = QuickRaceState.MasterMapId,
            Mode = RaceMode.Race
        };
    }

    private RaceSegmentTime[] GenerateRandomSegment()
    {
        return Enumerable.Range(0, 3)
            .Select(x => GenerateRandomSegment(x, 2.0f))
            .ToArray();
    }

    private RaceSegmentTime GenerateRandomSegment(int id, float averageTime)
    {
        // int numberSegment = 10;
        return new RaceSegmentTime()
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