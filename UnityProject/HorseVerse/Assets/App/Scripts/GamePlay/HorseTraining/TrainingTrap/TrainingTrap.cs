using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrainingTrap
{

}

public interface ITrainingTrap<T> : ITrainingTrap where T : new()
{
    T entity { get; }
    void SetEntity(T entity);

}

public class TrainingTrap : MonoBehaviour
{

    public bool IsReady { get; set; }

    public System.Action OnFinishPlatform = ActionUtility.EmptyAction.Instance;

    public virtual void Active() { }
}

public abstract class TrainingTrap<T> : TrainingTrap, ITrainingTrap<T> where T : new()
{

    public T entity { get; protected set; }

    public void SetEntity(T entity)
    {
        this.entity = entity;
        if (!EqualityComparer<T>.Default.Equals(this.entity, default(T)))
        {
            OnSetEntity();
        }
    }

    protected abstract void OnSetEntity();

    public abstract T ParseData(string data);
}
