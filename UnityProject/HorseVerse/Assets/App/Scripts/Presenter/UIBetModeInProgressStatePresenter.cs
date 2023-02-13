using System;
using System.Threading;
using Cysharp.Threading.Tasks;

internal class UIBetModeInProgressStatePresenter : IDisposable
{
    private IDIContainer Container { get; }
    private CancellationTokenSource cts;
    private UIBetInProgressPopUp uiBetInProgressPopUp;
    private UITouchDisablePresenter uiTouchDisablePresenter;
    private UITouchDisablePresenter UITouchDisablePresenter => uiTouchDisablePresenter ??= Container.Inject<UITouchDisablePresenter>();
    
    public event Action OnBack = ActionUtility.EmptyAction.Instance; 
    public event Action OnTimeOut = ActionUtility.EmptyAction.Instance;
    
    private IReadOnlyBetMatchRepository betMatchRepository;
    private IReadOnlyBetMatchRepository BetMatchRepository => betMatchRepository ??= Container.Inject<IReadOnlyBetMatchRepository>();
    
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
            outerBtn = new ButtonComponent.Entity(UniTask.Action(async () =>
            {
                await uiBetInProgressPopUp.Out(); 
                OnBack();
            })),
            timer = new UIComponentCountDownTimer.Entity()
            {
                outDatedEvent = UniTask.Action(async() =>
                {
                    await UITouchDisablePresenter.ShowTillFinishTaskAsync(UniTask.Delay(1000));
                    if(uiBetInProgressPopUp != default)
                        await uiBetInProgressPopUp.Out();
                    OnTimeOut();
                }),
                utcEndTimeStamp = (int)BetMatchRepository.Current.TimeToNextMatch,
            }
        });
        await uiBetInProgressPopUp.In().AttachExternalCancellation(cts.Token); 
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiBetInProgressPopUp);
    }
}