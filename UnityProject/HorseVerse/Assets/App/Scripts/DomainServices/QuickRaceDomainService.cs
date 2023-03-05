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
    private UniTaskCompletionSource<StartRoomReceipt> findMatchUcs;
    private HorseRaceInfoFactory horseRaceInfoFactory;
    private ISocketClient SocketClient => socketClient ??= Container.Inject<ISocketClient>();
    private HorseRaceInfoFactory HorseRaceInfoFactory => horseRaceInfoFactory ??= Container.Inject<HorseRaceInfoFactory>();
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
            HorseRaceInfos = HorseRaceInfoFactory.GetHorseRaceInfos((await findMatchUcs.Task).RaceScript),
            MasterMapId = RacingState.MasterMapId,
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
        UserDataRepository.UpdateLightPlayerInfoAsync(raceScriptResponse.PlayerInfo).Forget();
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