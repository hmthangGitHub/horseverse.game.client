using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class BreedingFinishState : InjectedBState
{
    private BreedingFinishPresenter presenter;
    public override void Enter()
    {
        base.Enter();
        presenter = new BreedingFinishPresenter(Container);
        presenter.FinishBreedingAsync().Forget();
    }

    public override void Exit()
    {
        base.Exit();
        DisposeUtility.SafeDispose(ref presenter);
    }
}

internal class BreedingFinishPresenter : IDisposable
{
    private readonly IDIContainer container;
    private UIHorseStableBreedingComplete uiHorseStableBreedingComplete;
    private CancellationTokenSource cts;
    private IBreedingDomainService breedingDomainService;
    private IReadOnlyBreedSlotRepository breedSlotRepository;
    private HorseRepository horseRepository;
    private BreedingStateContext breedingStateContext;
    private IBreedingDomainService BreedingDomainService => breedingDomainService ??= container.Inject<IBreedingDomainService>();
    private IReadOnlyBreedSlotRepository BreedSlotRepository => breedSlotRepository ??= container.Inject<IReadOnlyBreedSlotRepository>();
    private BreedingStateContext BreedingStateContext => breedingStateContext ??= container.Inject<BreedingStateContext>();
    private HorseRepository HorseRepository => horseRepository ??= container.Inject<HorseRepository>();
    
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
            }
        });
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
    }
}