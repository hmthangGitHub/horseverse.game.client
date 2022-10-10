using RobustFSM.Base;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HorseTrainingAirState : HorseTrainingControllerStateBase
{
    private float velocity;
    private float targetAngle;
    private float currentAngle;
    private static readonly int Flying = Animator.StringToHash("Flying");

    public override void Enter()
    {
        base.Enter();
        Animator.SetBool(Flying, true);
        Animator.CrossFade(Flying, 0.25f, 0);
    }

    public override void Execute()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeVelocity(-1);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            ChangeVelocity(1);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ChangeVelocity(1);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            ChangeVelocity(-1);
        }
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Jump();
        }
    }
    
    private void ChangeVelocity(int direction)
    {
        HorseTrainingControllerData.CurrentOffSetInt = direction;
        velocity += HorseTrainingAttribute.offset * HorseTrainingControllerData.CurrentOffSetInt /HorseTrainingAttribute.moveTime;
        targetAngle += HorseTrainingAttribute.flyingAngle * HorseTrainingControllerData.CurrentOffSetInt;
    }

    public override void PhysicsExecute()
    {
        HorseTrainingControllerData.CurrentOffset += this.velocity * Time.deltaTime;
        HorseTrainingControllerData.CurrentOffset = Mathf.Clamp(HorseTrainingControllerData.CurrentOffset, 
            -HorseTrainingAttribute.offset, 
            HorseTrainingAttribute.offset);
        
        UpdatePosition();

        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * HorseTrainingAttribute.angularSpeed);
        HorseMesh.localRotation = Quaternion.Euler(0, 0, -currentAngle);
    }

    public override void Exit()
    {
        base.Exit();
        velocity = 0.0f;
        targetAngle = 0.0f;
        currentAngle = 0.0f;
        Animator.SetBool(Flying, false);
    }
}