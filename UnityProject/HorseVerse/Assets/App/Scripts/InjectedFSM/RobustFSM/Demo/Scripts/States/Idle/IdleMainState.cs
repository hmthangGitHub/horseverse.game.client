using Assets.RobustFSM.Mono;
using RobustFSM.Base;
using System;
using UnityEngine;

namespace Assets.SimpleFSM.Demo.Scripts.States.Idle
{
    public class IdleMainState : BHState
    {
        /// <summary>
        /// A reference to the manual execute frequency
        /// </summary>
        float _manualExecuteRate = 0f;

        /// <summary>
        /// A reference to the previous manual execute time
        /// </summary>
        DateTime _prevManualExecuteTime;

        /// <summary>
        /// Overriden add states method
        /// </summary>
        public override void AddStates()
        {
            AddState<ChoosePatrolPoint>();
            AddState<SleepSubState>();

            SetInitialState<SleepSubState>();
        }

        public override void Enter()
        {
            base.Enter();

            //set the previous manual execute time
            _prevManualExecuteTime = DateTime.Now;

            //set the custom update frequency
            ((MonoFSM)SuperMachine).SetUpdateFrequency(0.1f);
            _manualExecuteRate = ((MonoFSM)SuperMachine).ManualUpdateFrequency;
        }

        public override void ManualExecute()
        {
            base.ManualExecute();

            //find the time difference
            double timeDiff = DateTime.Now.Subtract(_prevManualExecuteTime).TotalSeconds;

            //prepare the message
            string message = string.Format("{0}::{1}::{2}", "<color=red>Invoke idle main state manual execute.</color>", _manualExecuteRate, timeDiff);
            Debug.Log(message);

            //set the previous manual execute time
            _prevManualExecuteTime = DateTime.Now;
        }

        /// <summary>
        /// Retrives the super state machine
        /// </summary>
        public CharacterFSM OwnerFSM
        {
            get
            {
                return (CharacterFSM)SuperMachine;
            }
        }

        /// <summary>
        /// A little extra stuff. Accessing info inside the OwnerFSM
        /// </summary>
        public Character Owner
        {
            get
            {
                return OwnerFSM.Owner;
            }
        }
    }
}
