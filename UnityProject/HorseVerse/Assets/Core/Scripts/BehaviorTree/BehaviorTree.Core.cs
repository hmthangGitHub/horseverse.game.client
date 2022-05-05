using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Core.MVVM;
using System.Linq;
using CoreData;
using SQLite4Unity3d;
using FixRound;

namespace Core.Behavior
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreShowUIAttribute : Attribute
    {
    }

    public static class ParentTaskExtension
    {
        public static Task AddTask(this ParentTask source, Task task)
        {
            source.AddChildTask(task);
            return source;
        }
    }

    public static class CompositeTaskExtension
    {
        public static CompositeTask AddTask(this CompositeTask source, Task task)
        {
            source.AddChildTask(task);
            return source;
        }
    }

    public enum TaskStatus
    {
        Inactive,
        Failure,
        Success,
        Running
    }

    public enum AbortType
    {
        None,
        Self,
        LowerPriority,
        Both
    }



    public class SharePropertyBehavior : Node{
        public string VariableKey { get; set; } = "";
    }

    public sealed class ShareStringBehavior : SharePropertyBehavior
    {
        public string Value { get; set; }

        public ShareStringBehavior() : base(){
            
        }

        public ShareStringBehavior(string value) : base()
        {
            Value = value;
        }
    }

    public sealed class ShareBoolBehavior : SharePropertyBehavior
    {
        public bool Value { get; set; }

        public ShareBoolBehavior() : base(){

        }

        public ShareBoolBehavior(bool value) : base()
        {
            Value = value;
        }
    }

    public sealed class ShareIntBehavior : SharePropertyBehavior
    {
        public int Value { get; set; }

        public ShareIntBehavior() : base(){

        }

        public ShareIntBehavior(int value) : base()
        {
            Value = value;
        }
    }

    public sealed class ShareFloatBehavior : SharePropertyBehavior
    {
        public float Value { get; set; }

        public ShareFloatBehavior() : base()
        {

        }

        public ShareFloatBehavior(float value) : base()
        {
            Value = value;
        }
    }

    public sealed class ShareVector3Behavior : SharePropertyBehavior
    {
        public Vector3 Value { get; set; }

        public ShareVector3Behavior() : base()
        {

        }

        public ShareVector3Behavior(Vector3 value) : base()
        {
            Value = value;
        }
    }

    public sealed class ShareTransformViewModelBehavior : SharePropertyBehavior
    {
        public TransformViewModel Value { get; set; }

        public ShareTransformViewModelBehavior() : base()
        {

        }

        public ShareTransformViewModelBehavior(TransformViewModel value) : base()
        {
            Value = value;
        }
    }

    public sealed class ShareListTransformViewModelBehavior : SharePropertyBehavior
    {
        public List<TransformViewModel> Value { get; set; }

        public ShareListTransformViewModelBehavior() : base()
        {

        }

        public ShareListTransformViewModelBehavior(List<TransformViewModel> value) : base()
        {
            Value = value;
        }
    }

    public abstract class Node : OriginalModel{
        [PrimaryKey, IgnoreShowUIAttribute]
        public int Id { get; set; }
        [IgnoreShowUIAttribute]
        public string TypeNameOfModel { get; set; }
        [IgnoreShowUIAttribute]
        public string Assembly { get; set; }
        public Node()
        {
            Assembly = "Assembly-CSharp";
            var type = this.GetType();
            TypeNameOfModel = type.ToString();
            Id = Guid.NewGuid().GetHashCode();
        }
    }

    public abstract class TaskNode : Node{
        public float X { get; set; }
        public float Y { get; set; }
    }

    public abstract class Task : TaskNode
    {
        
        protected internal int _priority;
        protected internal BehaviorTree _tree;
        [Ignore]
        internal virtual TransformViewModel Transform{
            get{
                return _tree.Transform;
            }
        }
        [Ignore]
        internal virtual float TickInterval
        {
            get
            {
                return _tree.TickInterval;
            }
        }
        [Ignore]
        public ParentTask Parent { get; set; }
        public string Name { get; set; }
        protected TaskStatus _taskStatus;
        [Ignore]
        public TaskStatus TaskStatus
        {
            get
            {
                return _taskStatus;
            }
            protected internal set
            {
                _taskStatus = value;
                if (_taskStatus == TaskStatus.Running)
                {
                    Running();
                }
                else if (_taskStatus == TaskStatus.Success)
                {
                    End();
                    if (Parent != null)
                    {
                        Parent.ChildSuccess(this);
                    }
                }
                else if (_taskStatus == TaskStatus.Failure)
                {
                    End();
                    if (Parent != null)
                    {
                        Parent.ChildFailure(this);
                    }
                }
                else if (_taskStatus == TaskStatus.Inactive)
                {
                }
            }
        }

        protected internal void Initial(BehaviorTree tree){
            _tree = tree;
            _tree._priority++;
            _priority = _tree._priority;

            var type = this.GetType();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var share = propertyInfo.GetValue(this, null) as SharePropertyBehavior;
                if(share != null){
                    if(!string.IsNullOrEmpty(share.VariableKey)){
                        propertyInfo.SetValue(this, _tree.GetVariable(share.VariableKey), null);
                    }
                }
            }

            var parentTask = this as ParentTask;
            var childrenTask = GetChildren();
            if (childrenTask != null)
            {
                for (int i = 0; i < childrenTask.Count; i++)
                {
                    var childTask = childrenTask[i];

                    childTask.Parent = parentTask;
                    var childIsParentTask = childTask as ParentTask;
                    if(childIsParentTask != null){
                        parentTask.AddChildIsParentTask(childIsParentTask);
                    }

                    var hasReceiveEventTask = childTask as HasReceiveEventTask;
                    if (hasReceiveEventTask != null)
                    {
                        _tree.RegisterEvent(hasReceiveEventTask.Key.Value, () =>
                        {
                            //Debug.Log("RegisterEvent: " + hasReceiveEventTask.Key + ";" + _tree._receiveTasks.Count);
                            //for (int i1 = 0; i1 < _tree._receiveTasks.Count; i1++)
                            //{
                            //    Debug.Log("_receiveTasks: " + i1 + ";" + _tree._receiveTasks[i1].Name);
                            //}
                            if (_tree._receiveTasks.Contains(hasReceiveEventTask)){
                                _tree._receiveHasEventTasks.Add(hasReceiveEventTask);
                                //Debug.Log("_receiveHasEventTasks: " + _tree._receiveHasEventTasks.Count + ";" + Time.time);
                                hasReceiveEventTask.HasReceive = true;
                            }
                        }, hasReceiveEventTask);
                    } else{
                        var compositeTask = childTask as CompositeTask;
                        if (compositeTask == null)
                        {
                            var parentTaskChild = childTask as ParentTask;
                            if(parentTaskChild != null){
                                parentTaskChild._abortType = parentTask._abortType;
                            }
                        }
                    }
                    childTask.Initial(tree);
                }
            }
        }

        protected virtual List<Task> GetChildren(){
            return null;
        }

        protected internal void Start()
        {
            TaskStatus = TaskStatus.Running;
            OnStart();
        }

        protected virtual void OnStart()
        {

        }

        public void Update()
        {
            if (TaskStatus == TaskStatus.Running)
            {
                OnUpdate();
            }
        }

        public virtual void ChangeToRunningImmediately(){
            
        }

        protected virtual void OnUpdate()
        {
            
        }

        protected internal void Running(){
            OnRunning();
        }

        protected virtual void OnRunning(){
            
        }

        protected internal void End()
        {
            //TaskStatus = TaskStatus.Inactive;
            OnEnd();
        }

        protected virtual void OnEnd()
        {
        }

        protected internal virtual void SetFailure()
        {
            _taskStatus = TaskStatus.Failure;
        }
    }

    public abstract class ConditionTask : Task
    {

        protected sealed override void OnStart()
        {
            if (IsPassCondition())
            {
                TaskStatus = TaskStatus.Success;
                return;
            }
            TaskStatus = TaskStatus.Failure;
        }

        protected override void OnEnd()
        {

        }

        protected internal bool CanReceiveEvent(out TaskStatus result){
            var preTaskStatus = TaskStatus;
            result = IsPassCondition() ? TaskStatus.Success : TaskStatus.Failure;
            return preTaskStatus != result;
        }

        protected internal virtual bool IsPassCondition(){
            return false;
        }
    }

    public class ActionTask : Task
    {
        public ActionTask() : base(){
            
        }

        protected override void OnStart()
        {
            //TaskStatus = TaskStatus.Running;
            //TaskStatus = TaskStatus.Success;
        }

        protected override void OnUpdate()
        {
            //TaskStatus = TaskStatus.Success;
        }

        protected override void OnEnd()
        {

        }
    }

    public class ParentTask : Task{
        public ParentTask() : base()
        {
        }

        protected List<Task> _tasks = new List<Task>();
        public List<Task> Tasks {
            get{
                return _tasks;
            }
            set{
                _tasks = value;
            }
        }
        protected List<ParentTask> _parentTaskChildren = new List<ParentTask>();
        protected Task _currentTask = null;
        protected int _currentTaskIndex = 0;
        protected internal AbortType _abortType;

        protected List<ConditionTask> _self = new List<ConditionTask>();
        protected List<ConditionTask> _lowerPriority = new List<ConditionTask>();

        protected override List<Task> GetChildren()
        {
            return _tasks;
        }

        public virtual void AddChildTask(Task task)
        {
            _tasks.Clear();
            _tasks.Add(task);
        }

        protected internal void AddChildIsParentTask(ParentTask child){
            _parentTaskChildren.Add(child);
        }

        protected override void OnStart()
        {
            if (_tasks.Count == 0)
                throw new System.Exception("ParentTask Tasks have to have task child");
            _currentTaskIndex = 0;
            _currentTask = null;

            CheckTask();
        }

        protected override void OnRunning()
        {
            // receive condition task
            if (_currentTask != null)
            {
                for (int i = 0; i < _lowerPriority.Count; i++)
                {
                    var lower = _lowerPriority[i];
                    if (lower._priority > _currentTask._priority)
                    {
                        _lowerPriority.RemoveAt(i--);
                        _tree._receiveTasks.Remove(lower);
                    }
                }
                ChangeLowerPriorityToSelf();
            }
            //
        }

        protected override void OnUpdate()
        {
            //Debug.Log("Selector Name OnUpdate: " + Name + ";" + _currentTaskIndex + ";" + TaskStatus);
            if (_currentTask != null)
            {
                _currentTask.Update();
            }
        }

        protected virtual void CheckTask(){
            if (_currentTaskIndex >= _tasks.Count)
            {
                _currentTaskIndex = 0;
                _currentTask = null;
                TaskStatus = TaskStatus.Success;
                return;
            }
            _currentTask = _tasks[_currentTaskIndex];
            var conditionTask = _currentTask as ConditionTask;
            if (conditionTask != null)
            {
                CheckAddReceiveTaskSelf(conditionTask);
            }
            _currentTask.Start();
        }

		protected override void OnEnd()
		{
            if (_abortType == AbortType.None) return;
            if (_currentTask != null)
            {
                for (int i = 0; i < _self.Count; i++)
                {
                    var self = _self[i];
                    if (self._priority > _currentTask._priority)
                    {
                        _self.RemoveAt(i--);
                        _tree._receiveTasks.Remove(self);
                    }
                }
            }


            ChangeSelfToLowerPriority();
		}

        protected internal virtual void ChildSuccess(Task task)
        {
            _currentTaskIndex = _tasks.IndexOf(task);
            _currentTaskIndex++;
            CheckTask();
        }

        protected internal virtual void ChildFailure(Task task)
        {
            _currentTaskIndex = _tasks.IndexOf(task);
            _currentTask = task;
            TaskStatus = TaskStatus.Failure;
        }

        protected void CheckAddReceiveTaskSelf(ConditionTask conditionTask)
        {
            if (_abortType == AbortType.None) return;

            if(_self.Contains(conditionTask)){
                return;
            }
            _self.Add(conditionTask);
            if (_abortType == AbortType.Self || _abortType == AbortType.Both)
            {
                if (conditionTask.IsPassCondition() &&!_tree._receiveTasks.Contains(conditionTask)){
                    _tree._receiveTasks.Add(conditionTask);
                }

            }
        }

        protected void ChangeLowerPriorityToSelf()
        {
            for (int i = 0; i < _lowerPriority.Count; i++)
            {
                var task = _lowerPriority[i];
                if (!_self.Contains(task))
                    _self.Add(task);
                _tree._receiveTasks.Remove(task);
            }
            _lowerPriority.Clear();
            if (_abortType == AbortType.Self || _abortType == AbortType.Both)
            {
                for (int i = 0; i < _self.Count; i++)
                {
                    var task = _self[i];
                    //if (task.IsPassCondition() && !_tree._receiveTasks.Contains(task))
                    //{
                    //    _tree._receiveTasks.Add(task);
                    //}
                    if (!_tree._receiveTasks.Contains(task))
                    {
                        _tree._receiveTasks.Add(task);
                    }
                }
            }
        }

        protected void ChangeSelfToLowerPriority()
        {
            for (int i = 0; i < _self.Count; i++)
            {
                var task = _self[i];
                if(!_lowerPriority.Contains(task))
                    _lowerPriority.Add(task);
                _tree._receiveTasks.Remove(task);
            }
            _self.Clear();
            if(TaskStatus == TaskStatus.Failure){
                if (_abortType == AbortType.LowerPriority || _abortType == AbortType.Both)
                {
                    for (int i = 0; i < _lowerPriority.Count; i++)
                    {
                        var task = _lowerPriority[i];
                        if (!_tree._receiveTasks.Contains(task))
                        {
                            _tree._receiveTasks.Add(task);
                        }
                    }
                }
            }
        }

        protected internal void CheckRemoveReceiveEventOfCompositeChildren(Task task)
        {
            List<ParentTask> parentTasks = new List<ParentTask>();
            for (int i = 0; i < _parentTaskChildren.Count; i++)
            {
                var parentTask = _parentTaskChildren[i];
                if (parentTask.TaskStatus == TaskStatus.Inactive) continue;

                if (parentTask._priority > task._priority)
                {
                    parentTasks.Add(parentTask);
                }
            }
            RemoveReceiveEventOfChildren(parentTasks);
        }

        protected internal void RemoveReceiveEventOfChildren(List<ParentTask> parentTasks)
        {
            for (int i = 0; i < parentTasks.Count; i++)
            {
                var parentTask = parentTasks[i];
                if (parentTask.TaskStatus == TaskStatus.Inactive) continue;

                parentTask.SetFailure();
            }
        }

        protected internal void RunningWhenReceiveChild(Task currentTask)
        {
            if(_currentTask != null){
                CheckRemoveReceiveEventOfCompositeChildren(_currentTask);
            }
            var currentTaskStatus = TaskStatus;
            _currentTaskIndex = _tasks.IndexOf(currentTask);
            _currentTask = currentTask;
            TaskStatus = TaskStatus.Running;
            ChangeToRunningImmediately();
            if (currentTaskStatus == TaskStatus.Failure
                || currentTaskStatus == TaskStatus.Success
               )
            {
                if (Parent != null)
                {
                    Parent.RunningWhenReceiveChild(this);
                }
            } else if(currentTaskStatus == TaskStatus.Running){
                Update();
            }
        }

        protected internal override void SetFailure()
        {
            _taskStatus = TaskStatus.Failure;
            var parentTask = _tasks[0] as ParentTask;
            if(parentTask != null){
                parentTask.SetFailure();
            }
        }

        internal virtual void CheckRecieveEvent(ConditionTask receiveEvent, TaskStatus taskStatus)
        {
            RunningWhenReceiveChild(receiveEvent);
            CheckRemoveReceiveEventOfCompositeChildren(receiveEvent);
            receiveEvent.TaskStatus = taskStatus;
        }
	}

    public abstract class CompositeTask : ParentTask
    {
        public AbortType AbortType {
            get{
                return _abortType;
            }
            set{
                _abortType = value;
            }
        }

        protected sealed override void OnStart()
        {
            if (_tasks.Count == 0)
                throw new System.Exception("CompositeTask Tasks have to have task child");
            _currentTaskIndex = 0;
            _currentTask = null;
            CheckTask();
        }

		public override void AddChildTask(Task task)
        {
            _tasks.Add(task);
        }

        protected internal sealed override void SetFailure(){
            _taskStatus = TaskStatus.Failure;
            if (AbortType != AbortType.None){
                for (int i = 0; i < _self.Count; i++)
                {
                    var task = _self[i];
                    _tree._receiveTasks.Remove(task);
                }
                for (int i = 0; i < _lowerPriority.Count; i++)
                {
                    var task = _lowerPriority[i];
                    _tree._receiveTasks.Remove(task);
                }
            }

            RemoveReceiveEventOfChildren(_parentTaskChildren);
        }
	}


    public sealed class SequenceTask : CompositeTask
    {
        public SequenceTask() : base()
        {

        }

		internal Task GetBrotherOfTask(Task task){
            var index = _tasks.IndexOf(task);
            if(index == 0){
                return null;
            }
            return _tasks[index - 1];
        }
    }

    public sealed class SelectorTask : CompositeTask
    {
        public SelectorTask() : base()
        {

        }

        protected override void CheckTask()
        {
            if (_currentTaskIndex >= _tasks.Count)
            {
                _currentTaskIndex = 0;
                _currentTask = null;
                TaskStatus = TaskStatus.Failure;
                return;
            }
            _currentTask = _tasks[_currentTaskIndex];
            var conditionTask = _currentTask as ConditionTask;
            if (conditionTask != null)
            {
                CheckAddReceiveTaskSelf(conditionTask);
            }
            _currentTask.Start();
        }

        protected internal override void ChildSuccess(Task task)
        {
            //Debug.Log("Selector Name ChildSuccess: " + Name + ";" + _currentTaskIndex + ";" + _tasks.IndexOf(task));
            _currentTaskIndex = 0;
            _currentTask = null;
            TaskStatus = TaskStatus.Success;
        }

        protected internal override void ChildFailure(Task task)
        {
            _currentTaskIndex = _tasks.IndexOf(task);
            _currentTaskIndex++;
            //Debug.Log("Selector Name ChildFailure: " + Name + ";" + _currentTaskIndex + ";" + _tasks.IndexOf(task));
            CheckTask();
        }
    }

    public sealed class ParallelTask : CompositeTask
    {
        public ParallelTask() : base()
        {

        }

        private bool _usedTo = false;
        protected override void CheckTask()
        {
            if(!_usedTo){
                _usedTo = true;
                for (int i = 0; i < _tasks.Count; i++)
                {
                    var task = _tasks[i];
                    var conditionTask = task as ConditionTask;
                    if (conditionTask != null)
                    {
                        CheckAddReceiveTaskSelf(conditionTask);
                    }
                }
            }
            for (int i = 0; i < _tasks.Count; i++){
                var task = _tasks[i];
                task.Start();
            }
        }

		protected override void OnRunning()
		{
            if (!_usedTo) return;
            ChangeLowerPriorityToSelf();
		}

		protected override void OnUpdate()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                var task = _tasks[i];
                task.Update();
            }
        }

        protected internal override void ChildSuccess(Task task)
        {
            int countSuccess = _tasks.Count(x => x.TaskStatus == TaskStatus.Success);
            if(countSuccess == _tasks.Count)
            {
                TaskStatus = TaskStatus.Success;
            }
        }

        protected internal override void ChildFailure(Task task)
        {
            TaskStatus = TaskStatus.Failure;
        }

        internal override void CheckRecieveEvent(ConditionTask receiveEvent, TaskStatus taskStatus)
        {
            RunningWhenReceiveChild(receiveEvent);
            receiveEvent.TaskStatus = taskStatus;
        }

		protected override void OnEnd()
		{
            if (AbortType == AbortType.None) return;
            ChangeSelfToLowerPriority();
            if(TaskStatus == TaskStatus.Failure){
                for (int i = 0; i < _tasks.Count; i++){
                    var task = _tasks[i];
                    if (task.TaskStatus == TaskStatus.Inactive) continue;
                    if (task as ConditionTask != null) continue;

                    task.SetFailure();
                }
            }
		}
	}

    public sealed class ParallelSelectorTask : CompositeTask
    {
        public ParallelSelectorTask() : base()
        {

        }

        private bool _usedTo = false;
        protected override void CheckTask()
        {
            if (!_usedTo)
            {
                _usedTo = true;
                for (int i = 0; i < _tasks.Count; i++)
                {
                    var task = _tasks[i];
                    var conditionTask = task as ConditionTask;
                    if (conditionTask != null)
                    {
                        CheckAddReceiveTaskSelf(conditionTask);
                    }
                }
            }
            for (int i = 0; i < _tasks.Count; i++)
            {
                var task = _tasks[i];
                task.Start();
            }
        }

        protected override void OnRunning()
        {
            if (!_usedTo) return;
            ChangeLowerPriorityToSelf();
        }

        protected override void OnUpdate()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                var task = _tasks[i];
                task.Update();
            }
        }

        protected internal override void ChildSuccess(Task task)
        {
            TaskStatus = TaskStatus.Success;
        }

        protected internal override void ChildFailure(Task task)
        {
            int countFail = _tasks.Count(x => x.TaskStatus == TaskStatus.Failure);
            if (countFail == _tasks.Count)
            {
                TaskStatus = TaskStatus.Failure;
            }
        }

        internal override void CheckRecieveEvent(ConditionTask receiveEvent, TaskStatus taskStatus)
        {
            RunningWhenReceiveChild(receiveEvent);
            receiveEvent.TaskStatus = taskStatus;
        }

        protected override void OnEnd()
        {
            if (AbortType == AbortType.None) return;
            ChangeSelfToLowerPriority();
        }
    }

    #region action
    public sealed class LogTask : ActionTask{
        public ShareStringBehavior Value { get; set; }
        public LogTask() : base()
        {
            Value = new ShareStringBehavior();
            Value.Value = "";
        }

        protected override void OnStart()
        {
            Debug.Log("LogTask OnStart: " + Value.Value + ";" + Time.time + ";" + Id);
            TaskStatus = TaskStatus.Success;
        }

        protected override void OnUpdate()
        {
            
        }

        protected override void OnEnd()
        {
        }
    }
    public sealed class WaitingTimeTask : ActionTask
    {
        public ShareFloatBehavior Time { get; set; }
        public ShareBoolBehavior Random { get; set; }
        public ShareFloatBehavior FromTime { get; set; }
        public ShareFloatBehavior ToTime { get; set; }
        private float _time = 0;
        public WaitingTimeTask() : base()
        {
            Time = new ShareFloatBehavior();
            Time.Value = 1;
            Random = new ShareBoolBehavior();
            FromTime = new ShareFloatBehavior();
            ToTime = new ShareFloatBehavior();
        }
        float _currentTime = 0;

		protected override void OnStart()
		{
            _currentTime = 0;
            _time = FloatValue.Round(Random.Value ? (_tree.RandomRange(FromTime.Value, ToTime.Value)) : Time.Value);
            if(_time <= 0)
            {
                TaskStatus = TaskStatus.Success;
            }
        }

		protected override void OnUpdate()
        {
            _currentTime += TickInterval;
            if (_currentTime >= _time)
            {
                TaskStatus = TaskStatus.Success;
            }
        }

        protected override void OnEnd()
        {
            _currentTime = 0;
        }
    }

    public class SendEventTask : ActionTask
    {
        public ShareStringBehavior Key { get; set; }

        public SendEventTask() : base()
        {
            Key = new ShareStringBehavior();
        }

        public SendEventTask(string key) : base()
        {
            Key = new ShareStringBehavior();
            Key.Value = key;
        }

        protected override void OnStart()
        {
            _tree.SendEvent(Key.Value);
            TaskStatus = TaskStatus.Success;
        }
    }
    #endregion

    #region conditional
    public class HasReceiveEventTask : ConditionTask
    {
        public ShareStringBehavior Key { get; set; }
        protected internal bool HasReceive = false;
        public HasReceiveEventTask() : base(){
            Key = new ShareStringBehavior();
        }

        public HasReceiveEventTask(string key) : base()
        {
            Key = new ShareStringBehavior();
            Key.Value = key;
        }

        protected internal override bool IsPassCondition()
        {
            var result = HasReceive;
            HasReceive = false;
            //if (result)
            //{
            //    Debug.Log(Key + ": " + result + ";" + Time.time);
            //}
            return result;
        }
    }
    #endregion

    #region condition task
    public sealed class CompareWithShareIntTask : ConditionTask
    {
        public ShareIntBehavior ShareValue { get; set; }
        public ShareIntBehavior CompareValue { get; set; }

        public CompareWithShareIntTask() : base(){
            ShareValue = new ShareIntBehavior();
            CompareValue = new ShareIntBehavior();
        }

        protected override void OnEnd()
        {

        }

        protected internal override bool IsPassCondition()
        {
            if (ShareValue.Value == CompareValue.Value)
            {
                return true;
            }
            return false;
        }
    }

    public sealed class CompareWithShareBoolTask : ConditionTask
    {
        public ShareBoolBehavior ShareValue { get; set; }
        public ShareBoolBehavior CompareValue { get; set; }

        public CompareWithShareBoolTask() : base()
        {
            ShareValue = new ShareBoolBehavior();
            CompareValue = new ShareBoolBehavior();
        }

        public CompareWithShareBoolTask(bool compareValue) : base()
        {
            ShareValue = new ShareBoolBehavior();
            CompareValue = new ShareBoolBehavior();
            CompareValue.Value = compareValue;
        }

        protected override void OnEnd()
        {

        }

        protected internal override bool IsPassCondition()
        {
            if (ShareValue.Value == CompareValue.Value)
            {
                return true;
            }
            return false;
        }
    }

    public sealed class CompareWithShareFloatTask : ConditionTask
    {
        public ShareFloatBehavior ShareValue { get; set; }
        public ShareFloatBehavior CompareValue { get; set; }

        public CompareWithShareFloatTask() : base()
        {
            ShareValue = new ShareFloatBehavior();
            CompareValue = new ShareFloatBehavior();
        }

        protected override void OnEnd()
        {

        }

        protected internal override bool IsPassCondition()
        {
            if (ShareValue.Value == CompareValue.Value)
            {
                return true;
            }
            return false;
        }
    }

    public sealed class CompareWithShareVector3Task : ConditionTask
    {
        public ShareVector3Behavior ShareValue { get; set; }
        public ShareVector3Behavior CompareValue { get; set; }

        public CompareWithShareVector3Task() : base()
        {
            ShareValue = new ShareVector3Behavior();
            CompareValue = new ShareVector3Behavior();
        }

        protected override void OnEnd()
        {

        }

        protected internal override bool IsPassCondition()
        {
            if (ShareValue.Value == CompareValue.Value)
            {
                return true;
            }
            return false;
        }
    }

    public sealed class CompareWithShareTransformViewModelTask : ConditionTask
    {
        public ShareTransformViewModelBehavior ShareValue { get; set; }
        public ShareTransformViewModelBehavior CompareValue { get; set; }

        public CompareWithShareTransformViewModelTask() : base()
        {
            ShareValue = new ShareTransformViewModelBehavior();
            CompareValue = new ShareTransformViewModelBehavior();
        }

        protected override void OnEnd()
        {

        }

        protected internal override bool IsPassCondition()
        {
            if (ShareValue.Value == CompareValue.Value)
            {
                return true;
            }
            return false;
        }
    }
    #endregion

    #region parent
    public sealed class InvertTask : ParentTask
    {
        public InvertTask() : base()
        {

        }

        protected internal override void ChildSuccess(Task task)
        {
            TaskStatus = TaskStatus.Failure;
        }

        protected internal override void ChildFailure(Task task)
        {
            TaskStatus = TaskStatus.Success;
        }
    }

    public sealed class SuccessTask : ParentTask
    {
        public SuccessTask() : base()
        {

        }

        protected internal override void ChildSuccess(Task task)
        {
            TaskStatus = TaskStatus.Success;
        }

        protected internal override void ChildFailure(Task task)
        {
            TaskStatus = TaskStatus.Success;
        }
    }

    public sealed class FailureTask : ParentTask
    {
        public FailureTask() : base()
        {

        }

        protected internal override void ChildSuccess(Task task)
        {
            TaskStatus = TaskStatus.Failure;
        }

        protected internal override void ChildFailure(Task task)
        {
            TaskStatus = TaskStatus.Failure;
        }
    }

    public sealed class UntilSuccessTask : ParentTask
    {
        public UntilSuccessTask() : base()
        {

        }

        protected internal override void ChildSuccess(Task task)
        {
            TaskStatus = TaskStatus.Success;
        }

        protected internal override void ChildFailure(Task task)
        {
            Start();
        }
    }

    public sealed class RepeatTask : ParentTask
    {
        public ShareIntBehavior Count { get; set; }
        public ShareBoolBehavior RepeatForever { get; set; }
        public ShareBoolBehavior EndOnFailure { get; set; }

        private int _currentCount = 0;
        private bool _repeat = false;
        public RepeatTask() : base(){
            var typename = TypeNameOfModel;
            Count = new ShareIntBehavior();
            RepeatForever = new ShareBoolBehavior();
            EndOnFailure = new ShareBoolBehavior();
        }

		protected override void OnStart()
		{
            _repeat = false;
            // reset tree from repeat
            RemoveReceiveEventOfChildren(_parentTaskChildren);
            //
            base.OnStart();
		}

		public override void AddChildTask(Task task)
		{
            base.AddChildTask(task);
		}

		protected override void OnUpdate()
        {
            if(_repeat){
                Start();
            } else{
                if (_currentTask != null)
                {
                    _currentTask.Update();
                }
            }
        }

		protected override void OnEnd()
		{
            _repeat = false;
            _currentCount = 0;
            base.OnEnd();
		}

        protected internal override void ChildSuccess(Task task)
        {
            if(RepeatForever.Value){
                _repeat = true;
            } else{
                _currentCount++;
                if(_currentCount >= Count.Value){
                    _repeat = false;
                    TaskStatus = TaskStatus.Success;
                } else{
                    _repeat = true;
                }
            }
        }

        protected internal override void ChildFailure(Task task)
        {
            if(EndOnFailure.Value){
                _repeat = false;
                TaskStatus = TaskStatus.Failure;
            }
            else{
                if (RepeatForever.Value)
                {
                    _repeat = true;
                }
                else{
                    _currentCount++;
                    if (_currentCount >= Count.Value)
                    {
                        _repeat = false;
                        TaskStatus = TaskStatus.Failure;
                    }
                    else
                    {
                        _repeat = true;
                    }
                }
            }
        }

		public override void ChangeToRunningImmediately()
		{
            _repeat = false;
		}
	}
    #endregion
}

