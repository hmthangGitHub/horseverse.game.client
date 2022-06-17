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

    public event Action OnBack = EmptyAction.Instance;
    public event Action OnFoundMatch = EmptyAction.Instance;

    private IReadOnlyUserDataRepository userDataRepository;
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private IReadOnlyHorseRepository horseRepository;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private MasterHorseContainer masterHorseContainer;
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private IQuickRaceDomainService quickRaceDomainService;
    private IQuickRaceDomainService QuickRaceDomainService => quickRaceDomainService ??= container.Inject<IQuickRaceDomainService>();

    public UIQuickRacePresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ShowUIQuickRaceAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiQuickMode = await UILoader.Load<UIQuickMode>(token: cts.Token);
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
            horseDetail = GetHorseDetailEntity(),
            horseSelectSumaryList = new UIComponentTraningHorseSelectSumaryList.Entity()
            {
                entities = HorseRepository.Models.Select(x => new UIComponentTraningHorseSelectSumary.Entity()
                {
                    horseName = MasterHorseContainer.MasterHorseIndexer[x.Key].Name,
                    selectBtn = new ButtonComponent.Entity(() => OnSelectHorse(x.Key))
                }).ToArray()
            }
        });
        uiQuickMode.In().Forget();
    }

    private UIComponentHorseDetail.Entity GetHorseDetailEntity()
    {
        var userHorse = HorseRepository.Models[UserDataRepository.Current.MasterHorseId];
        var masterHorse = MasterHorseContainer.MasterHorseIndexer[UserDataRepository.Current.MasterHorseId];
        return new UIComponentHorseDetail.Entity()
        {
            earning = userHorse.Earning,
            horseName = masterHorse.Name,
            powerProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
            {
                bonus = userHorse.PowerBonus,
                progressBar = new UIComponentProgressBar.Entity()
                {
                    progress = userHorse.PowerRatio
                }
            },
            speedProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
            {
                bonus = userHorse.SpeedBonus,
                progressBar = new UIComponentProgressBar.Entity()
                {
                    progress = userHorse.SpeedRatio
                }
            },
            technicallyProgressBarWithBonus = new UIComponentProgressBarWithBonus.Entity()
            {
                bonus = userHorse.TechnicallyBonus,
                progressBar = new UIComponentProgressBar.Entity()
                {
                    progress = userHorse.TechnicallyRatio
                }
            },
        };
    }

    private void UserDataRepositoryOnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.before.MasterHorseId != model.after.MasterHorseId)
        {
            uiQuickMode.entity.horseDetail = GetHorseDetailEntity();
            uiQuickMode.horseDetail.SetEntity(uiQuickMode.entity.horseDetail);
        }
    }

    private void OnSelectHorse(long masterHorseId)
    {
        QuickRaceDomainService.ChangeHorse(masterHorseId).Forget();
    }

    private void OnFindMatch()
    {
        OnFindMatchAsync().Forget();
    }

    private async UniTaskVoid OnFindMatchAsync()
    {
        StartFindMatchTimerAsync().Forget();
        await QuickRaceDomainService.FindMatch().AttachExternalCancellation(cts.Token);
        StopFindMatchTimer();
        OnFoundMatch.Invoke();
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
        UILoader.SafeUnload(ref uiQuickMode);
        UserDataRepository.OnModelUpdate -= UserDataRepositoryOnModelUpdate;
    }
}
