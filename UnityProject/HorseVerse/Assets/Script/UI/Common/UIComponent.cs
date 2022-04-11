using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UIComponent
{
    
}

public interface IUIComponent<T> : UIComponent where T : new()
{
    T entity { get; }
    void SetEntity(T entity);

}

public abstract class UIComponent<T> : MonoBehaviour, IUIComponent<T> where T : new()
{
    public T entity { get; protected set; }

    public void SetEntity(T entity)
    {
        this.entity = entity;
        if (!EqualityComparer<T>.Default.Equals(this.entity, default(T)))
        {
            OnSetEntity();
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    protected abstract void OnSetEntity();
}
