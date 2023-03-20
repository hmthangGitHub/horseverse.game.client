using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

public class HorseRaceStatusPresenter : IDisposable
{
    private readonly IHorseRaceManager horseRaceManager;
    private readonly IHorseRaceInGameStatus[] horseRaceInGameStatus;
    private readonly int[] playerList;
    private readonly int playerHorseIndex;
    private readonly bool isReplay;
    private readonly bool isShowSelfRank;
    private readonly int[] cachedPositions;
    private UIHorseRaceStatus uiHorseRaceStatus;
    private CancellationTokenSource cts;
    public event Action OnSkip = ActionUtility.EmptyAction.Instance;

    public HorseRaceStatusPresenter(IHorseRaceManager horseRaceManager, 
                                    IHorseRaceInGameStatus[] horseRaceInGameStatus,
                                    int[] playerList,
                                    int playerHorseIndex,
                                    bool isReplay,
                                    bool isShowSelfRank)
    {
        this.horseRaceManager = horseRaceManager;
        this.horseRaceInGameStatus = horseRaceInGameStatus;
        this.playerList = playerList;
        this.playerHorseIndex = playerHorseIndex;
        this.isReplay = isReplay;
        this.isShowSelfRank = isShowSelfRank;
        cachedPositions = Enumerable.Repeat(-1, playerList.Length).ToArray();
        cts = new CancellationTokenSource();
    }

    public async UniTask Initialize()
    {
        uiHorseRaceStatus ??= await UILoader.Instantiate<UIHorseRaceStatus>(token: cts.Token);
        SetEntityUIHorseRaceStatus();
        StartUpdateRaceHorseStatus().Forget();
    }
    
    private void SetEntityUIHorseRaceStatus()
    {
        uiHorseRaceStatus.SetEntity(new UIHorseRaceStatus.Entity()
        {
            playerList = new HorseRaceStatusPlayerList.Entity()
            {
                horseIdInLane = playerList,
                playerId = playerHorseIndex,
            },
            selfRaceRankGroup = isShowSelfRank,
            isReplay = isReplay,
            skipBtn = new ButtonComponent.Entity(() => OnSkip())
        });
        uiHorseRaceStatus.In().Forget();
    }
    
    private async UniTaskVoid StartUpdateRaceHorseStatus()
    {
        while (!cts.Token.IsCancellationRequested)
        {
            await UniTask.Yield(cts.Token);
            UpdateRaceStatus();
        }
    }

    private void UpdateRaceStatus()
    {
        var horseControllersOrderByRank = horseRaceInGameStatus.OrderByDescending(x => x.CurrentRaceProgressWeight)
                                                          .Select(x => x)
                                                          .ToArray();
        for (var i = 0; i < horseControllersOrderByRank.Length; i++)
        {
            if (cachedPositions[i] != horseControllersOrderByRank[i].InitialLane)
            {
                uiHorseRaceStatus.playerList.ChangePosition(horseControllersOrderByRank[i].InitialLane, i);
                cachedPositions[i] = horseControllersOrderByRank[i].InitialLane;
            }
            
            if (i == 0) uiHorseRaceStatus.UpdateFirstRank(horseControllersOrderByRank[i].Name);
            if (i == 1) uiHorseRaceStatus.UpdateSecondRank(horseControllersOrderByRank[i].Name);
            if (horseControllersOrderByRank[i].IsPlayer) uiHorseRaceStatus.UpdateSelfRank(i);
        }
        uiHorseRaceStatus.UpdateNormalizeTime(horseRaceManager.NormalizedRaceTime);
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiHorseRaceStatus);
    }
}