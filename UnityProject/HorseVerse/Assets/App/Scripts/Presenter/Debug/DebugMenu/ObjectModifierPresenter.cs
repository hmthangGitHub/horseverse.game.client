using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using io.hverse.game.protogen;

public class ObjectModifierPresenter : IDisposable
{
    private readonly IDIContainer container;
    private UIDebugMenuObjectModifier uiDebugMenuObjectModifier;
    private UITouchDisablePresenter uiTouchDisablePresenter;
    private UITouchDisablePresenter UITouchDisablePresenter => uiTouchDisablePresenter ??= container.Inject<UITouchDisablePresenter>();
    private CancellationTokenSource cts;
    public ObjectModifierPresenter(IDIContainer container)
    {
        this.container = container;
        cts = new CancellationTokenSource();
    }

    public enum ModifyAction
    {
        Cancel,
        Modified
    }
    
    public async UniTask<ModifyAction> ModifiedObjectAsync(object obj, string title)
    {
        var ucs = new UniTaskCompletionSource<ModifyAction>();
        await UITouchDisablePresenter.ShowTillFinishTaskAsync(UniTask.Create(async () =>
        {
            uiDebugMenuObjectModifier ??= await UILoader.Instantiate<UIDebugMenuObjectModifier>(UICanvas.UICanvasType.Debug, token: cts.Token);
        }));

        uiDebugMenuObjectModifier.SetEntity(new UIDebugMenuObjectModifier.Entity()
        {
            title = title,
            objectToInspect = obj,
            backBtn = new ButtonComponent.Entity(() =>
            {
                uiDebugMenuObjectModifier.Out().Forget();
                ucs.TrySetResult(ModifyAction.Cancel);
            }),
            closeBtn = new ButtonComponent.Entity(() =>
            {
                uiDebugMenuObjectModifier.Out().Forget();
                ucs.TrySetResult(ModifyAction.Cancel);
            }),
            okBtn = new ButtonComponent.Entity(UniTask.Action(async () =>
            {
                ucs.TrySetResult(ModifyAction.Modified);
            })),
        });
        await uiDebugMenuObjectModifier.In();
        return await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        UILoader.SafeRelease(ref uiDebugMenuObjectModifier);
    }
}