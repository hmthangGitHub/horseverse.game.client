using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class LevelEditorState : InjectedBState
{
    private LevelEditorPresenter levelEditorPresenter;
    public override void Enter()
    {
        base.Enter();
        OnEnterStateAsync().Forget();
    }

    private async UniTask OnEnterStateAsync()
    {
        levelEditorPresenter ??= new LevelEditorPresenter(Container);
        await levelEditorPresenter.ShowDebugMenuAsync();
        levelEditorPresenter.OnBack += OnBack;
    }

    private void OnBack()
    {
        this.Machine.ChangeState<InitialState>();
    }

    public override void Exit()
    {
        base.Exit();
        levelEditorPresenter.OnBack -= OnBack;
        levelEditorPresenter.Dispose();
        levelEditorPresenter = default;
    }
}