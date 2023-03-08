public interface IInjectedState
{
    IDIContainer Container { get; }
}

public interface IOnApplicationPauseState
{
    void OnApplicationPause(bool pauseState);
}