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
    UniTask<RaceMatchData> FindMatch(long ntfHorseId);
    UniTask CancelFindMatch(long ntfHorseId);
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
    private UniTaskCompletionSource<RaceScriptResponse> findMatchUcs;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();

    public QuickRaceDomainService(IDIContainer container) : base(container){}

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

    public async UniTask<RaceMatchData> FindMatch(long ntfHorseId)
    {
        JoinPool(ntfHorseId).Forget();
        return new RaceMatchData()
        {
            HorseRaceInfos = GetHorseRaceInfos((await findMatchUcs.Task).RaceScript, MasterHorseContainer),
            MasterMapId = QuickRaceState.MasterMapId,
            Mode = RaceMode.Race
        };
    }

    private async UniTaskVoid JoinPool(long ntfHorseId)
    {
        findMatchUcs = new UniTaskCompletionSource<RaceScriptResponse>();
        await SocketClient.Send<JoinPoolRequest, JoinPoolResponse>(new JoinPoolRequest()
        {
            HorseId = ntfHorseId
        });
        SocketClient.Subscribe<RaceScriptResponse>(OnRacingScriptResponse);
    }

    private void OnRacingScriptResponse(RaceScriptResponse raceScriptResponse)
    {
        findMatchUcs.TrySetResult(raceScriptResponse);
        SocketClient.UnSubscribe<RaceScriptResponse>(OnRacingScriptResponse);
        findMatchUcs = default;
    }

    public async UniTask CancelFindMatch(long ntfHorseId)
    {
        await SocketClient.Send<ExitPoolRequest, ExitPoolResponse>(new ExitPoolRequest()
        {
            HorseId = ntfHorseId
        });
        findMatchUcs.TrySetCanceled();
        findMatchUcs = default;
        SocketClient.UnSubscribe<RaceScriptResponse>(OnRacingScriptResponse);
    }

    public static HorseRaceInfo[] GetHorseRaceInfos(RaceScript responseRaceScript, MasterHorseContainer masterHorseContainer)
    {
        return responseRaceScript.Phases.SelectMany(x =>
                x.HorseStats.Select((stat, i) => (stat: stat, horseId: stat.HorseId, start: x.Start, end: x.End)))
            .GroupBy(x => x.horseId)
            .Select(x =>
            {
                var horseInfo = responseRaceScript.HorseInfos.First(info => info.NftId == x.Key);
                var masterHorse = masterHorseContainer.FromTypeToMasterHorse((int)horseInfo.HorseType);
                return new HorseRaceInfo()
                {
                    DelayTime = x.First()
                                 .stat.DelayTime,
                    RaceSegments = x.Select(info => new RaceSegmentTime()
                                    {
                                        CurrentLane = info.stat.LaneStart,
                                        ToLane = info.stat.LaneEnd,
                                        Time = info.stat.Time,
                                        Percentage = (float)(info.end) / responseRaceScript.TotalLength
                                    })
                                    .ToArray(),
                    MeshInformation = new MasterHorseMeshInformation()
                    {
                        masterHorseId = masterHorse.MasterHorseId,
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
            })
            .OrderBy(x => x.RaceSegments.First().CurrentLane)
            .ToArray();
    }
}

public class LocalQuickRaceDomainService : QuickRaceDomainServiceBase, IQuickRaceDomainService
{
    public LocalQuickRaceDomainService(IDIContainer container) : base(container) { }

    public async UniTask CancelFindMatch(long ntfHorseId)
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

    public async UniTask<RaceMatchData> FindMatch(long ntfHorseId)
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