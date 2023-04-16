#define DEVELOPMENT
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using io.hverse.game.protogen;

public partial class UIRacePresenter : IDisposable
{
    private UIQuickMode uiQuickMode = default;
    private CancellationTokenSource cts;
    private readonly IDIContainer container;
    private readonly List<IDisposable> disposableList = new List<IDisposable>();
    private IReadOnlyUserDataRepository userDataRepository;
    private IReadOnlyHorseRepository horseRepository;
    private IQuickRaceDomainService quickRaceDomainService;
    private HorseDetailEntityFactory horseDetailEntityFactory;
    private HorseSumaryListEntityFactory horseSumaryListEntityFactory;
    private HorseRaceContext horseRaceContext;
    public event Action OnFindMatch = ActionUtility.EmptyAction.Instance;

    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
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
            findMatchBtn = new ButtonComponent.Entity(OnFindMatch)
            {
                isInteractable = UserDataRepository.Current.DailyRacingNumberLeft > 0
            },
            findMatchBtnVisible = new IsVisibleComponent.Entity()
            {
                isVisible = true
            },
            findMatchTimer = new UIComponentDuration.Entity(),
            findMatchLimit = UserSettingLocalRepository.MasterDataModel.MaxDailyRacingNumber,
            currentFindMatchLeft = UserDataRepository.Current.DailyRacingNumberLeft,
            horseDetail = HorseDetailEntityFactory.InstantiateHorseDetailEntity(UserDataRepository.Current.CurrentHorseNftId),
            horseSelectSumaryList = HorseSumaryListEntityFactory.InstantiateHorseSelectSumaryListEntity(),
            raceRoomInfo = new UIComponentRaceRoomInfo.Entity()
            {
                rarityRequirement = GetRankRequirement(),
                rewardGroup1st = UIComponentRaceRewardGroupFactory.CreateRewardGroup(1, HorseRaceContext.RaceMatchDataContext.RacingRoomType),
                rewardGroup2nd = UIComponentRaceRewardGroupFactory.CreateRewardGroup(2, HorseRaceContext.RaceMatchDataContext.RacingRoomType),
                rewardGroup3rd = UIComponentRaceRewardGroupFactory.CreateRewardGroup(3, HorseRaceContext.RaceMatchDataContext.RacingRoomType),
            }
        });
        uiQuickMode.In().Forget();
    }

    private UIComponentHorseRankRequirement.Rarity GetRankRequirement()
    {
        return HorseRaceContext.RaceMatchDataContext.RacingRoomType switch
        {
            RacingRoomType.Novice => UIComponentHorseRankRequirement.Rarity.Common,
            RacingRoomType.Basic => UIComponentHorseRankRequirement.Rarity.Uncommon,
            RacingRoomType.Advance => UIComponentHorseRankRequirement.Rarity.Rare,
            RacingRoomType.Expert => UIComponentHorseRankRequirement.Rarity.Epic,
            RacingRoomType.Master => UIComponentHorseRankRequirement.Rarity.Legend,
            _ => UIComponentHorseRankRequirement.Rarity.Uncommon
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
}