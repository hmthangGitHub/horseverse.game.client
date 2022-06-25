using Cysharp.Threading.Tasks;

public class StartUpState : InjectedBHState
{
    private ErrorHandler errorHandler = new ErrorHandler();

    public async override void Enter()
    {
        base.Enter();
        errorHandler.OnError += ErrorHandlerOnError;
    }

    public override void AddStates()
    {
        base.AddStates();
        this.AddState<InitialState>();
        this.SetInitialState<InitialState>();
    }

    private void ErrorHandlerOnError()
    {
        this.Machine.CurrentState.Exit();
    }

    public override void Exit()
    {
        base.Exit();
        errorHandler.OnError -= ErrorHandlerOnError;
        errorHandler.Dispose();
        this.Machine.RemoveAllStates();
        this.Machine.Initialize();
    }
}