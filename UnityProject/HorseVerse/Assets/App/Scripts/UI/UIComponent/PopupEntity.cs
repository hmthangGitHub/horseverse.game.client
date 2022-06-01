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
    public T entity { get; protected set; }

    public void SetEntity(T entity)
    {
        this.entity = entity;
        this.gameObject.SetActive(false);
        OnSetEntity();
    }

    public virtual async UniTask In()
    {
        DefaultIn();
        await UniTask.CompletedTask;
    }

    private void DefaultIn()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        this.gameObject.SetActive(true);
    }

    public virtual async UniTask Out()
    {
        DefaultOut();
        await UniTask.CompletedTask;
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
