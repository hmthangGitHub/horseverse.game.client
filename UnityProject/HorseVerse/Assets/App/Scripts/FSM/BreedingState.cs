using Cysharp.Threading.Tasks;

public class BreedingState : InjectedBHState
{
    private UIBackGroundPresenter uiBackGroundPresenter;
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();
    public override void AddStates()
    {
        base.AddStates();
        AddState<BreedingSlotState>();
        AddState<BreedingPreviewState>();
        AddState<BreedingFinishState>();
        SetInitialState<BreedingSlotState>();
    }

    public override void Enter()
    {
        Container.Bind(new BreedingStateContext());
        Container.Bind(new BreedSlotRepository(Container));
        Container.Bind(new BreedingBreedingDomainService(Container));
        UIBackGroundPresenter.ShowBackGroundAsync().Forget();
        UIHorse3DViewPresenter.HideHorse3DViewAsync().Forget();
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        
        UIBackGroundPresenter.HideBackground().Forget();
        UIHorse3DViewPresenter.ShowHorse3DViewAsync(2, true, false, MainMenuCameraType.CameraType.Stable).Forget();
        Container.RemoveAndDisposeIfNeed<BreedingStateContext>();
        Container.RemoveAndDisposeIfNeed<BreedSlotRepository>();
        Container.RemoveAndDisposeIfNeed<BreedingBreedingDomainService>();
    }
}