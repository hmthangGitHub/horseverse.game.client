using Assets.SimpleFSM.Demo.Scripts.States.Idle;
using Assets.SimpleFSM.Demo.Scripts.States.Patrol;
using Assets.RobustFSM.Mono;

namespace Assets.SimpleFSM.Demo.Scripts
{
    public class CharacterFSM : MonoFSM
    {
        public Character Owner { get; set; }

        public override void AddStates()
        {
            //set the custom update frequenct
            SetUpdateFrequency(0.1f);

            //add the states
            AddState<IdleMainState>();
            AddState<PatrolMainState>();

            //set the initial state
            SetInitialState<IdleMainState>();
        }
    }
}
