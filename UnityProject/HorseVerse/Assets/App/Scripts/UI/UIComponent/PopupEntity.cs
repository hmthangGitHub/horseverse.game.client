using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PopupEntity : UIComponent
{
}

[RequireComponent(typeof(CanvasGroup))]
public abstract class PopupEntity<T> : PopupEntity, IUIComponent<T>, IPopupEntity<T> where T : new()
{
    public CanvasGroup canvasGroup;
    public UISequenceAnimationBase animation;

    public T entity { get; protected set; }

    public void SetEntity(T entity)
    {
        this.entity = entity;
        this.gameObject.SetActive(false);
        OnSetEntity();
    }

    protected virtual UniTask AnimationIn()
    {
        return animation?.AnimationIn() ?? UniTask.CompletedTask;
    }

    protected virtual UniTask AnimationOut()
    {
        return animation?.AnimationOut() ?? UniTask.CompletedTask;
    }

    public async UniTask In()
    {
        gameObject.SetActive(true);
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        DefaultIn();
        await AnimationIn();
    }

    private void DefaultIn()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public async UniTask Out()
    {
        await AnimationOut().AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
        this.gameObject.SetActive(false);
        DefaultOut();
    }

    private void DefaultOut()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    protected virtual void Awake()
    {
        DefaultOut();
    }

    protected abstract void OnSetEntity();
}
