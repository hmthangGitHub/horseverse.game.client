using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using io.hverse.game.protogen;

public interface IQuickRaceDomainService
{
    UniTask ChangeHorse(long horseNtfId);
    UniTask<RaceScriptData> FindMatch(long ntfHorseId, RacingMode racingMode);
    UniTask CancelFindMatch(long ntfHorseId);
    event Action<long, int>  OnConnectedPlayerChange;
    void OnResumeFromSoftClose(long matchId, int numberCurrentPlayer, int maxNumberPlayer);
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
    private UniTaskCompletionSource<RaceScript> findMatchUcs;
    private HorseRaceInfoFactory horseRaceInfoFactory;
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();
    private HorseRaceInfoFactory HorseRaceInfoFactory => horseRaceInfoFactory ??= Container.Inject<HorseRaceInfoFactory>();
    public event Action<long, int> OnConnectedPlayerChange  = ActionUtility.EmptyAction<long, int>.Instance;
    
    public void OnResumeFromSoftClose(long matchId,
                                      int numberCurrentPlayer,
                                      int maxNumberPlayer)
    {
        OnResumeFromSoftCloseAsync(matchId, numberCurrentPlayer, maxNumberPlayer).Forget();
    }

    private async UniTaskVoid OnResumeFromSoftCloseAsync(long matchId,
                                                         int numberCurrentPlayer,
                                                         int maxNumberPlayer)
    {
        if (numberCurrentPlayer < maxNumberPlayer)
        {
            SocketClient.UnSubscribe<UpdateRoomReceipt>(UpdateRoomReceiptResponse);
            SocketClient.UnSubscribe<StartRoomReceipt>(OnStartRoomReceiptResponse);
            
            var updateResponse = await SocketClient.Send<UpdateRoomRequest, UpdateRoomResponse>(new UpdateRoomRequest()
            {
                RoomId = matchId
            });
            
            if (updateResponse.RaceRoom.HorseInfos.Count == maxNumberPlayer)
            {
                await RequestToStartRoom(matchId);
            }
            else
            {
                SocketClient.Subscribe<UpdateRoomReceipt>(UpdateRoomReceiptResponse);
                SocketClient.Subscribe<StartRoomReceipt>(OnStartRoomReceiptResponse);
            }
        }
        else
        {
            SocketClient.UnSubscribe<UpdateRoomReceipt>(UpdateRoomReceiptResponse);
            SocketClient.UnSubscribe<StartRoomReceipt>(OnStartRoomReceiptResponse);
            
            await RequestToStartRoom(matchId);
        }
    }

    private async UniTask RequestToStartRoom(long matchId)
    {
        var startRoomResponse = await SocketClient.Send<StartRoomRequest, StartRoomResponse>(new StartRoomRequest()
        {
            RoomId = matchId
        });
        OnStartMatch(startRoomResponse.PlayerInfo, startRoomResponse.RaceScript);
    }

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
            HorseRaceInfos = HorseRaceInfoFactory.GetHorseRaceInfos((await findMatchUcs.Task)),
            MasterMapId = RacingState.MasterMapId,
        };
    }

    private async UniTaskVoid JoinPool(long ntfHorseId, RacingMode racingMode)
    {
        
        findMatchUcs = new UniTaskCompletionSource<RaceScript>();
        var joinRoomResponse = await SocketClient.Send<JoinRoomRequest, JoinRoomResponse>(new JoinRoomRequest()
        {
            HorseId = ntfHorseId,
            RacingMode = racingMode,
            
        });
        
        OnConnectedPlayerChange.Invoke(joinRoomResponse.RaceRoom.RoomId, joinRoomResponse.RaceRoom.HorseInfos.Count);
        
        SocketClient.Subscribe<StartRoomReceipt>(OnStartRoomReceiptResponse);
        SocketClient.Subscribe<UpdateRoomReceipt>(UpdateRoomReceiptResponse);
    }

    private void UpdateRoomReceiptResponse(UpdateRoomReceipt response)
    {
        OnConnectedPlayerChange.Invoke(response.RaceRoom.RoomId, response.RaceRoom.HorseInfos.Count);
    }

    private void OnStartRoomReceiptResponse(StartRoomReceipt receipt)
    {
        OnStartMatch(receipt.PlayerInfo, receipt.RaceScript);
    }

    private void OnStartMatch(LitePlayerInfo litePlayerInfo, RaceScript raceScript)
    {
        UserDataRepository.UpdateLightPlayerInfoAsync(litePlayerInfo).Forget();
        findMatchUcs.TrySetResult(raceScript);
        SocketClient.UnSubscribe<StartRoomReceipt>(raceScriptResponse1 => OnStartMatch(raceScriptResponse1.PlayerInfo, raceScriptResponse1.RaceScript));
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
        SocketClient.UnSubscribe<StartRoomReceipt>(raceScriptResponse => OnStartMatch(raceScriptResponse.PlayerInfo, raceScriptResponse.RaceScript));
        SocketClient.UnSubscribe<UpdateRoomReceipt>(UpdateRoomReceiptResponse);
    }
}

public class LocalQuickRaceDomainService : QuickRaceDomainServiceBase, IQuickRaceDomainService
{
    public LocalQuickRaceDomainService(IDIContainer container) : base(container) { }

    public async UniTask CancelFindMatch(long ntfHorseId)
    {
        await UniTask.CompletedTask;
    }

    public event Action<long, int> OnConnectedPlayerChange;

    public void OnResumeFromSoftClose(long matchId,
                                      int numberCurrentPlayer,
                                      int maxNumberPlayer)
    {
        throw new NotImplementedException();
    }

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
                                RaceSegments = GenerateRandomSegment()
                            })
                            .ToArray();
        }

        return new RaceScriptData()
        {
            HorseRaceInfos = GetAllMasterHorseIds(),
            MasterMapId = RacingState.MasterMapId,
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