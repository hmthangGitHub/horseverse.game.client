using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformGeneratorPool : MonoBehaviour
{
    protected Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();

    bool _destroying = false;

    private void OnEnable()
    {
        _destroying = false;
    }

    public Object GetOrInstante(Object prefab, Transform parent = null)
    {
        if (_pools.ContainsKey(prefab.name))
        {
            var queue = _pools[prefab.name];
            if (queue.Count > 0)
            {
                var item = queue.Dequeue(); Debug.Log("Get from pool " + prefab.name);
                item.SetActive(true);
                item.transform.parent = parent;
                item.transform.localPosition = Vector3.zero;
                return item;
            }
        }
        return Instantiate(prefab, parent);
    }

    public void AddToPool(string prefabName, GameObject gameObject)
    {
        if (this.transform == default || _destroying) { Destroy(gameObject); return; }
        if (!_pools.ContainsKey(prefabName))
            _pools.Add(prefabName, new Queue<GameObject>());
        var queue = _pools[prefabName];
        gameObject.SetActive(false);
        gameObject.transform.parent = this.transform;
        queue.Enqueue(gameObject);
        _pools[prefabName] = queue;
        Debug.Log("Add to pool " + prefabName);
    }

    private void OnDestroy()
    {
        _destroying = true;
        clearPool();
    }


    void clearPool()
    {
        foreach(var _key in _pools.Keys)
        {
            var item = _pools[_key];
            if (item != null)
            {
                while (item.Count > 0)
                {
                    var ii = item.Dequeue();
                    Destroy(ii);
                }
            }
        }
        _pools.Clear();
    }
}
