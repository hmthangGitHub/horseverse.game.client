using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class UIBackGroundPresenter : IDisposable
{
    private IDIContainer container;
    private UIBackGround uiBackGround;
    private CancellationTokenSource cts;
    public UIBackGroundPresenter(IDIContainer container)
    {
        this.container = container;
    }

    public async UniTask ShowBackGroundAsync()
    {
        if(uiBackGround == null)
        {
            cts.SafeCancelAndDispose();
            cts = new CancellationTokenSource();
            uiBackGround ??= await UILoader.Instantiate<UIBackGround>(UICanvas.UICanvasType.BackGround, token: cts.Token);
            uiBackGround.SetEntity(new UIBackGround.Entity());
            uiBackGround.In().Forget();
        } 
    }

    public void ReleaseBackGround()
    {
        UILoader.SafeRelease(ref uiBackGround);
    }

    public void Dispose()
    {
        ReleaseBackGround();
    }
}