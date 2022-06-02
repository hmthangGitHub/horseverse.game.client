using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIContainer : IDIContainer
{
    private Dictionary<Type, object> dependencies = new Dictionary<Type, object>();

    public void Bind<T>(T dependency)
    {
        dependencies.Add(typeof(T), dependency);
    }

    public T Inject<T>()
    {
        return (T)dependencies[typeof(T)];
    }

    public void RemoveAndDisposeIfNeed<T>()
    {
        if (dependencies.TryGetValue(typeof(T), out var dependency))
        {
            if (dependency is IDisposable disposeable)
            {
                disposeable.Dispose();
            }
            dependencies.Remove(typeof(T));
        }
    }
}
