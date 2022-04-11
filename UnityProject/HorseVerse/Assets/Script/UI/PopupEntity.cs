using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PopupEntity<T> : MonoBehaviour, IUIComponent<T>, IPopupEntity<T> where T : new()
{
    public T entity { get; protected set; }

    public void SetEntity(T entity)
    {
        this.entity = entity;
        this.gameObject.SetActive(false);
        OnSetEntity();
    }

    public virtual async UniTask In()
    {
        this.gameObject.SetActive(true);
        await UniTask.CompletedTask;
    }

    public virtual async UniTask Out()
    {
        this.gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }

    new private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    protected abstract void OnSetEntity();
}
