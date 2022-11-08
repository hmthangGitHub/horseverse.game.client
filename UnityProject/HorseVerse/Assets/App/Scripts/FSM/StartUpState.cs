using System;
using Cysharp.Threading.Tasks;

public class StartUpState : InjectedBHState
{
    private ErrorHandler errorHandler = new ErrorHandler();
#if ENABLE_DEBUG_MODULE
    private UIDebugMenuPresenter uiDebugMenuPresenter;
#endif
    public override void Enter()
    {
        base.Enter();
        errorHandler.OnError += ErrorHandlerOnError;
#if ENABLE_DEBUG_MODULE
        uiDebugMenuPresenter ??= new UIDebugMenuPresenter(Container);
        uiDebugMenuPresenter.InitializeAsync().Forget();
        uiDebugMenuPresenter.OnToLevelEditorState += ToLevelEditorState;
#endif
    }

    private void ToLevelEditorState()
    {
        ChangeState<LevelEditorState>();
    }

    public override void AddStates()
    {
        base.AddStates();
        this.AddState<InitialState>();
#if ENABLE_DEBUG_MODULE
        this.AddState<LevelEditorState>();
#endif
        this.AddState<DownloadAssetState>();
        this.SetInitialState<DownloadAssetState>();
    }

    private void ErrorHandlerOnError()
    {
        this.Machine.CurrentState.Exit();
    }

    public override void Exit()
    {
        try
        {
            base.Exit();
        
#if ENABLE_DEBUG_MODULE
            uiDebugMenuPresenter.OnToLevelEditorState -= ToLevelEditorState;
            uiDebugMenuPresenter.Dispose();
            uiDebugMenuPresenter = default;
#endif
        }
        finally
        {
            errorHandler.OnError -= ErrorHandlerOnError;
            errorHandler.Dispose();
            errorHandler = default;
            
            this.Machine.RemoveAllStates();
            this.Machine.Initialize();
        }
        
    }
}