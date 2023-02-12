#define DEVELOPMENT
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public partial class UIRacePresenter : IDisposable
{
    private const int FindMatchEnergyCost = 10;
    private const int FindMatchTimerInterval = 1000;
    
    private UIQuickMode uiQuickMode = default;
    private CancellationTokenSource cts;
    private readonly IDIContainer container;
    private readonly List<IDisposable> disposableList = new List<IDisposable>();
    public event Action<RaceScriptData> OnFoundMatch = ActionUtility.EmptyAction<RaceScriptData>.Instance;
    private IReadOnlyUserDataRepository userDataRepository;
    private IReadOnlyHorseRepository horseRepository;
    private MasterHorseContainer masterHorseContainer;
    private IQuickRaceDomainService quickRaceDomainService;
    private HorseDetailEntityFactory horseDetailEntityFactory;
    private HorseSumaryListEntityFactory horseSumaryListEntityFactory;
    private HorseRaceContext horseRaceContext;
    public event Action OnFindMatch = ActionUtility.EmptyAction.Instance;

    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private IQuickRaceDomainService QuickRaceDomainService => quickRaceDomainService ??= container.Inject<IQuickRaceDomainService>();
    private HorseDetailEntityFactory HorseDetailEntityFactory => horseDetailEntityFactory ??= container.Inject<HorseDetailEntityFactory>();
    private HorseSumaryListEntityFactory HorseSumaryListEntityFactory => horseSumaryListEntityFactory ??= container.Inject<HorseSumaryListEntityFactory>();
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= container.Inject<HorseRaceContext>();
    
    public UIRacePresenter(IDIContainer container)
    {
        this.container = container;
    }


    private long currentSelectHorseId = -1;

    public async UniTask ShowUIQuickRaceAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiQuickMode = await UILoader.Instantiate<UIQuickMode>(token: cts.Token);
        await HorseRepository.LoadRepositoryIfNeedAsync().AttachExternalCancellation(cts.Token);

        UserDataRepository.OnModelUpdate += UserDataRepositoryOnModelUpdate;
        currentSelectHorseId = UserDataRepository.Current.CurrentHorseNftId;
        uiQuickMode.SetEntity(new UIQuickMode.Entity()
        {
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
            findMatchLimit = FindMatchEnergyCost,
            currentFindMatchLeft = FindMatchEnergyCost,
            horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(UserDataRepository.Current.CurrentHorseNftId),
            horseSelectSumaryList = HorseSumaryListEntityFactory.InstantiateHorseSelectSumaryListEntity(),
            raceRoomInfo = new UIComponentRaceRoomInfo.Entity()
            {
                horseRankRequirement = GetRankRequirement(),
                rewardGroup1st = new UIComponentRaceRewardGroup.Entity()
                {
                    chestNumber = 3,
                    coinNumber = 100
                },
                rewardGroup2nd = new UIComponentRaceRewardGroup.Entity()
                {
                    chestNumber = 3,
                    coinNumber = 200
                },
                rewardGroup3rd = new UIComponentRaceRewardGroup.Entity()
                {
                    chestNumber = 3,
                    coinNumber = 300
                },
            }
        });
        uiQuickMode.In().Forget();
    }

    private UIComponentHorseRankRequirement.HorseRank GetRankRequirement()
    {
        return HorseRaceContext.RaceMatchDataContext.TraditionalRoomMasteryType switch
        {
            TraditionalRoomMasteryType.Novice => UIComponentHorseRankRequirement.HorseRank.Uncommon,
            TraditionalRoomMasteryType.Basic => UIComponentHorseRankRequirement.HorseRank.Common,
            TraditionalRoomMasteryType.Advance => UIComponentHorseRankRequirement.HorseRank.Rare,
            TraditionalRoomMasteryType.Expert => UIComponentHorseRankRequirement.HorseRank.Epic,
            TraditionalRoomMasteryType.Master => UIComponentHorseRankRequirement.HorseRank.Legend,
            _ => UIComponentHorseRankRequirement.HorseRank.Uncommon
        };
    }

    private void UserDataRepositoryOnModelUpdate((UserDataModel before, UserDataModel after) model)
    {
        if (model.before.CurrentHorseNftId != model.after.CurrentHorseNftId)
        {
            uiQuickMode.SetHorseDetailEntity(HorseDetailEntityFactory.InstantiateHorseDetailEntity(model.after.CurrentHorseNftId));
            OnSelectHorse(model.after.CurrentHorseNftId);
        }
    }

    private void StopFindMatchTimer()
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
            await UniTask.Delay(FindMatchTimerInterval, cancellationToken: cts.Token);
            timer++;
        }
    }

    private void OnCancelFindMatch()
    {
        OnCancelFindMatchAsync().Forget();
    }

    private async UniTaskVoid OnCancelFindMatchAsync()
    {
        await QuickRaceDomainService.CancelFindMatch(UserDataRepository.Current.CurrentHorseNftId);
        StopFindMatchTimer();
        uiQuickMode.entity.findMatchBtnVisible.isVisible = true;
        uiQuickMode.entity.cancelMatchBtnVisible.isVisible = false;
        uiQuickMode.findMatchBtnVisible.SetEntity(true);
        uiQuickMode.cancelMatchBtnVisible.SetEntity(false);
    }

    public void Dispose()
    {
#if DEVELOPMENT
        DebugCleanUp();
#endif
        cts.SafeCancelAndDispose();
        cts = default;
        
        UILoader.SafeRelease(ref uiQuickMode);
        UserDataRepository.OnModelUpdate -= UserDataRepositoryOnModelUpdate;
        
        disposableList.ForEach(x => x.Dispose());
        disposableList.Clear();
    }

    private void OnSelectHorse(long nftId)
    {
        if (currentSelectHorseId == nftId) return;
        var l = uiQuickMode.horseSelectSumaryList.instanceList;
        if (currentSelectHorseId > -1)
        {
            var old = l.FirstOrDefault(o => o.entity.horseNFTId == currentSelectHorseId);
            if (old != null)
            {
                old.selectBtn.SetSelected(false);
            }
            currentSelectHorseId = -1;
        }
        var current = l.FirstOrDefault(o => o.entity.horseNFTId == nftId);
        if (current != default)
        {
            current.selectBtn.SetSelected(true);
            currentSelectHorseId = nftId;
        }

    }
}