using Cysharp.Threading.Tasks;

public class StartUpState : InjectedBHState
{
    private ErrorHandler errorHandler;
#if ENABLE_DEBUG_MODULE
    private UIDebugMenuPresenter uiDebugMenuPresenter;
#endif
    private bool isNeedResetState = false;
    private StartUpStatePresenter startUpStateHandler;
    
    public override void Enter()
    {
        base.Enter();
        OnEnterStateAsync().Forget();
#if ENABLE_DEBUG_MODULE
        if (uiDebugMenuPresenter == default)
        {
            uiDebugMenuPresenter ??= new UIDebugMenuPresenter(Container);
            uiDebugMenuPresenter.InitializeAsync().Forget();
            uiDebugMenuPresenter.OnToLevelEditorState += ToLevelEditorState;
            uiDebugMenuPresenter.OnToTrainingState += ToTrainingState;
            Container.Bind(uiDebugMenuPresenter);
        }
#endif
    }

    private async UniTaskVoid OnEnterStateAsync()
    {
        errorHandler = await ErrorHandler.Instantiate();
        errorHandler.OnError += OnReboot;
        startUpStateHandler = new StartUpStatePresenter();
        startUpStateHandler.OnReboot += OnReboot;
        Container.Bind(startUpStateHandler);
    }

#if ENABLE_DEBUG_MODULE
    private void ToLevelEditorState()
    {
        ChangeState<LevelEditorState>();
    }
    
    private void ToTrainingState()
    {
        ChangeState<TrainingState>();
    }
#endif

    public override void AddStates()
    {
        base.AddStates();
        this.AddState<InitialState>();
        this.AddState<LoginState>();
#if ENABLE_DEBUG_MODULE
        this.AddState<LevelEditorState>();
        this.AddState<TrainingState>();
#endif
        this.AddState<DownloadAssetState>();
        this.SetInitialState<DownloadAssetState>();
    }

    private void OnReboot()
    {
        isNeedResetState = true;
        this.Machine.CurrentState.Exit();
    }

    public override void Exit()
    {
        try
        {
            base.Exit();
#if ENABLE_DEBUG_MODULE
            Container.RemoveAndDisposeIfNeed<UIDebugMenuPresenter>();
            uiDebugMenuPresenter.OnToLevelEditorState -= ToLevelEditorState;
            DisposeUtility.SafeDispose(ref uiDebugMenuPresenter);
#endif
            startUpStateHandler.OnReboot -= OnReboot;
            Container.RemoveAndDisposeIfNeed<StartUpStatePresenter>();
        }
        finally
        {
            errorHandler.OnError -= OnReboot;
            DisposeUtility.SafeDispose(ref errorHandler);
            
            this.Machine.RemoveAllStates();
            if (isNeedResetState)
            {
                ((MonoFSMContainer)this.Machine).Reset();
                this.Machine.Initialize();
            }
            isNeedResetState = false;
        }
        
    }
}