using UnityEngine;
using System.Collections;

namespace Core{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance != null) {
                        return _instance;
                    }
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        //DontDestroyOnLoad(_instance.gameObject);
                        return _instance;
                    }
                    if (_instance != null) {
                        //DontDestroyOnLoad(_instance.gameObject);
                        return _instance;
                    }
                    var singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = string.Format("[{0}]", typeof(T));
                    DontDestroyOnLoad(singleton);
                    return _instance;
                }
            }
        }

    }
}