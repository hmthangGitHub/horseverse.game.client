using DG.Tweening;
using UnityEngine;

public class HorseTrainingCloudState : HorseTrainingControllerStateBase
{
    private Tween currentMovingTween;
    private Tween currentJumpingTween;

    public override void Enter()
    {
        base.Enter();

        var transformPosition = HorseTrainingController.transform.position;
        var point = PathCreator.path.GetPointAtTime(PathCreator.path.GetClosestDistanceAlongPath(transformPosition));
        HorseTrainingControllerData.CurrentOffset = -(point - transformPosition).x;
        HorseTrainingController.HorseTrainingControllerData.Speed = HorseTrainingController.HorseTrainingControllerData.Speed == 0 ? HorseTrainingAttribute.originalSpeed : HorseTrainingController.HorseTrainingControllerData.Speed;
        Animator.SetFloat(SpeedHash, 1.0f);
    }

    private void JumpRight()
    {
        if (!ValidateBeforeMove(1)) return;
        UpdateOffSet();
    }

    private void Jump()
    {
        if (currentJumpingTween?.IsPlaying() == true) return;
        UpdateHeight();
        UpdateJumpAnimation(true);
    }

    private void UpdateHeight()
    {
        currentJumpingTween?.Kill();

        currentJumpingTween = DOTween.To(val =>
        {
            HorseTrainingControllerData.CurrentHeight = Mathf.Lerp(0, HorseTrainingController.HorseTrainingAttribute.jumpHeight, val);
        }, 0.0f, 1.0f, HorseTrainingController.HorseTrainingAttribute.regularJumpTime)
        .SetEase(Ease.OutCubic)
        .SetLoops(2, loopType: LoopType.Yoyo)
        .OnComplete(() => UpdateJumpAnimation(false));
    }

    private bool ValidateBeforeMove(int move)
    {
        if (currentMovingTween == default || currentMovingTween.IsPlaying() == false)
        {
            HorseTrainingControllerData.CurrentOffSetInt += move;
            if (HorseTrainingControllerData.CurrentOffSetInt < -1 || HorseTrainingControllerData.CurrentOffSetInt > 1)
            {
                HorseTrainingControllerData.CurrentOffSetInt = Mathf.Clamp(HorseTrainingControllerData.CurrentOffSetInt, -1, 1);
                return false;
            }
            return true;
        }
        return false;
    }

    private void JumpLeft()
    {
        if (!ValidateBeforeMove(-1)) return;
        UpdateOffSet();
    }

    private void UpdateOffSet()
    {
        var sourceOffset = HorseTrainingControllerData.CurrentOffset;
        var targetOffSet = HorseTrainingControllerData.CurrentOffSetInt * HorseTrainingAttribute.offset;
    
        currentMovingTween?.Kill();
        currentMovingTween = DOTween.To(val =>
        {
            HorseTrainingControllerData.CurrentOffset = Mathf.Lerp(sourceOffset, targetOffSet, val);
        }, 0.0f, 1.0f, HorseTrainingAttribute.moveTime).SetEase(Ease.InOutQuart);

        HorseMesh.DOLocalRotate(new Vector3(0, 40.0f * Mathf.Sign(targetOffSet - sourceOffset), 0), HorseTrainingAttribute.moveTime * 0.5f)
            .SetEase(Ease.InBack)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                HorseMesh.localRotation = Quaternion.identity;
            });
    }

    public override void Execute()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            JumpLeft();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            JumpRight();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    public override void PhysicsExecute()
    {
        UpdatePosition();        
    }
}
