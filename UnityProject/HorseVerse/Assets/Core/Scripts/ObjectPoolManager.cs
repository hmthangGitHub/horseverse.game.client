using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Core
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        protected ObjectPoolManager()
        {

        }

        protected Dictionary<string, List<GameObject>> _objectsPool = new Dictionary<string, List<GameObject>>();

        public void Add(string key, GameObject go)
        {
            if (go == null) return;
            List<GameObject> list = null;
            if(!_objectsPool.TryGetValue(key, out list))
            {
                list = new List<GameObject>();
                _objectsPool.Add(key, list);
            }
            go.SetActive(false);
            go.transform.SetParent(transform);
            list.Add(go);
            //Debug.Log("ObjectPoolManager Add key: " + key + ": " + list.Count);
        }

        public GameObject Get(string key)
        {
            List<GameObject> list = null;
            if (_objectsPool.TryGetValue(key, out list))
            {
                if(list.Count > 0)
                {
                    var go = list[0];
                    list.RemoveAt(0);
                    go.transform.SetParent(null);
                    //Debug.Log("ObjectPoolManager Get key: " + key + ": " + list.Count);
                    go.SetActive(true);
                    return go;
                }
            }
            return null;
        }

        public void Clear(string key)
        {
            List<GameObject> list = null;
            if (_objectsPool.TryGetValue(key, out list))
            {
                for(int i = 0; i < list.Count; i++)
                {
                    Destroy(list[i].gameObject);
                }
                list.Clear();
            }
        }

        public void ClearAll()
        {
            foreach(var item in _objectsPool)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    Destroy(item.Value[i].gameObject);
                }
                item.Value.Clear();
            }
            _objectsPool.Clear();
        }
    }
}

