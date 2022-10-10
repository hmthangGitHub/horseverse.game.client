using PathCreation;
using RobustFSM.Base;
using UnityEngine;

public class HorseTrainingControllerStateBase : BState
{
    private HorseTrainingController horseTrainingController;
    protected static readonly int Jumping = Animator.StringToHash("Jumping");
    protected static readonly int SpeedHash = Animator.StringToHash("Speed");
    protected HorseTrainingController HorseTrainingController => horseTrainingController ??= GetSuperMachine<HorseTrainingController>();
    protected PathCreator PathCreator
    {
        get => HorseTrainingController.pathCreator;
        set => HorseTrainingController.pathCreator = value;
    }

    protected HorseTrainingTrigger HorseTrainingTrigger => HorseTrainingController.horseTrainingTrigger;
    protected Animator Animator => HorseTrainingController.animator;
    protected Transform Transform => HorseTrainingController.transform;
    protected Transform HorseMesh => HorseTrainingController.horseMesh;
    protected HorseTrainingAttribute HorseTrainingAttribute => HorseTrainingController.HorseTrainingAttribute;

    protected HorseTrainingControllerData HorseTrainingControllerData => HorseTrainingController.HorseTrainingControllerData;

    protected void UpdatePosition()
    {
        HorseTrainingController.HorseTrainingControllerData.TotalDistance += HorseTrainingController.HorseTrainingControllerData.Speed * Time.deltaTime;
        var t = (HorseTrainingController.HorseTrainingControllerData.TotalDistance / PathCreator.path.length);
        var rotationAtDistance = PathCreator.path.GetRotation(t);
        Transform.rotation = Quaternion.Euler(0, rotationAtDistance.eulerAngles.y, 0);

        var right = Transform.right;
        var pos = PathCreator.path.GetPointAtTime(t);
        Transform.position = pos + right * HorseTrainingControllerData.CurrentOffset + Transform.up * HorseTrainingControllerData.CurrentHeight;
        UpdateSpeed();
    }
    
    private void UpdateSpeed()
    {
        HorseTrainingControllerData.movingTime += Time.deltaTime;
        HorseTrainingController.HorseTrainingControllerData.Speed = GetDesiredSpeed(HorseTrainingControllerData.movingTime);
    }

    private float GetDesiredSpeed(float time)
    {
        return HorseTrainingAttribute.originalSpeed + (int)((Mathf.Abs(time)) / 60.0f) * HorseTrainingAttribute.originalSpeed * 0.30f;
    }

    protected void UpdateJumpAnimation(bool isJumping)
    {
        if (isJumping)
        {
            Animator.SetBool(Jumping, true);
            Animator.CrossFade(Jumping , 0.1f, 0);    
        }
        else
        {
            Animator.SetBool(Jumping, false);
            Animator.SetBool(Jumping, false);
        }
    }
}