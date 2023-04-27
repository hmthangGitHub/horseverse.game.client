using Cysharp.Threading.Tasks;
using RobustFSM.Interfaces;

public class BreedingSlotState : InjectedBState
{
    private BreedingSlotPresenter presenter;
    private BreedingStateContext breedingStateContext;
    private BreedingStateContext BreedingStateContext => breedingStateContext ??= Container.Inject<BreedingStateContext>();
    private UIHeaderPresenter uiHeaderPresenter;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    
    public override void Enter()
    {
        base.Enter();
        presenter = new BreedingSlotPresenter(Container);
        presenter.OnEnterBreedingSlotEvent += OnEnterBreedingSlotEvent;
        presenter.FinishBreedingSlotEvent += FinishBreedingSlotEvent;
        presenter.ShowBreedingSlotAsync().Forget();
        UIHeaderPresenter.ShowHeaderAsync(true, "BREEDING").Forget();
        UIHeaderPresenter.OnBack += OnBack;
    }

    private void OnBack()
    {
        ((IState)Machine).Machine.ChangeState<StableUIState>();
    }

    private void OnEnterBreedingSlotEvent(int slotIndex)
    {
        BreedingStateContext.BreedingSlotIndex = slotIndex;
        Machine.ChangeState<BreedingPreviewState>();
    }
    
    private void FinishBreedingSlotEvent(int slotIndex)
    {
        BreedingStateContext.BreedingSlotIndex = slotIndex;
        Machine.ChangeState<BreedingFinishState>();
    }

    public override void Exit()
    {
        base.Exit();
        UIHeaderPresenter.OnBack -= OnBack;
        presenter.OnEnterBreedingSlotEvent -= OnEnterBreedingSlotEvent;
        presenter.FinishBreedingSlotEvent -= FinishBreedingSlotEvent;
        breedingStateContext = default;
        
        DisposeUtility.SafeDispose(ref presenter);
    }
}