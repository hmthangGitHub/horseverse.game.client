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
    UniTask<RaceScriptData> FindMatch(long ntfHorseId, RacingMode racingMode);
    UniTask CancelFindMatch(long ntfHorseId);
    event Action<int> OnConnectedPlayerChange;
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
    private UniTaskCompletionSource<StartRoomReceipt> findMatchUcs;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= Container.Inject<MasterHorseContainer>();
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();
    public event Action<int> OnConnectedPlayerChange  = ActionUtility.EmptyAction<int>.Instance;
    
    public QuickRaceDomainService(IDIContainer container) : base(container){}

    public async UniTask ChangeHorse(long horseNtfId)
    {
        await UserDataRepository.UpdateHorse(horseNtfId);
    }

    public async UniTask<RaceScriptData> FindMatch(long ntfHorseId, RacingMode racingMode)
    {
        JoinPool(ntfHorseId, racingMode).Forget();
        return new RaceScriptData()
        {
            HorseRaceInfos = GetHorseRaceInfos((await findMatchUcs.Task).RaceScript, MasterHorseContainer),
            MasterMapId = QuickRaceState.MasterMapId,
        };
    }

    private async UniTaskVoid JoinPool(long ntfHorseId, RacingMode racingMode)
    {
        
        findMatchUcs = new UniTaskCompletionSource<StartRoomReceipt>();
        var joinRoomResponse = await SocketClient.Send<JoinRoomRequest, JoinRoomResponse>(new JoinRoomRequest()
        {
            HorseId = ntfHorseId,
            RacingMode = racingMode,
            
        });
        
        OnConnectedPlayerChange.Invoke(joinRoomResponse.RaceRoom.HorseInfos.Count);
        
        SocketClient.Subscribe<StartRoomReceipt>(StartRoomReceiptResponse);
        SocketClient.Subscribe<UpdateRoomReceipt>(UpdateRoomReceiptResponse);
    }

    private void UpdateRoomReceiptResponse(UpdateRoomReceipt response)
    {
        OnConnectedPlayerChange.Invoke(response.RaceRoom.HorseInfos.Count);
    }

    private void StartRoomReceiptResponse(StartRoomReceipt raceScriptResponse)
    {
        UserDataRepository.UpdateDailyRacingNumber(raceScriptResponse.FreeRacingNumber).Forget();
        findMatchUcs.TrySetResult(raceScriptResponse);
        SocketClient.UnSubscribe<StartRoomReceipt>(StartRoomReceiptResponse);
        SocketClient.UnSubscribe<UpdateRoomReceipt>(UpdateRoomReceiptResponse);
        findMatchUcs = default;
    }

    public async UniTask CancelFindMatch(long ntfHorseId)
    {
        await SocketClient.Send<ExitRoomRequest, ExitRoomResponse>(new ExitRoomRequest()
        {
            HorseId = ntfHorseId
        });
        findMatchUcs.TrySetCanceled();
        findMatchUcs = default;
        SocketClient.UnSubscribe<StartRoomReceipt>(StartRoomReceiptResponse);
        SocketClient.UnSubscribe<UpdateRoomReceipt>(UpdateRoomReceiptResponse);
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
                    DelayTime = x.First().stat.DelayTime,
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

    public event Action<int> OnConnectedPlayerChange;

    public async UniTask ChangeHorse(long horseNtfId)
    {
        await UserDataRepository.UpdateHorse(horseNtfId);
    }

    public async UniTask<RaceScriptData> FindMatch(long ntfHorseId, RacingMode racingMode)
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

        return new RaceScriptData()
        {
            HorseRaceInfos = GetAllMasterHorseIds(),
            MasterMapId = QuickRaceState.MasterMapId,
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