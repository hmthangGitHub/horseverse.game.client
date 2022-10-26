using System;
using System.Linq;
using DG.Tweening;
using PathCreation;
using RobustFSM.Base;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HorseTrainingBridgeState : HorseTrainingControllerStateBase
{
    private float velocity;
    private float targetAngle;
    private float currentAngle;

    private bool canControl;
    
    public override void Enter()
    {
        base.Enter();
        HorseTrainingController.OnBridge();
        
        var currentPos = Transform.position;
        var t = HorseTrainingControllerData.TotalDistance / PathCreator.path.length;
        var time = (PathCreator.path.length - HorseTrainingControllerData.TotalDistance) / HorseTrainingControllerData.Speed;
        canControl = false;
        DOTween.Sequence()
            .Append(DOTween.To(x =>
            {
                UpdatePosition();
            }, 0.0f, 1.0f, time))
            .AppendCallback(() =>
            {
                canControl = true;
                UpdateJumpAnimation(true);
            })
            .Append(DOTween.To(UpdateHorseOnBridgePosition, 0.0f, 0.5f, HorseTrainingAttribute.bridgeTime * 0.5f).SetEase(Ease.Linear))
            .Append(DOTween.To(UpdateHorseOnBridgePosition, 0.5f, 1.0f, HorseTrainingAttribute.bridgeTime * 0.3f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                UpdateJumpAnimation(false);
                
                if (IsLandingOnAPlatform())
                {
                    PathCreator = HorseTrainingControllerData.Bridge.DestinationPath;
                    HorseTrainingControllerData.TotalDistance = 0.0f;
            
                    if (HorseTrainingControllerData.Bridge.DestinationPathType == MeshPathContainer.PathType.Cloud)
                    {
                        Machine.ChangeState<HorseTrainingAirState>();
                    }
                    else
                    {
                        Machine.ChangeState<HorseTrainingAirState>();
                    }
                    HorseTrainingController.OnLanding();
                }
                else
                {
                    Machine.ChangeState<HorseTrainingFallState>();   
                }
            })
            .SetUpdate(UpdateType.Fixed);
    }

    private bool IsLandingOnAPlatform()
    {
#if UNITY_EDITOR
        Debug.DrawRay(HorseTrainingController.transform.position, -Vector3.up, Color.red, 5f);
#endif
        return Physics.RaycastAll(HorseTrainingController.transform.position + Vector3.up, -Vector3.up, 2.5f)
            .Any(x => x.collider.CompareTag("BridgeDestination"));
    }

    private void UpdateHorseOnBridgePosition(float val)
    {
        var pos = HorseTrainingControllerData.Bridge.Bridge.path.GetPointAtTime(val, EndOfPathInstruction.Stop);
        var right = Transform.right;
        Transform.position = pos + right * HorseTrainingControllerData.CurrentOffset;
    }
    
    private void ChangeVelocity(int direction)
    {
        HorseTrainingControllerData.CurrentOffSetInt = direction;
        velocity += HorseTrainingAttribute.offset * HorseTrainingControllerData.CurrentOffSetInt /HorseTrainingAttribute.moveTime;
        targetAngle += HorseTrainingAttribute.flyingAngle * HorseTrainingControllerData.CurrentOffSetInt;
    }
    
    public override void Execute()
    {
        if (!canControl) return;
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
    }

    public override void PhysicsExecute()
    {
        HorseTrainingControllerData.CurrentOffset += this.velocity * Time.deltaTime;
        // HorseTrainingControllerData.CurrentOffset = Mathf.Clamp(HorseTrainingControllerData.CurrentOffset,
        //     -HorseTrainingAttribute.offset,
        //     HorseTrainingAttribute.offset);
        
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * HorseTrainingAttribute.angularSpeed);
        // HorseMesh.localRotation = Quaternion.Euler(0, 0, -currentAngle);
    }

    public override void Exit()
    {
        base.Exit();
        velocity = 0;
        targetAngle = 0;
        currentAngle = 0;
        canControl = false;
    }
}