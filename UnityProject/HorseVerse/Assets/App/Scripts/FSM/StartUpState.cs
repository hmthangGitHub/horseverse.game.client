using System;
//using System.Windows.Forms;
using Cysharp.Threading.Tasks;

public class StartUpState : InjectedBHState
{
    private ErrorHandler errorHandler;
#if ENABLE_DEBUG_MODULE
    private UIDebugMenuPresenter uiDebugMenuPresenter;
#endif
    private bool isNeedResetState = false;
    
    public override void Enter()
    {
        base.Enter();
        errorHandler = new ErrorHandler();
        errorHandler.OnError += ErrorHandlerOnError;
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
#if ENABLE_DEBUG_MODULE
        this.AddState<LevelEditorState>();
        this.AddState<TrainingState>();
#endif
        this.AddState<DownloadAssetState>();
        this.SetInitialState<DownloadAssetState>();
    }

    private void ErrorHandlerOnError()
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
        }
        finally
        {
            errorHandler.OnError -= ErrorHandlerOnError;
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