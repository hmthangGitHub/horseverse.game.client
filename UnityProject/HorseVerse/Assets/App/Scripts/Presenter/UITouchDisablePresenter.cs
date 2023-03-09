using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class UITouchDisablePresenter : IDisposable
{
    private readonly IDIContainer container;
    private ISocketClient socketClient;
    private ISocketClient SocketClient => socketClient ??= container.Inject<ISocketClient>();
    
    private CancellationTokenSource cts;
    private UITouchDisable uiTouchDisable;
    private UITouchDisable internalUiTouchDisable;
    private int uiInOutNumber;

    private UITouchDisablePresenter(IDIContainer container)
    {
        this.container = container;
        SocketClient.OnStartRequest += OnStartRequest;
        SocketClient.OnEndRequest += OnEndRequest;
        PopupEntity.BeginInOut += OnBeginInOut;
        PopupEntity.EndInOut += OnEndInOut;
    }

    private void OnEndInOut(Type type)
    {
        if (type != internalUiTouchDisable.GetType())
        {
            uiInOutNumber--;
            if (uiInOutNumber <= 0)
            {
                internalUiTouchDisable.Out().Forget();
            }
        }
    }

    private void OnBeginInOut(Type type)
    {
        if (type != internalUiTouchDisable.GetType())
        {
            uiInOutNumber++;
            internalUiTouchDisable.In().Forget();
        }
    }

    private void OnEndRequest()
    {
        Hide().Forget();
    }

    private void OnStartRequest()
    {
        ShowAsync().Forget();
    }

    public static async UniTask<UITouchDisablePresenter> InstantiateAsync(IDIContainer container)
    {
        var presenter = new UITouchDisablePresenter(container);
        presenter.uiTouchDisable = await UILoader.Instantiate<UITouchDisable>(UICanvas.UICanvasType.Loading);
        presenter.internalUiTouchDisable = await UILoader.Instantiate<UITouchDisable>(UICanvas.UICanvasType.Loading);
        presenter.uiTouchDisable.SetEntity(new UITouchDisable.Entity()
        {
            loadingHorse = true
        });
        presenter.internalUiTouchDisable.SetEntity(new UITouchDisable.Entity());
        return presenter;
    }

    public async UniTask ShowAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await uiTouchDisable.In();
    }

    public async UniTask ShowTillFinishTaskAsync(UniTask task)
    {
        try
        {
            await ShowAsync();
            await task;
        }
        finally
        {
            await Hide();
        }
    }

    public async UniTask Delay(float second, CancellationToken token = default, bool ignoreTimeScale = false)
    {
        await ShowTillFinishTaskAsync(UniTask.Delay(TimeSpan.FromSeconds(second), cancellationToken : token, ignoreTimeScale: ignoreTimeScale));
    }
    
    public async UniTask Hide()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        await uiTouchDisable.Out();
    }
    
    public void Dispose()
    {
        SocketClient.OnStartRequest -= OnStartRequest;
        SocketClient.OnEndRequest -= OnEndRequest;
        socketClient = default;
        PopupEntity.BeginInOut -= OnBeginInOut;
        PopupEntity.EndInOut -= OnEndInOut;
        
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiTouchDisable);
        UILoader.SafeRelease(ref internalUiTouchDisable);
    }
}