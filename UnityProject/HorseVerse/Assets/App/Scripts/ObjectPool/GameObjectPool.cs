using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameObjectPool
{
    public GenericPool<GameObjectWrapper> Pool { get; }

    public GameObjectPool(GameObject prefab)
    {
        Pool = new GenericPool<GameObjectWrapper>(() => new GameObjectWrapper(Pool, Object.Instantiate(prefab)),
            wrapper => wrapper.GameObject.SetActive(true),
            wrapper => wrapper.GameObject.SetActive(false));
    }
}

public class GameObjectWrapper : IPoolableDisposableObject
{
    private readonly GenericPool<GameObjectWrapper> pool;
    public GameObject GameObject { get; }
    
    public GameObjectWrapper(GenericPool<GameObjectWrapper> pool, GameObject gameObject)
    {
        this.pool = pool;
        GameObject = gameObject;
    }

    public void Dispose()
    {
        Object.Destroy(GameObject);
    }

    public void ReturnToPool()
    {
        pool.Return(this);
    }
}

public class GameObjectPoolList : IDisposable
{
    private readonly Dictionary<Object, GameObjectPool> poolList = new Dictionary<Object, GameObjectPool>();

    public GameObjectWrapper Get(GameObject prefab)
    {
        if(!poolList.TryGetValue(prefab, out var gameObjectPool))
        {
            gameObjectPool = new GameObjectPool(prefab);
            poolList.Add(prefab, gameObjectPool);
        }

        return gameObjectPool.Pool.Rent();
    }

    public void Dispose()
    {
        foreach (var item in poolList)
        {
            item.Value.Pool.Clear();
        }
        poolList.Clear();
    }
}