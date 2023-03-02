using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;
using UnityEngine;

public class UITrainingLeaderBoardPresenter : IDisposable
{
    private readonly IDIContainer container;
    private ISocketClient socketClient;
    private UITouchDisablePresenter uiTouchDisablePresenter;
    private UITrainingLeaderBoard uiTrainingLeaderBoard;
    private CancellationTokenSource cts;
    private const int MAX_RANK = 50;
    
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();
    private UITouchDisablePresenter UITouchDisablePresenter => uiTouchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();

    public UITrainingLeaderBoardPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTaskVoid ShowLeaderBoardAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        
        await UITouchDisablePresenter.ShowTillFinishTaskAsync(InstantiateUI());
        
        var response = await SocketClient.Send<TrainingLeaderBoardRequest, TrainingLeaderBoardResponse>(new TrainingLeaderBoardRequest());
        
        uiTrainingLeaderBoard.SetEntity(new UITrainingLeaderBoard.Entity()
        {
            closeBtn = new ButtonComponent.Entity(() =>
            {
                uiTrainingLeaderBoard.Out().Forget();
            }),
            leaderBoard = response.LeaderBoardRecords.Select(x => new UITrainingLeaderBoardRecord.Entity()
                                  {
                                      rank = x.Rank,
                                      highestScore = (int)x.Score,
                                      horseName = x.PlayerName,
                                      rankContainer = UITrainingLeaderBoardRankType.RankType.Rank
                                  }).ToArray(),
            userRank = new UITrainingLeaderBoardRecord.Entity()
            {
                rank = response.CurrentPlayerRecord.Rank,
                highestScore = (int)response.CurrentPlayerRecord.Score,
                maxRank = MAX_RANK,
                horseName = response.CurrentPlayerRecord.PlayerName,
                rankContainer = response.CurrentPlayerRecord.Rank > MAX_RANK ? UITrainingLeaderBoardRankType.RankType.OutOfLeaderBoard
                    : response.CurrentPlayerRecord.Rank <= 0 ? UITrainingLeaderBoardRankType.RankType.None
                    : UITrainingLeaderBoardRankType.RankType.UserRank,
            }
        });
        
        uiTrainingLeaderBoard.In().Forget();
    }

    private async UniTask InstantiateUI()
    {
        uiTrainingLeaderBoard ??= await UILoader.Instantiate<UITrainingLeaderBoard>(token: cts.Token);
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiTrainingLeaderBoard);
    }
}
