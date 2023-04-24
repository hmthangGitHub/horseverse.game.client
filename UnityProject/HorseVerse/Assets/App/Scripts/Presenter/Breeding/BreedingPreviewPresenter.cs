using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;

internal class BreedingPreviewPresenter : IDisposable
{
    private readonly IDIContainer container;
    private CancellationTokenSource cts;
    private UIHorseStableBreedingPreview uiHorseStableBreedingPreview;
    private UIHeaderPresenter uiHeaderPresenter;
    private IReadOnlyHorseRepository horseRepository;
    private CancellationTokenSource selectHorseCts;
    private MasterHorseContainer masterHorseContainer;
    private IBreedingDomainService breedingDomainService;
    private BreedingStateContext breedingStateContext;
    private IReadOnlyUserDataRepository userDataRepository;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= container.Inject<UIHeaderPresenter>();
    private IBreedingDomainService BreedingDomainService => breedingDomainService ??= container.Inject<IBreedingDomainService>();
    public event Action OnBack = ActionUtility.EmptyAction.Instance;
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private IReadOnlyUserDataRepository UserDataRepository => userDataRepository ??= container.Inject<IReadOnlyUserDataRepository>();
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    private BreedingStateContext BreedingStateContext => breedingStateContext ??= container.Inject<BreedingStateContext>();
    private readonly Dictionary<HorseSex, long> currentSelectingHorse = new Dictionary<HorseSex, long>()
    {
        { HorseSex.Female, -1 },
        { HorseSex.Male, -1 }
    };



    private enum BreedingState
    {
        Preview,
        SelectHorse
    }

    public BreedingPreviewPresenter(IDIContainer container)
    {
        this.container = container;
        cts = new CancellationTokenSource();
    }

    public async UniTaskVoid ShowHorseBreedingPreviewAsync()
    {
        SetHeaderAndCallBack(BreedingState.Preview, default);
        uiHorseStableBreedingPreview ??= await UILoader.Instantiate<UIHorseStableBreedingPreview>(token:cts.Token);
        uiHorseStableBreedingPreview.SetEntity(new UIHorseStableBreedingPreview.Entity()
        {
            speed = 10,
            stamina = 10,
            sprintSpeed = 10,
            commonOdd = 75,
            uncommonOdd = 55,
            rareOdd = 35,
            epicOdd = 15,
            legendOdd = 5,
            maleBreedingHorse = new UIHorseBreedingCandidate.Entity()
            {
                isShowBriefInfo = false,
                chooseHorseBtn = new ButtonComponent.Entity(() => SelectHorseAsync(HorseSex.Male).Forget())
            },
            femaleBreedingHorse = new UIHorseBreedingCandidate.Entity()
            {
                isShowBriefInfo = false,
                chooseHorseBtn = new ButtonComponent.Entity(() => SelectHorseAsync(HorseSex.Female).Forget())
            },
            breedingBtn = CreateBreedingBtnEntity()
        });
        
        uiHorseStableBreedingPreview.In().Forget();
    }

    private ButtonComponent.Entity CreateBreedingBtnEntity()
    {
        if (currentSelectingHorse[HorseSex.Male] > 0 && currentSelectingHorse[HorseSex.Female] > 0)
        {
            var breedingCost = GetBreedingCost();
            var isEnoughChipToBreeding = UserDataRepository.Current.Coin >= breedingCost;
            if (isEnoughChipToBreeding)
            {
                return new ButtonComponent.Entity(UniTask.Action(async () =>
                {
                    await BreedingDomainService.Breed(BreedingStateContext.BreedingSlotIndex, 
                        currentSelectingHorse[HorseSex.Male], 
                        currentSelectingHorse[HorseSex.Female]);
                    OnBack.Invoke();
                }));
            }
        }
        return new ButtonComponent.Entity(ActionUtility.EmptyAction.Instance, false);
    }

    private int GetBreedingCost()
    {
        var maleRarity = (int)HorseRepository.Models[currentSelectingHorse[HorseSex.Male]].Rarity;
        var femaleRarity = (int)HorseRepository.Models[currentSelectingHorse[HorseSex.Female]].Rarity;
        var breedingCost = UserSettingLocalRepository
                           .MasterDataModel
                           .BreedingFees.First(x => x.MaleRarity == maleRarity && x.FemaleRarity == femaleRarity)
                           .Fee;
        return breedingCost;
    }

    private async UniTaskVoid SelectHorseAsync(HorseSex horseSex)
    {
        SetHeaderAndCallBack(BreedingState.SelectHorse, horseSex);
        selectHorseCts.SafeCancelAndDispose();
        selectHorseCts = new CancellationTokenSource();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(selectHorseCts.Token, cts.Token);
        using var breedingSelectorPresenter = new HorseBreedingSelectorPresenter(container);
        
        await uiHorseStableBreedingPreview.Out();
        var horseId = await breedingSelectorPresenter.SelectHorseAsync(horseSex, linkedCts.Token);
        currentSelectingHorse[horseSex] = horseId;
        await uiHorseStableBreedingPreview.In();
        SetHeaderAndCallBack(BreedingState.Preview, default);
        UpdateBreedingBtn();
        SetSelectHorseEntity(horseId, horseSex);
    }

