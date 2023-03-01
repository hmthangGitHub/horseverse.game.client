using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BetModeHistoryPresenter : IDisposable
{
    private readonly IDIContainer container;
    private CancellationTokenSource cts;
    private UIBetHistory uiBetHistory;
    private UIBackGround uiBackGround;
    private UIBetModeResult uiBetModeResult;
    private IReadOnlyBetHistoryRepository betHistoryRepository;
    private ISocketClient socketClient;
    private IBetModeDomainService betModeDomainService;
    private UITouchDisablePresenter uiTouchDisablePresenter;
    private UserBetSummaryPresenter userBetSummaryPresenter;
    private UITouchDisablePresenter UITouchDisablePresenter => uiTouchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();

    private IReadOnlyBetHistoryRepository BetHistoryRepository =>
        betHistoryRepository ??= container.Inject<IReadOnlyBetHistoryRepository>();

    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();

    private IBetModeDomainService BetModeDomainService =>
        betModeDomainService ??= container.Inject<BetModeDomainService>();

    public BetModeHistoryPresenter(IDIContainer container)
    {
        this.container = container;
        userBetSummaryPresenter = new UserBetSummaryPresenter(container);
    }

    public async UniTask ShowHistoryAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();

        await UITouchDisablePresenter.ShowTillFinishTaskAsync(InstantiateUIAsync());
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
                                                           viewBetDetailBtn = new ButtonComponent.Entity(() => userBetSummaryPresenter.ShowUserBetSummaryAsync(x.MatchId,
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

    private async UniTask InstantiateUIAsync()
    {
        uiBackGround ??= await UILoader.Instantiate<UIBackGround>(token: cts.Token);
        uiBetHistory ??= await UILoader.Instantiate<UIBetHistory>(token: cts.Token);
        uiBetModeResult ??= await UILoader.Instantiate<UIBetModeResult>(UICanvas.UICanvasType.PopUp, token: cts.Token);
    }

    private async UniTaskVoid OnViewBetResultAsync()
    {
        uiBetModeResult.SetEntity(new UIBetModeResult.Entity()
        {
            betModeResultPanel = new UIBetModeResultPanel.Entity()
            {
                betModeResultList = Enumerable.Range(0, 8)
                                              .Select(x => new UIComponentBetModeResult.Entity()
                                              {
                                                  no = x + 1,
                                                  time = UnityEngine.Random.Range(40.0f, 50.0f),
                                                  horseName = "Horse Name" + x,
                                                  horseNumber = UnityEngine.Random.Range(0, 8),
                                                  rewardGroupVisible = false,
                                                  isSelfHorse = false
                                              }).ToArray(),
                
            },
            nextBtn = new ButtonComponent.Entity(() => uiBetModeResult.Out().Forget())
        });

        uiBetModeResult.In().Forget();
    }

    public async UniTask Out()
    {
        await uiBetHistory.Out();
        await uiBackGround.Out();
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiBetHistory);
        UILoader.SafeRelease(ref uiBackGround);
        UILoader.SafeRelease(ref uiBetModeResult);
        DisposeUtility.SafeDispose(ref userBetSummaryPresenter);
    }
}