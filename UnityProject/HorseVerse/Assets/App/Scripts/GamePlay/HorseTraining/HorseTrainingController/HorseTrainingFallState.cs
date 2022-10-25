    public class HorseTrainingFallState : HorseTrainingControllerStateBase
    {
        public override void Enter()
        {
            base.Enter();
            HorseTrainingController.OnFall();
            HorseTrainingController.SetEnablePhysicController(true);
        }
    }
