using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;

internal class BreedingFinishPresenter : IDisposable
{
    private readonly IDIContainer container;
    private UIHorseStableBreedingComplete uiHorseStableBreedingComplete;
    private CancellationTokenSource cts;
    private IBreedingDomainService breedingDomainService;
    private IReadOnlyBreedSlotRepository breedSlotRepository;
    private IReadOnlyHorseRepository horseRepository;
    private BreedingStateContext breedingStateContext;
    private MasterHorseContainer masterHorseContainer;
    private IBreedingDomainService BreedingDomainService => breedingDomainService ??= container.Inject<IBreedingDomainService>();
    private IReadOnlyBreedSlotRepository BreedSlotRepository => breedSlotRepository ??= container.Inject<IReadOnlyBreedSlotRepository>();
    private BreedingStateContext BreedingStateContext => breedingStateContext ??= container.Inject<BreedingStateContext>();
    private IReadOnlyHorseRepository HorseRepository => horseRepository ??= container.Inject<IReadOnlyHorseRepository>();
    private MasterHorseContainer MasterHorseContainer => masterHorseContainer ??= container.Inject<MasterHorseContainer>();
    
    public BreedingFinishPresenter(IDIContainer container)
    {
        this.container = container;
        cts = new CancellationTokenSource();
    }

    public async UniTaskVoid FinishBreedingAsync()
    {
        var horseId = BreedSlotRepository.Models[BreedingStateContext.BreedingSlotIndex].ChildHorse.Id;
        await BreedingDomainService.FinishBreeding(BreedingStateContext.BreedingSlotIndex);
        
        uiHorseStableBreedingComplete ??= await UILoader.Instantiate<UIHorseStableBreedingComplete>(token: cts.Token);
        var horseDataModel = HorseRepository.Models[horseId];
        uiHorseStableBreedingComplete.SetEntity(new UIHorseStableBreedingComplete.Entity()
        {
            briefInfo = new UIHorseStableBriefInfo.Entity()
            {
                age = horseDataModel.HorseBasic.Age,
                element = (UIComponentHorseElement.Element)horseDataModel.HorseType,
                rarity = (UIComponentHorseRankRequirement.Rarity)horseDataModel.HorseBasic.Rarity,
                sex = (UIHorseSexInfo.Sex)horseDataModel.HorseBasic.Sex,
                horseName = horseDataModel.HorseBasic.Name,
                showChangeHorseBtn = false
            },
            horseLoader = new HorseLoader.Entity()
            {
                horse = MasterHorseContainer.MasterHorseIndexer[HorseRepository.Models[horseId].HorseMasterId].ModelPath
            },
            currentSlot = BreedSlotRepository.Models.Values.Count(x => x.Status == BreedSlotStatus.Available),
            maxSlot = BreedSlotRepository.Models.Count
        });
        uiHorseStableBreedingComplete.In().Forget();
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiHorseStableBreedingComplete);
    }
}