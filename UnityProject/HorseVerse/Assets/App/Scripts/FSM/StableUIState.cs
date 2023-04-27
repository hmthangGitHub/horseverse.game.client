using Cysharp.Threading.Tasks;

public class StableUIState : InjectedBState
{
    private UIHeaderPresenter uiHeaderPresenter = default;
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private UIHorse3DViewPresenter uiHorse3DViewPresenter = default;
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();
    private StablePreviewPresenter stablePreviewPresenter;

    public override void Enter()
    {
        base.Enter();
        UIHorse3DViewPresenter.ShowHorse3DViewAsync(2, true, false, cameraType:MainMenuCameraType.CameraType.Stable).Forget();
        UIHeaderPresenter.ShowHeaderAsync(true, "STABLE").Forget();
        UIHeaderPresenter.OnBack += OnBack;

        stablePreviewPresenter = new StablePreviewPresenter(Container);
        stablePreviewPresenter.ShowAsync().Forget();
        stablePreviewPresenter.OnToHorseDetail += OnViewHorseDetail;
        stablePreviewPresenter.OnToBreeding += OnToBreeding;
    }

    private void OnBack()
    {
        OnBackAsync().Forget();
    }

    private async UniTaskVoid OnBackAsync()
    {
        this.GetMachine<StableState>().GetMachine<InitialState>().ChangeState<MainMenuState>();
    }

    private void OnViewHorseDetail()
    {
        this.Machine.ChangeState<StableHorseDetailState>();
    }
    
    private void OnToBreeding()
    {
        this.Machine.ChangeState<BreedingState>();
    }

    public override void Exit()
    {
        base.Exit();
        UIHeaderPresenter.HideHeader();
        UIHeaderPresenter.OnBack -= OnBack;
        stablePreviewPresenter.OnToHorseDetail -= OnViewHorseDetail;
        DisposeUtility.SafeDispose(ref stablePreviewPresenter);
    }
}