using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class UIQuickRacePresenter : IDisposable
{
    private const int findMatchEnergyCost = 10;
    private const int findMatchTimerInterval = 1000;
    private UIQuickMode uiQuickMode = default;
    private CancellationTokenSource cts;
    private IDIContainer container;

    public event Action OnBack = ActionUtility.EmptyAction.Instance;
    public event Action<RaceMatchData> OnFoundMatch = ActionUtility.EmptyAction<RaceMatchData>.Instance;

    private IReadOnlyUserDataRepository userDataRepository;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private IReadOnlyHorseRepository horseRepository;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private IQuickRaceDomainService quickRaceDomainService;
    private IQuickRaceDomainService QuickRaceDomainService => quickRaceDomainService ??= container.Inject<IQuickRaceDomainService>();
    private HorseDetailEntityFactory horseDetailEntityFactory;
    private HorseDetailEntityFactory HorseDetailEntityFactory => horseDetailEntityFactory ??= container.Inject<HorseDetailEntityFactory>();
    private HorseSumaryListEntityFactory horseSumaryListEntityFactory;
    private HorseSumaryListEntityFactory HorseSumaryListEntityFactory => horseSumaryListEntityFactory ??= container.Inject<HorseSumaryListEntityFactory>();
    public UIQuickRacePresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ShowUIQuickRaceAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiQuickMode = await UILoader.Instantiate<UIQuickMode>(token: cts.Token);
        await HorseRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);

        UserDataRepository.OnModelUpdate += UserDataRepositoryOnModelUpdate;

        uiQuickMode.SetEntity(new UIQuickMode.Entity()
        {
            backBtn = new ButtonComponent.Entity(OnBack),
            cancelMatchBtn = new ButtonComponent.Entity(OnCancelFindMatch),
            cancelMatchBtnVisible = new IsVisibleComponent.Entity()
            {
                isVisible = false,
            },
            findMatchBtn = new ButtonComponent.Entity(OnFindMatch),
            findMatchBtnVisible = new IsVisibleComponent.Entity()
            {
                isVisible = true
            },
            findMatchTimer = new UIComponentDuration.Entity(),
            findMatchEnergyCost = findMatchEnergyCost,
            horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(UserDataRepository.Current.MasterHorseId),
            horseSelectSumaryList = HorseSumaryListEntityFactory.InstantiateHorseSelectSumaryListEntity(),
        });
        uiQuickMode.In().Forget();
    }

    private void UserDataRepositoryOnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.before.MasterHorseId != model.after.MasterHorseId)
        {
            uiQuickMode.entity.horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(model.after.MasterHorseId);
            uiQuickMode.horseDetail.SetEntity(uiQuickMode.entity.horseDetail);
        }
    }

    private void OnFindMatch()
    {
        OnFindMatchAsync().Forget();
    }

    private async UniTaskVoid OnFindMatchAsync()
    {
        StartFindMatchTimerAsync().Forget();
        var raceMatchData = await QuickRaceDomainService.FindMatch().AttachExternalCancellation(cts.Token);
        StopFindMatchTimer();
        OnFoundMatch.Invoke(raceMatchData);
    }

    public void StopFindMatchTimer()
    {
        cts.SafeCancelAndDispose();
        cts = default;
    }

    private async UniTaskVoid StartFindMatchTimerAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiQuickMode.entity.findMatchBtnVisible.isVisible = false;
        uiQuickMode.entity.cancelMatchBtnVisible.isVisible = true;
        uiQuickMode.findMatchBtnVisible.SetEntity(false);
        uiQuickMode.cancelMatchBtnVisible.SetEntity(true);
        
        var timer = 0;
        while (true)
        {
            uiQuickMode.entity.findMatchTimer.duration = timer;
            uiQuickMode.findMatchTimer.SetEntity(uiQuickMode.entity.findMatchTimer);
            await UniTask.Delay(findMatchTimerInterval, cancellationToken: cts.Token);
            timer++;
        }
    }

    private void OnCancelFindMatch()
    {
        OnCancelFindMatchAsync().Forget();
    }

    private async UniTaskVoid OnCancelFindMatchAsync()
    {
        StopFindMatchTimer();
        await QuickRaceDomainService.CancelFindMatch();
        uiQuickMode.entity.findMatchBtnVisible.isVisible = true;
        uiQuickMode.entity.cancelMatchBtnVisible.isVisible = false;
        uiQuickMode.findMatchBtnVisible.SetEntity(true);
        uiQuickMode.cancelMatchBtnVisible.SetEntity(false);
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeRelease(ref uiQuickMode);
        UserDataRepository.OnModelUpdate -= UserDataRepositoryOnModelUpdate;
    }
}
