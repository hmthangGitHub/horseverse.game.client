using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using FixRound;
using Core.Behavior;

namespace Core.MVVM{
    public class TransformViewModel : TransformViewModelBase
    {
        private bool _isRemove;
        public virtual bool IsRemove
        {
            get
            {
                return _isRemove;
            }
            set
            {
                _isRemove = value;
            }
        }
        bool _isRunning = true;
        public virtual bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                _isRunning = value;
            }
        }
        public virtual bool IsAlive { get; } = true;
        public Vector3 NextTargetPosition { get; set; }
        public float TickInterval { get; set; }
        private BehaviorTree _tree;
        public BehaviorTree Tree {
            get{
                return _tree;
            }
            set{
                _tree = value;
                _tree.SetTransform(this);
            }
        }

        public float Radius { get; set; }

        public Vector3 Direction{
            get{
                return MathCustom.GetDirectionFromEulerAngle(EulerAngles.Value);
            }
            set{
                EulerAngles.Value = MathCustom.GetEulerAngleFromDirect(value);
            }
        }

        #region logic call back in battle
        protected List<WaitingForCallBack> _callBacks = new List<WaitingForCallBack>();
        protected List<WaitingForCallBack> _nextCallBacks = new List<WaitingForCallBack>();

        public IDisposable AddCallBack(Action callBack, bool repeat = false, CallbackInProcessType type = CallbackInProcessType.Running)
        {
            WaitingForCallBack waitingForCallBack = new WaitingForCallBack(0, repeat, type, callBack);
            _callBacks.Add(waitingForCallBack);
            return waitingForCallBack;
        }

        public IDisposable AddCallBack(int numberTick, Action callBack, bool repeat = false, CallbackInProcessType type = CallbackInProcessType.Running)
        {
            WaitingForCallBack waitingForCallBack = new WaitingForCallBack(numberTick, repeat, type, callBack);
            if (numberTick <= 0)
            {
                _callBacks.Add(waitingForCallBack);
            }
            else
            {
                _nextCallBacks.Add(waitingForCallBack);
            }
            return waitingForCallBack;
        }

        public IDisposable AddCallBack(float time, Action callBack, bool repeat = false, CallbackInProcessType type = CallbackInProcessType.Running)
        {
            time = time < 0 ? 0 : time;
            int tick = (int)FloatValue.Round(time / TickInterval);
            //Debug.Log("tick:" + tick + ";" + time + ";" + TickInterval);
            return AddCallBack(tick, callBack, repeat, type);
        }

        public void UpdateCallBack(float tickInterval)
        {
            for (int i = 0; i < _callBacks.Count; i++)
            {
                WaitingForCallBack waitingForCallBack = _callBacks[i];
                if (waitingForCallBack.CallbackInProcess == CallbackInProcessType.All
                    || (waitingForCallBack.CallbackInProcess == CallbackInProcessType.Pause && !IsRunning)
                    || (waitingForCallBack.CallbackInProcess == CallbackInProcessType.Running && IsRunning))
                {
                    waitingForCallBack.Update(tickInterval);
                }
            }
            for (int i = 0; i < _callBacks.Count; i++)
            {
                var callback = _callBacks[i];
                if (callback.IsRemove)
                {
                    _callBacks.RemoveAt(i--);
                }
            }
            UpdateNewCallBack();
        }

        protected void UpdateNewCallBack()
        {
            if (_nextCallBacks.Count > 0)
            {
                _callBacks.AddRange(_nextCallBacks);
                _nextCallBacks.Clear();
            }
        }

        public void ClearAllCallback()
        {
            _callBacks.Clear();
            _nextCallBacks.Clear();
        }
        #endregion

        protected virtual bool CanUpdate
        {
            get
            {
                return true;
            }
        }

        protected virtual bool CanUpdateTick
        {
            get
            {
                return IsRunning;
            }
        }

        protected virtual void BeforeUpdateTick(float tickInterval)
        {

        }

        protected virtual void UpdateTick(float tickInterval)
        {

        }

        protected virtual void LateUpdateTick(float tickInterval)
        {

        }

        public void Update(float tickInterval)
        {
            if (!CanUpdate)
                return;

            BeforeUpdateTick(tickInterval);
            Position.Value = NextTargetPosition;
            if (CanUpdateTick)
            {
                UpdateTick(tickInterval);
                if(Tree != null){
                    Tree.Update();
                }
            }
            UpdateCallBack(tickInterval);
            if (CanUpdateTick)
            {
                LateUpdateTick(tickInterval);
            }
        }

		public override void Pause(bool arg)
		{
            IsRunning = !arg;
		}

        public override void InitViewModel()
        {
            base.InitViewModel();

            DestroyGameObjectCommand.Subscribe(_ =>
            {
                if(Tree != null)
                {
                    BehaviorTreePoolManager.Instance.Add(Tree.Name, Tree);
                }
            });
        }
    }


    #region callback
    public enum CallbackInProcessType
    {
        Running,
        Pause,
        All
    }

    public class WaitingForNextAction
    {
        Action _callback;
        int _count;

        public WaitingForNextAction()
        {
            _count = 0;
        }

        public void SetActionCallBack(Action callback, bool isResetCount = true)
        {
            _callback = callback;
            if (isResetCount)
            {
                _count = 0;
            }
        }

        public Action GetActionCallBack()
        {
            return _callback;
        }

        public void Increase()
        {
            _count++;
            //      UnityEngine.Debug.Log ("Increase " + _count + "----" + _callback);
        }

        public void Decrease()
        {
            _count--;
            //      UnityEngine.Debug.Log ("Decrease " + _count + "----" + _callback);
            if (_count == 0)
            {
                if (_callback != null)
                {
                    Action temp = _callback;
                    _callback = null;
                    temp();
                }
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }
    }

    public class WaitingForCallBack : IDisposable
    {

        private int _currentTick;
        private int _maxTick;
        private Action _callBack;
        private bool _repeat;
        public bool IsRemove { get; private set; }
        public CallbackInProcessType CallbackInProcess { get; private set; }

        protected WaitingForCallBack()
        {
        }

        public WaitingForCallBack(int maxTick, bool repeat, CallbackInProcessType type, Action callback)
        {
            _maxTick = maxTick < 0 ? 0 : maxTick;
            _currentTick = _maxTick;
            _callBack = callback;
            _repeat = repeat;
            CallbackInProcess = type;
            IsRemove = false;
        }

        public void Update(float tickInterval)
        {
            if (IsRemove)
                return;
            _currentTick -= 1;
            CheckInvoke();
        }

        void CheckInvoke()
        {
            if (_currentTick <= 0)
            {
                Invoke();
            }
        }

        public void Invoke()
        {
            if (IsRemove)
                return;
            if (_repeat)
            {
                _currentTick = _maxTick;
            }
            else
            {
                IsRemove = true;
            }
            _callBack();
        }

        #region IDisposable implementation
        public void Dispose()
        {
            IsRemove = true;
        }
        #endregion
    }
    #endregion
}