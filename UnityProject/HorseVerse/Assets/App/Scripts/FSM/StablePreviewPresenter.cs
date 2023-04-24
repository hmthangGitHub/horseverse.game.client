using System;
using System.Threading;
using Cysharp.Threading.Tasks;

internal class StablePreviewPresenter : IDisposable
{
    private readonly IDIContainer container;
    private CancellationTokenSource cts;
    private UISwipeRegister uiSwipeRegister;
    private UIHorseStablePreview uiHorseStablePreview;
    private UIHorse3DViewPresenter uiHorse3DViewPresenter;
    private UIHorse3DViewPresenter UIHorse3DViewPresenter => uiHorse3DViewPresenter ??= container.Inject<UIHorse3DViewPresenter>();
    public event Action OnToHorseDetail = ActionUtility.EmptyAction.Instance;
    public event Action OnToBreeding = ActionUtility.EmptyAction.Instance;
    
    public StablePreviewPresenter(IDIContainer container)
    {
        this.container = container;
    }
    
    public async UniTaskVoid ShowAsync()
    {
        cts = new CancellationTokenSource();
        uiSwipeRegister ??= await UILoader.Instantiate<UISwipeRegister>(token: cts.Token);
        uiHorseStablePreview ??= await UILoader.Instantiate<UIHorseStablePreview>(token: cts.Token);
        uiSwipeRegister.SetEntity(new UISwipeRegister.Entity()
        {
            OnHorizontalDirection = OnSwipe
        });
        uiSwipeRegister.In().Forget();
        uiHorseStablePreview.SetEntity(new UIHorseStablePreview.Entity()
        {
            breedingBtn = new ButtonComponent.Entity(OnToBreeding.Invoke)
        });
        uiHorseStablePreview.In().Forget();
        
        UIHorse3DViewPresenter.OnTouchHorseEvent += OnTouchHorse;
    }

    private void OnTouchHorse()
    {
        OnToHorseDetail.Invoke();
        // UIHorse3DViewPresenter.ChangeCameraType(MainMenuCameraType.CameraType.StableDetail);
    }

    private void OnSwipe(UISwipeRegister.Direction d)
    {
        UIHorse3DViewPresenter.ChangeHorseOnSwipe(d == UISwipeRegister.Direction.LEFT ? -1 : 1).Forget();
    }

    public void Dispose()
    {
        UILoader.SafeRelease(ref uiSwipeRegister);
        UILoader.SafeRelease(ref uiHorseStablePreview);
        UIHorse3DViewPresenter.OnTouchHorseEvent -= OnTouchHorse;
        uiHorse3DViewPresenter = default;
    }
}