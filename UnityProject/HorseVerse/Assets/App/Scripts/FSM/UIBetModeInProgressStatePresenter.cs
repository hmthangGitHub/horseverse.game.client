using System;
using System.Threading;
using Cysharp.Threading.Tasks;

internal class UIBetModeInProgressStatePresenter : IDisposable
{
    private IDIContainer Container { get; }
    private CancellationTokenSource cts;
    private UIBetInProgressPopUp uiBetInProgressPopUp;
    public event Action OnBack = ActionUtility.EmptyAction.Instance; 
    public UIBetModeInProgressStatePresenter(IDIContainer container)
    {
        Container = container;
    }

    public async UniTaskVoid ShowInProgressAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiBetInProgressPopUp = await UILoader.Instantiate<UIBetInProgressPopUp>(token : cts.Token);
        uiBetInProgressPopUp.SetEntity(new UIBetInProgressPopUp.Entity()
        {
            outerBtn = new ButtonComponent.Entity(() => OnBack())
        });
        await uiBetInProgressPopUp.In();
    }

    public void Dispose()
    {
        UILoader.SafeRelease(ref uiBetInProgressPopUp);
    }
}