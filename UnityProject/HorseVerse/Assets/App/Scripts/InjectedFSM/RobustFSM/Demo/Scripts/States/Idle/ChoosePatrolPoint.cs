using Assets.SimpleFSM.Demo.Scripts.States.Patrol;
using RobustFSM.Base;
using UnityEngine;

namespace Assets.SimpleFSM.Demo.Scripts.States.Idle
{
    public class ChoosePatrolPoint : BState
    {
        public override void Enter()
        {
            base.Enter();

            //find a random patrol point
            Transform temp = Owner.PatrolPoints[Random.Range(0, Owner.PatrolPoints.Count)];

            //check if point is valid
            do
            {
                temp = Owner.PatrolPoints[Random.Range(0, Owner.PatrolPoints.Count)];
            }
            while (temp == Owner.Target);

            //update target and go to patrol
            Owner.Target = temp;
            SuperFSM.ChangeState<PatrolMainState>();
        }

        #region Properties
        ///=================================================================================================
        /// <summary>   Gets the super fsm. </summary>
        ///
        /// <value> The super fsm. </value>
        ///=================================================================================================

        public CharacterFSM SuperFSM
        {
            get
            {
                return (CharacterFSM)SuperMachine;
            }
        }

        ///=================================================================================================
        /// <summary>   Gets the owner. </summary>
        ///
        /// <value> The owner. </value>
        ///=================================================================================================

        public Character Owner
        {
            get
            {
                return SuperFSM.Owner;
            }
        }
        #endregion
    }
}
