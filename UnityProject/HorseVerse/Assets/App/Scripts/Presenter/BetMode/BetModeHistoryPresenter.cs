using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BetModeHistoryPresenter : IDisposable
{
    private readonly IDIContainer container;
    private CancellationTokenSource cts;
    private UIBetHistory uiBetHistory;
    private UIBackGround uiBackGround;
    private UIUserBetSumary uiUserBetSummary;
    private IReadOnlyBetHistoryRepository betHistoryRepository;
    private ISocketClient socketClient;
    private IBetModeDomainService betModeDomainService;

    private IReadOnlyBetHistoryRepository BetHistoryRepository =>
        betHistoryRepository ??= container.Inject<IReadOnlyBetHistoryRepository>();

    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();

    private IBetModeDomainService BetModeDomainService =>
        betModeDomainService ??= container.Inject<BetModeDomainService>();

    public BetModeHistoryPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ShowHistoryAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiBackGround ??= await UILoader.Instantiate<UIBackGround>(UICanvas.UICanvasType.BackGround, token: cts.Token);
        uiBetHistory ??= await UILoader.Instantiate<UIBetHistory>(token: cts.Token);
        await BetHistoryRepository.LoadRepositoryIfNeedAsync()
                                  .AttachExternalCancellation(cancellationToken: cts.Token);
        uiBetHistory.SetEntity(new UIBetHistory.Entity()
        {
            historyContainer = BetHistoryRepository.Models.Values.Select(x =>
                                                   {
                                                       var firstHorse = new UIComponentHorseInfoHistory.Entity()
                                                       {
                                                           horseIndex = x.FirstHorseIndex - 1,
                                                           horseName = x.FirstHorseName,
                                                           horseRank = 1,
                                                       };
                                                       var secondHorse = new UIComponentHorseInfoHistory.Entity()
                                                       {
                                                           horseIndex = x.SecondHorseIndex - 1,
                                                           horseName = x.SecondHorseName,
                                                           horseRank = 2,
                                                       };
                                                       return new UIBetHistoryRecord.Entity()
                                                       {
                                                           matchId = x.MatchId,
                                                           time = x.TimeStamp,
                                                           firstHorse = firstHorse,
                                                           secondHorse = secondHorse,
                                                           viewRaceScriptBtn = new ButtonComponent.Entity()
                                                           {
                                                               isInteractable = false
                                                           },
                                                           viewBetDetailBtn = new ButtonComponent.Entity(() =>
                                                               OnViewBetDetailAsync(x.MatchId, 
                                                                       x.TimeStamp, 
                                                                       firstHorse,
                                                                       secondHorse)
                                                                   .Forget()),
                                                           viewResultBtn = new ButtonComponent.Entity(() => OnViewBetResultAsync().Forget())
                                                       };
                                                   })
                                                   .ToArray()
        });
        uiBackGround.SetEntity(new UIBackGround.Entity());
        await uiBackGround.In();
        await uiBetHistory.In();
    }

    private async UniTaskVoid OnViewBetResultAsync()
    {
    }

    private async UniTaskVoid OnViewBetDetailAsync(long matchId,
                                                   long time,
                                                   UIComponentHorseInfoHistory.Entity firstHorse,
                                                   UIComponentHorseInfoHistory.Entity secondHorse)
    {
        var userBets = await BetModeDomainService.GetCurrentBetMatchRawData(matchId).AttachExternalCancellation(cancellationToken: cts.Token);
        await uiBetHistory.Out();
        uiUserBetSummary ??= await UILoader.Instantiate<UIUserBetSumary>(token: cts.Token);
        uiUserBetSummary.SetEntity(new UIUserBetSumary.Entity()
        {
            time = time,
            matchId = matchId,
            firstHorse = firstHorse,
            secondHorse = secondHorse,
            userBetRecordList = userBets.Record.Select(x => new UIComponentBetModeMyResult.Entity()
            {
                rate = x.rate,
                result = x.winMoney,
                spend = x.betMoney,
                isDoubleBet = x.doubleBet,
                horseNumberFirst = x.pool_1,
                horseNumberSecond = x.pool_2,
            }).ToArray(),
            closeBtn = new ButtonComponent.Entity(UniTask.Action(async() =>
            {
                await uiUserBetSummary.Out();
                await uiBetHistory.In();
            }))
        });
        
        await uiUserBetSummary.In().AttachExternalCancellation(cts.Token);
    }

    public async UniTask Out()
    {
        await uiBetHistory.Out()
                          .AttachExternalCancellation(cts.Token);
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiBetHistory);
        UILoader.SafeRelease(ref uiUserBetSummary);
        UILoader.SafeRelease(ref uiBackGround);
    }
}