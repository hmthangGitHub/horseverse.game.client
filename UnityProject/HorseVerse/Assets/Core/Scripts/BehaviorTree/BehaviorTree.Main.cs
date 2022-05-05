using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Core.MVVM;
using CoreData;
using SQLite4Unity3d;

namespace Core.Behavior
{
    public class BehaviorTree : ParentTask
    {
        class RegisterEventOfTree
        {
            public HasReceiveEventTask Task;
            public object Callback;
        }

        public class PropertyVariable : OriginalModel
        {
            public string Key { get; set; }
            public SharePropertyBehavior Value { get; set; }
        }
        private List<PropertyVariable> _listPropertyVariable = new List<PropertyVariable>();
        public List<PropertyVariable>  ListPropertyVariable{
            get{
                return _listPropertyVariable;
            }
            set{
                _listPropertyVariable = value;
            }
        }

        //public List<Task> AllTasks { get; set; }

        Dictionary<string, List<RegisterEventOfTree>> _dictRegisterEvent = new Dictionary<string, List<RegisterEventOfTree>>();
        Dictionary<string, List<RegisterEventOfTree>> _dictRegisterEvent1Param = new Dictionary<string, List<RegisterEventOfTree>>();
        Dictionary<string, List<RegisterEventOfTree>> _dictRegisterEvent2Param = new Dictionary<string, List<RegisterEventOfTree>>();
        Dictionary<string, List<RegisterEventOfTree>> _dictRegisterEvent3Param = new Dictionary<string, List<RegisterEventOfTree>>();

        protected internal List<ConditionTask> _receiveTasks = new List<ConditionTask>();
        protected internal List<HasReceiveEventTask> _receiveHasEventTasks = new List<HasReceiveEventTask>();

        bool _isRunning = false;
        [Ignore]
        internal override TransformViewModel Transform
        {
            get
            {
                return _transformVM;
            }
        }
        TransformViewModel _transformVM;
        public void SetTransform(TransformViewModel transformViewModel){
            _transformVM = transformViewModel;
        }
        float _tickIntervalTree = 0.05f;
        public void SetTickInterval(float tickInterval){
            _tickIntervalTree = tickInterval;
            if(_tickIntervalTree <= 0){
                _tickIntervalTree = 0.05f;
            }
        }
        [Ignore]
        internal override float TickInterval{
            get{
                return _tickIntervalTree;
            }
        }

        private int _seed;
        public void SetSeed(int seed){
            _seed = seed;
            _random = new System.Random(_seed);
        }
        System.Random _random;
        public float RandomRange(float min, float max)
        {
            if (_random == null)
            {
                _random = new System.Random(_seed);
            }
            var n = max - min < 0 ? 1 : (max - min);
            return _random.Next(0, 101) * n / 100 + min;
        }

        public int RandomRange(int min, int max)
        {
            if (_random == null)
            {
                _random = new System.Random(_seed);
            }
            return _random.Next(min, max);
        }

        public BehaviorTree()
        {
        }

        public void AddVariable(string key, SharePropertyBehavior value)
        {
            if(!_listPropertyVariable.Any(x => x.Key == key)){
                value.VariableKey = key;
                _listPropertyVariable.Add(new PropertyVariable { Key = key, Value = value });
            }
        }

        public T GetVariable<T>(string key) where T : SharePropertyBehavior, new()
        {
            var propertyVariable = _listPropertyVariable.FirstOrDefault(x => x.Key == key);
            if(propertyVariable != null)
            {
                var shareProperty = propertyVariable.Value;
                if (shareProperty != null)
                {
                    var t = shareProperty as T;
                    return t;
                }
            }
            return null;
        }

        public object GetVariable(string key)
        {
            var propertyVariable = _listPropertyVariable.FirstOrDefault(x => x.Key == key);
            if(propertyVariable != null)
            {
                return propertyVariable.Value;
            }
            return null;
        }

        public void SendEvent(string key)
        {
            List<RegisterEventOfTree> list = null;
            if (_dictRegisterEvent.TryGetValue(key, out list))
            {
                for (int i = 0; i < list.Count; i++){
                    var temp = list[i];
                    Action action = temp.Callback as Action;
                    if (action != null)
                    {
                        action();
                    }
                }
            }
        }

        public void SendEvent<T>(string key, T t)
        {
            List<RegisterEventOfTree> list = null;
            if (_dictRegisterEvent.TryGetValue(key, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var temp = list[i];
                    Action<T> action = temp.Callback as Action<T>;
                    if (action != null)
                    {
                        action(t);
                    }
                }
            }
        }

