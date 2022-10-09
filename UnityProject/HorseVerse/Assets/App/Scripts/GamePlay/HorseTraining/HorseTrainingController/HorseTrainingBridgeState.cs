using DG.Tweening;
using PathCreation;
using RobustFSM.Base;
using UnityEngine;

public class HorseTrainingBridgeState : HorseTrainingControllerStateBase
{
    public override void Enter()
    {
        base.Enter();
        HorseTrainingControllerData.OnBeginBridge();
        
        var currentPos = Transform.position;
        var t = HorseTrainingControllerData.TotalDistance / PathCreator.path.length;
        var time = (PathCreator.path.length - HorseTrainingControllerData.TotalDistance) / HorseTrainingControllerData.Speed;
        UpdateOffSet();
        UpdateJumpAnimation(true);
        
        DOTween.Sequence()
            .Append(DOTween.To(x =>
            {
                UpdatePosition();
            }, 0.0f, 1.0f, time))
            .Append(DOTween.To(UpdateHorseOnBridgePosition, 0.0f, 0.5f, HorseTrainingAttribute.bridgeTime * 0.5f).SetEase(Ease.OutQuint))
            .Append(DOTween.To(UpdateHorseOnBridgePosition, 0.5f, 1.0f, HorseTrainingAttribute.bridgeTime * 0.5f).SetEase(Ease.InQuint))
            .OnComplete(() =>
            {
                UpdateJumpAnimation(false);
                PathCreator = HorseTrainingControllerData.Bridge.DestinationPath;
                HorseTrainingControllerData.TotalDistance = 0.0f;

                if (HorseTrainingControllerData.Bridge.DestinationPathType == MeshPathContainer.PathType.Cloud)
                {
                    Machine.ChangeState<HorseTrainingCloudState>();
                }
                else
                {
                    Machine.ChangeState<HorseTrainingCloudState>();
                }
                HorseTrainingControllerData.OnFinishLandingBridge();
            }).SetUpdate(UpdateType.Fixed);
    }

    private void UpdateHorseOnBridgePosition(float val)
    {
        var pos = HorseTrainingControllerData.Bridge.Bridge.path.GetPointAtTime(val, EndOfPathInstruction.Stop);
        Transform.position = pos;
    }

    private void UpdateOffSet()
    {
        HorseTrainingControllerData.CurrentOffSetInt = 0;
        var sourceOffset = HorseTrainingControllerData.CurrentOffset;
        var targetOffSet = HorseTrainingControllerData.CurrentOffSetInt * HorseTrainingAttribute.offset;
    
        DOTween.To(val =>
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

}