    private void UpdateBreedingBtn()
    {
        uiHorseStableBreedingPreview.breedingBtn.SetEntity(CreateBreedingBtnEntity());
        if (currentSelectingHorse[HorseSex.Male] > 0 && currentSelectingHorse[HorseSex.Female] > 0)
        {
            uiHorseStableBreedingPreview.rubyNeedToBreed.SetEntity(GetBreedingCost());
        }
        else
        {
            uiHorseStableBreedingPreview.rubyNeedToBreed.SetEntity(0);
        }
    }

    private void SetSelectHorseEntity(long horseId,
                                      HorseSex horseSex)
    {
        var candidate = horseSex switch
        {
            HorseSex.Male => uiHorseStableBreedingPreview.maleBreedingHorse,
            HorseSex.Female => uiHorseStableBreedingPreview.femaleBreedingHorse,
            _ => throw new ArgumentOutOfRangeException(nameof(horseSex), horseSex, null)
        };

        var horseModel = HorseRepository.Models[horseId];
        candidate.SetEntity(new UIHorseBreedingCandidate.Entity()
        {
            isShowBriefInfo = true,
            breedCount = horseModel.HorseRising.BreedingCount,
            maxBreedCount = UserSettingLocalRepository.MasterDataModel.MaxBreedingNumber,
            briefInfo = new UIHorseStableBriefInfo.Entity()
            {
                age = horseModel.HorseBasic.Age,
                element = (UIComponentHorseElement.Element)horseModel.HorseType,
                rarity = (UIComponentHorseRankRequirement.Rarity)horseModel.HorseBasic.Rarity,
                sex = (UIHorseSexInfo.Sex)horseModel.HorseBasic.Sex,
                horseName = horseModel.HorseBasic.Name,
                leftBtn = new ButtonComponent.Entity(() => QuickSelectHorseAsync(-1, horseModel.HorseNtfId, horseSex)),
                rightBtn = new ButtonComponent.Entity(() => QuickSelectHorseAsync(1, horseModel.HorseNtfId, horseSex)),
                showChangeHorseBtn = true
            },
            horseLoader = new HorseLoader.Entity()
            {
                horse = MasterHorseContainer.MasterHorseIndexer[(long)horseModel.HorseMasterId]
                                            .ModelPath
            },
            chooseHorseBtn = new ButtonComponent.Entity(() => SelectHorseAsync(horseSex).Forget()),
        });
    }

    private void QuickSelectHorseAsync(int direction, long currentHorseId, HorseSex horseSex)
    {
        var allHorseWithSex = HorseRepository
                                .Models
                                .Values
                                .Where(x => x.HorseBasic.Sex == horseSex)
                                .ToList();
        
        var currentIndex = allHorseWithSex.FindIndex(x => x.HorseBasic.Id == currentHorseId);
        currentIndex += direction + allHorseWithSex.Count;
        currentIndex %= allHorseWithSex.Count;

        SetSelectHorseEntity(allHorseWithSex[currentIndex].HorseNtfId, horseSex); ;
    }

    private void SetHeaderAndCallBack(BreedingState state, HorseSex horseSex)
    {
        UIHeaderPresenter.OnBack -= OnBackToBreedSlot;
        UIHeaderPresenter.OnBack -= BackToPreview;
        
        switch(state)
        {
            case BreedingState.Preview:
                UIHeaderPresenter.ShowHeaderAsync(title: "BREEDING").Forget();
                UIHeaderPresenter.OnBack += OnBackToBreedSlot;
                break;
            case BreedingState.SelectHorse:
                UIHeaderPresenter.ShowHeaderAsync(title: horseSex == HorseSex.Female 
                    ? "MARE"
                    : "STALLION").Forget();
                UIHeaderPresenter.OnBack += BackToPreview;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void OnBackToBreedSlot()
    {
        OnBack();
    }

    private void BackToPreview()
    {
        DisposeUtility.SafeDispose(ref selectHorseCts);
        SetHeaderAndCallBack(BreedingState.Preview, default);
        ShowHorseBreedingPreviewAsync().Forget();
    }

    public void Dispose()
    {
        currentSelectingHorse.Clear();
        UIHeaderPresenter.OnBack -= BackToPreview;
        UIHeaderPresenter.OnBack -= OnBackToBreedSlot;
        
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiHorseStableBreedingPreview);
        
        horseRepository = default;
        masterHorseContainer = default;
        breedingDomainService = default;
        breedingStateContext = default;
        userDataRepository = default;
    }
}