        public void SendEvent<T, U>(string key, T t, U u)
        {
            List<RegisterEventOfTree> list = null;
            if (_dictRegisterEvent.TryGetValue(key, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var temp = list[i];
                    Action<T, U> action = temp.Callback as Action<T, U>;
                    if (action != null)
                    {
                        action(t, u);
                    }
                }
            }
        }

        public void SendEvent<T, U, K>(string key, T t, U u, K k)
        {
            List<RegisterEventOfTree> list = null;
            if (_dictRegisterEvent.TryGetValue(key, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var temp = list[i];
                    Action<T, U, K> action = temp.Callback as Action<T, U, K>;
                    if (action != null)
                    {
                        action(t, u, k);
                    }
                }
            }
        }

        public void RegisterEvent(string key, Action callback)
        {
            if(_dictRegisterEvent.ContainsKey(key)){
                var value = _dictRegisterEvent[key];
                value.Add(new BehaviorTree.RegisterEventOfTree { Callback = callback });
            }else{
                _dictRegisterEvent.Add(key, new List<RegisterEventOfTree> { new BehaviorTree.RegisterEventOfTree { Callback = callback } });
            }

        }

        public void RegisterEvent<T>(string key, Action<T> callback)
        {
            if (_dictRegisterEvent.ContainsKey(key))
            {
                var value = _dictRegisterEvent[key];
                value.Add(new BehaviorTree.RegisterEventOfTree { Callback = callback });
            }
            else
            {
                _dictRegisterEvent.Add(key, new List<RegisterEventOfTree> { new BehaviorTree.RegisterEventOfTree { Callback = callback } });
            }
        }

        public void RegisterEvent<T, U>(string key, Action<T, U> callback)
        {
            if (_dictRegisterEvent.ContainsKey(key))
            {
                var value = _dictRegisterEvent[key];
                value.Add(new BehaviorTree.RegisterEventOfTree { Callback = callback });
            }
            else
            {
                _dictRegisterEvent.Add(key, new List<RegisterEventOfTree> { new BehaviorTree.RegisterEventOfTree { Callback = callback } });
            }
        }

        public void RegisterEvent<T, U, K>(string key, Action<T, U, K> callback)
        {
            if (_dictRegisterEvent.ContainsKey(key))
            {
                var value = _dictRegisterEvent[key];
                value.Add(new BehaviorTree.RegisterEventOfTree { Callback = callback });
            }
            else
            {
                _dictRegisterEvent.Add(key, new List<RegisterEventOfTree> { new BehaviorTree.RegisterEventOfTree { Callback = callback } });
            }
        }

        protected internal void RegisterEvent(string key, Action callback, HasReceiveEventTask task)
        {
            if (_dictRegisterEvent.ContainsKey(key))
            {
                var value = _dictRegisterEvent[key];
                value.Add(new BehaviorTree.RegisterEventOfTree { Callback = callback, Task = task });
            }
            else
            {
                _dictRegisterEvent.Add(key, new List<RegisterEventOfTree> { new BehaviorTree.RegisterEventOfTree { Callback = callback, Task = task } });
            }

        }

        public void AddStartTask(Task task)
        {
            this.AddTask(task);
        }

        public void Pause()
        {
            _isRunning = false;
        }

        public void Resume()
        {
            _isRunning = true;
        }

        public void Reset()
        {

        }

        public void StartBehaviour(){
            if (_tree == null)
            {
                _tree = this;
                _tasks[0].Initial(this);
            }
            _isRunning = true;
            Start();
        }

        protected override void OnUpdate()
        {
            if (_isRunning)
            {
                for (int i = 0; i < _receiveTasks.Count; i++)
                {
                    var receiveTask = _receiveTasks[i];
                    TaskStatus result;
                    if(receiveTask.CanReceiveEvent(out result)){
                        //Debug.Log("receiveTask: " + receiveTask.Name + ";" + receiveTask.TaskStatus + ";" + receiveTask.GetType() + "; " + Time.time);
                        if (_receiveHasEventTasks.Count > 0){
                            for (int j = 0; j < _receiveHasEventTasks.Count; j++)
                            {
                                var hasEventTask = _receiveHasEventTasks[j];
                                hasEventTask.HasReceive = false;
                            }
                            _receiveHasEventTasks.Clear();
                        }
                        receiveTask.Parent.CheckRecieveEvent(receiveTask, result);
                        //return;
                    }
                }
                _tasks[0].Update();
            } 
        }

        protected sealed override void OnEnd()
        {
            base.OnEnd();
        }
    }

}
