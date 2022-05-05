using System.Collections;
using System.Collections.Generic;
using Core.Behavior;
using UnityEngine;

namespace Core
{
    public class BehaviorTreePoolManager
    {
        private BehaviorTreePoolManager()
        {

        }

        private static BehaviorTreePoolManager _instance;
        public static BehaviorTreePoolManager Instance
        {
            get
            {
                return _instance ?? (_instance = new BehaviorTreePoolManager());
            }
        }

        protected Dictionary<string, List<BehaviorTree>> _behaviorsPool = new Dictionary<string, List<BehaviorTree>>();

        public void Add(string key, BehaviorTree tree)
        {
            List<BehaviorTree> list = null;
            if (!_behaviorsPool.TryGetValue(key, out list))
            {
                list = new List<BehaviorTree>();
                _behaviorsPool.Add(key, list);
            }
            tree.Reset();
            list.Add(tree);
            //Debug.Log("BehaviorTreePoolManager Add key: " + key + ": " + list.Count);
        }

        public BehaviorTree Get(string key)
        {
            List<BehaviorTree> list = null;
            if (_behaviorsPool.TryGetValue(key, out list))
            {
                if (list.Count > 0)
                {
                    var tree = list[0];
                    tree.Reset();
                    list.RemoveAt(0);
                    //Debug.Log("BehaviorTreePoolManager Get key: " + key + ": " + list.Count);
                    return tree;
                }
            }
            return null;
        }

        public void Clear(string key)
        {
            List<BehaviorTree> list = null;
            if (_behaviorsPool.TryGetValue(key, out list))
            {
                list.Clear();
            }
        }

        public void ClearAll()
        {
            _behaviorsPool.Clear();
        }
    }
}
