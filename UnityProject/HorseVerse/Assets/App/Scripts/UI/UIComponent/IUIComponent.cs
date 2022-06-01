using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIComponent
{
    
}

public interface IUIComponent<T> : IUIComponent where T : new()
{
    T entity { get; }
    void SetEntity(T entity);

}

public class UIComponent : MonoBehaviour
{
    
}

public abstract class UIComponent<T> : UIComponent, IUIComponent<T> where T : new()
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
