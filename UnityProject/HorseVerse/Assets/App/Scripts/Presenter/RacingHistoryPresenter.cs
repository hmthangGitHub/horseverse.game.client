using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RacingHistoryPresenter : IDisposable
{
    private readonly IDIContainer container;
    private UIRacingHistory uiRacingHistory;
    private CancellationTokenSource cts;
    private IReadOnlyRacingHistoryRepository racingHistoryRepository;
    private IReadOnlyRacingHistoryRepository RacingHistoryRepository => racingHistoryRepository ??= container.Inject<IReadOnlyRacingHistoryRepository>();

    public RacingHistoryPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTaskVoid ShowHistoryAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await RacingHistoryRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);
        uiRacingHistory ??= await UILoader.Instantiate<UIRacingHistory>(token: this.cts.Token);
        uiRacingHistory.SetEntity(new UIRacingHistory.Entity()
        {
            historyContainer = new UIComponentHistoryRecordList.Entity()
            {
                entities = RacingHistoryRepository.Models.Values.Select(x => new UIComponentHistoryRecord.Entity()
                {
                    time = x.TimeStamp,
                    chestNumber = x.ChestRewardNumber,
                    coinNumber = x.CoinRewardNumber,
                    horseIndex = x.HorseIndex,
                    horseRank = new UIComponentHistoryRecordHorseRank.Entity()
                    {
                        rank = x.Rank
                    },
                    matchId = x.MatchId,
                    viewResultBtn = new ButtonComponent.Entity(() =>
                    {
                        
                    }),
                    viewRaceScriptBtn = new ButtonComponent.Entity(() =>
                    {
                        
                    })
                }).ToArray()
            }
        });

        uiRacingHistory.In().Forget();
    }
    
    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiRacingHistory);
    }
}
