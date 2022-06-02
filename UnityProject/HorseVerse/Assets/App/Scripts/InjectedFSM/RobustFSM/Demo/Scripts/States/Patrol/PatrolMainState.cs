using Assets.RobustFSM.Mono;
using Assets.SimpleFSM.Demo.Scripts.States.Idle;
using RobustFSM.Base;
using System;
using UnityEngine;

namespace Assets.SimpleFSM.Demo.Scripts.States.Patrol
{
    public class PatrolMainState : BState
    {
        /// <summary>
        /// A reference to the manual execute frequency
        /// </summary>
        float _manualExecuteRate = 0f;

        /// <summary>
        /// A reference to the previous manual execute time
        /// </summary>
        DateTime _prevManualExecuteTime;

        public override void Enter()
        {
            base.Enter();

            //set the previous manual execute time
            _prevManualExecuteTime = DateTime.Now;

            //set the custom update frequency
            ((MonoFSM)SuperMachine).SetUpdateFrequency(1f);
            _manualExecuteRate = ((MonoFSM)SuperMachine).ManualUpdateFrequency;
        }

        public override void Execute()
        {
            base.Execute();

            //find direction to target
            Vector3 targetDir = Owner.Target.position - Owner.transform.position;
            targetDir.y = 0f;

            //find target rot
            Quaternion targetRot = Quaternion.LookRotation(targetDir);

            //look at target
            Owner.transform.rotation = Quaternion.Lerp(Owner.transform.rotation, 
                targetRot, 
                Owner.Speed * Time.deltaTime);

            //move towards target
            Owner.transform.position = Vector3.MoveTowards(Owner.transform.position, 
                Owner.Target.position, 
                Owner.Speed * Time.deltaTime);

            //go to idle state if we have reached our target
            if (Vector3.Distance(Owner.transform.position, Owner.Target.position) <= 0.1f)
                SuperMachine.ChangeState<IdleMainState>();
        }

        public override void ManualExecute()
        {
            base.ManualExecute();

            //find the time difference
            double timeDiff = DateTime.Now.Subtract(_prevManualExecuteTime).TotalSeconds;

            //prepare the message
            string message = string.Format("{0}::{1}::{2}", "<color=green>Invoke patrol main state manual execute.</color>", _manualExecuteRate, timeDiff);
            Debug.Log(message);

            //set the previous manual execute time
            _prevManualExecuteTime = DateTime.Now;
        }

        /// <summary>
        /// Retries the FSM that owns this state
        /// </summary>
        public CharacterFSM SuperFSM
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
                return SuperFSM.Owner;
            }
        }
    }
}
