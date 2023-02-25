using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class MonoBehaviourPool<T> : GenericPool<T> where T : MonoBehaviour, IPoolableObject, IDisposable
{
    public MonoBehaviourPool(T prefab) : base(() => Object.Instantiate(prefab), 
        wrapper => wrapper.gameObject.SetActive(true), 
        wrapper => wrapper.gameObject.SetActive(false))
    {
    }
}