using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PopupEntity : UIComponent
{
    public static event Action<Type> BeginInOut = ActionUtility.EmptyAction<Type>.Instance;
    public static event Action<Type> EndInOut = ActionUtility.EmptyAction<Type>.Instance;

    protected void InvokeBegin()
    {
        BeginInOut.Invoke(this.GetType());
    }

    protected void InvokeEnd()
    {
        EndInOut.Invoke(this.GetType());
    }
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
        if (!EqualityComparer<T>.Default.Equals(this.entity, default(T)))
        {
            this.gameObject.SetActive(true);
            OnSetEntity();
        }
        else
        {
            this.gameObject.SetActive(false);
        }
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
        try
        {
            InvokeBegin();
            gameObject.SetActive(true);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, this.GetCancellationTokenOnDestroy());
            DefaultIn();
            await AnimationIn().AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            InvokeEnd();
        }
    }

    private void DefaultIn()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public async UniTask Out()
    {
        try
        {
            InvokeBegin();
            await AnimationOut().AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
            this.gameObject.SetActive(false);
            DefaultOut();
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            InvokeEnd();
        }
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
