using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DIContainer : IDIContainer
{
    private Dictionary<Type, object> dependencies = new Dictionary<Type, object>();

    public void Bind<T>(T dependency)
    {
        if (dependency == null)
        {
            throw new ArgumentNullException("Null dependency");
        }
        dependencies.Add(typeof(T), dependency);
    }

    public T Inject<T>()
    {
        var inheritDependencies = dependencies.Where(x => typeof(T).IsAssignableFrom(x.Key) || typeof(T) == x.Key)
                                              .Select(x => x.Value);
        if (inheritDependencies.Count() == 1)
        {
            return (T)inheritDependencies.First();
        }
        else if (inheritDependencies.Count() > 1)
        {
            throw new Exception("Duplicated dependencies");
        }
        else
        {
            throw new Exception("Not contains any dependency");
        }
    }

    public void RemoveAndDisposeIfNeed<T>()
    {
        if (dependencies.TryGetValue(typeof(T), out var dependency))
        {
            try
            {
                if (dependency is IDisposable disposeable)
                {
                    disposeable.Dispose();
                }
                dependencies.Remove(typeof(T));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to dispose {e}");
            }
        }
    }
}
