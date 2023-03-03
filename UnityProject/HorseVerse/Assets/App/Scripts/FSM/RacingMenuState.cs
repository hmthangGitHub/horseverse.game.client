using Cysharp.Threading.Tasks;

public class RacingMenuState : InjectedBHState
{
    private UIRacePresenter uiRacePresenter;
    private UIHeaderPresenter uiHeaderPresenter;
    private HorseRaceContext horseRaceContext;
    private UIBackGroundPresenter uiBackGroundPresenter;
    private UIHorse3DViewPresenter uiHorse3DViewPresenter ;
    
    private UIHeaderPresenter UIHeaderPresenter => uiHeaderPresenter ??= Container.Inject<UIHeaderPresenter>();
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    private UIBackGroundPresenter UIBackGroundPresenter => uiBackGroundPresenter ??= Container.Inject<UIBackGroundPresenter>();
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= Container.Inject<UIHorse3DViewPresenter>();


    public override void AddStates()
    {
        base.AddStates();
        AddState<EmptyState>();
        AddState<RacingMatchFindingState>();
        SetInitialState<EmptyState>();
    }

    public override void Enter()
    {
        base.Enter();
        OnEnterStateAsync().Forget();
    }

    private async UniTask OnEnterStateAsync()
    {
        await UIHorse3DViewPresenter.ShowHorse3DViewAsync(1);
        UIBackGroundPresenter.ReleaseBackGround();
        uiRacePresenter = new UIRacePresenter(this.Container);
        UIHeaderPresenter.ShowHeaderAsync(true, HorseRaceContext.RaceMatchDataContext.RacingRoomType.ToString()).Forget();
        UIHeaderPresenter.OnBack += OnBack;
        uiRacePresenter.OnFindMatch += OnFindMatch;
        await uiRacePresenter.ShowUIQuickRaceAsync();
    }

    private void OnFindMatch()
    {
        ChangeState<RacingMatchFindingState>();
    }
    
    private void OnBack()
    {
        HorseRaceContext.RaceMatchDataContext.RacingRoomType = RacingRoomType.None;
        this.Machine.ChangeState<RaceModeChoosingState>();
    }

    public override void Exit()
    {
        base.Exit();
        UIHeaderPresenter.HideHeader();
        UIHeaderPresenter.OnBack -= OnBack;
        uiRacePresenter.Dispose();
        
        uiRacePresenter = default;
        uiHeaderPresenter = default;
        horseRaceContext = default;
    }
}