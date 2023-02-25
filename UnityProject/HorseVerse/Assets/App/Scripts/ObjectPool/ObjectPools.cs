using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPools<T> : IDisposable
    where T : IPoolableObject
{
    bool disposedValue = false;
    Queue<T> q;

    /// <summary>
    /// Limit of instace count.
    /// </summary>
    protected int MaxPoolCount
    {
        get { return int.MaxValue; }
    }

    /// <summary>
    /// Create instance when needed.
    /// </summary>
    protected abstract T CreateInstance();

    /// <summary>
    /// Called before return to pool, useful for set active object(it is default behavior).
    /// </summary>
    protected abstract void OnBeforeRent(T instance);

    /// <summary>
    /// Called before return to pool, useful for set inactive object(it is default behavior).
    /// </summary>
    protected abstract void OnBeforeReturn(T instance);

    /// <summary>
    /// Called when clear or disposed, useful for destroy instance or other finalize method.
    /// </summary>
    protected abstract void OnClear(T instance);
    // {
    //     if (instance == null) return;
    //
    //     var go = instance.gameObject;
    //     if (go == null) return;
    //     UnityEngine.Object.Destroy(go);
    // }

    /// <summary>
    /// Current pooled object count.
    /// </summary>
    public int Count
    {
        get
        {
            if (q == null) return 0;
            return q.Count;
        }
    }

    /// <summary>
    /// Get instance from pool.
    /// </summary>
    public T Rent()
    {
        if (disposedValue) throw new ObjectDisposedException("ObjectPool was already disposed.");
        if (q == null) q = new Queue<T>();

        var instance = (q.Count > 0)
            ? q.Dequeue()
            : CreateInstance();

        OnBeforeRent(instance);
        return instance;
    }

    /// <summary>
    /// Return instance to pool.
    /// </summary>
    public void Return(T instance)
    {
        if (disposedValue) throw new ObjectDisposedException("ObjectPool was already disposed.");
        if (instance == null) throw new ArgumentNullException("instance");

        if (q == null) q = new Queue<T>();

        if ((q.Count + 1) == MaxPoolCount)
        {
            throw new InvalidOperationException("Reached Max PoolSize");
        }

        OnBeforeReturn(instance);
        q.Enqueue(instance);
    }

    /// <summary>
    /// Clear pool.
    /// </summary>
    public void Clear(bool callOnBeforeRent = false)
    {
        if (q == null) return;
        while (q.Count != 0)
        {
            var instance = q.Dequeue();
            if (callOnBeforeRent)
            {
                OnBeforeRent(instance);
            }

            OnClear(instance);
        }
    }

    /// <summary>
    /// Trim pool instances. 
    /// </summary>
    /// <param name="instanceCountRatio">0.0f = clear all ~ 1.0f = live all.</param>
    /// <param name="minSize">Min pool count.</param>
    /// <param name="callOnBeforeRent">If true, call OnBeforeRent before OnClear.</param>
    public void Shrink(float instanceCountRatio,
                       int minSize,
                       bool callOnBeforeRent = false)
    {
        if (q == null) return;

        if (instanceCountRatio <= 0) instanceCountRatio = 0;
        if (instanceCountRatio >= 1.0f) instanceCountRatio = 1.0f;

        var size = (int)(q.Count * instanceCountRatio);
        size = Math.Max(minSize, size);

        while (q.Count > size)
        {
            var instance = q.Dequeue();
            if (callOnBeforeRent)
            {
                OnBeforeRent(instance);
            }

            OnClear(instance);
        }
    }

    #region IDisposable Support

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Clear(false);
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    #endregion
}

/// <summary>
/// Bass class of ObjectPool. If needs asynchronous initialization, use this instead of standard ObjectPool.
/// </summary>
public abstract class AsyncObjectPool<T> : IDisposable
    where T : UnityEngine.Component
{
    bool disposedValue = false;
    Queue<T> q;

    /// <summary>
    /// Limit of instace count.
    /// </summary>
    protected int MaxPoolCount
    {
        get { return int.MaxValue; }
    }

    /// <summary>
    /// Called before return to pool, useful for set active object(it is default behavior).
    /// </summary>
    protected virtual void OnBeforeRent(T instance)
    {
        instance.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called before return to pool, useful for set inactive object(it is default behavior).
    /// </summary>
    protected virtual void OnBeforeReturn(T instance)
    {
        instance.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when clear or disposed, useful for destroy instance or other finalize method.
    /// </summary>
    protected virtual void OnClear(T instance)
    {
        if (instance == null) return;

        var go = instance.gameObject;
        if (go == null) return;
        UnityEngine.Object.Destroy(go);
    }

    /// <summary>
    /// Current pooled object count.
    /// </summary>
    public int Count
    {
        get
        {
            if (q == null) return 0;
            return q.Count;
        }
    }


    /// <summary>
    /// Return instance to pool.
    /// </summary>
    public void Return(T instance)
    {
        if (disposedValue) throw new ObjectDisposedException("ObjectPool was already disposed.");
        if (instance == null) throw new ArgumentNullException("instance");

        if (q == null) q = new Queue<T>();

        if ((q.Count + 1) == MaxPoolCount)
        {
            throw new InvalidOperationException("Reached Max PoolSize");
        }

        OnBeforeReturn(instance);
        q.Enqueue(instance);
    }

    /// <summary>
    /// Trim pool instances. 
    /// </summary>
    /// <param name="instanceCountRatio">0.0f = clear all ~ 1.0f = live all.</param>
    /// <param name="minSize">Min pool count.</param>
    /// <param name="callOnBeforeRent">If true, call OnBeforeRent before OnClear.</param>
    public void Shrink(float instanceCountRatio,
                       int minSize,
                       bool callOnBeforeRent = false)
    {
        if (q == null) return;

        if (instanceCountRatio <= 0) instanceCountRatio = 0;
        if (instanceCountRatio >= 1.0f) instanceCountRatio = 1.0f;

        var size = (int)(q.Count * instanceCountRatio);
        size = Math.Max(minSize, size);

        while (q.Count > size)
        {
            var instance = q.Dequeue();
            if (callOnBeforeRent)
            {
                OnBeforeRent(instance);
            }

            OnClear(instance);
        }
    }

    /// <summary>
    /// Clear pool.
    /// </summary>
    public void Clear(bool callOnBeforeRent = false)
    {
        if (q == null) return;
        while (q.Count != 0)
        {
            var instance = q.Dequeue();
            if (callOnBeforeRent)
            {
                OnBeforeRent(instance);
            }

            OnClear(instance);
        }
    }

    #region IDisposable Support

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Clear(false);
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    #endregion
}

public interface IPoolableObject
{
}

public interface IReturnable
{
    public void ReturnToPool();
}

public interface IPoolableDisposableObject : IPoolableObject, IDisposable, IReturnable
{
}