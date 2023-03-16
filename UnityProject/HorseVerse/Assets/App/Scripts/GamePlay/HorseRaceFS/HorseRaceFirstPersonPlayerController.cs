using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRaceFirstPersonPlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private HorseRaceFirstPersonController horseRaceFirstPersonController;
    [SerializeField] private Transform horseTransform;
    
    public void MoveHorizontal(int direction)
    {
        if (!horseRaceFirstPersonController.IsStart) return;
        horseRaceFirstPersonController.HorizontalDirection += direction;
        horseRaceFirstPersonController.HorizontalDirection = Mathf.Clamp(horseRaceFirstPersonController.HorizontalDirection, -1, 1);
    }

    private void FixedUpdate()
    {
        UpdateHorsePositionRotation();
        CheckIfFinish();
    }

    private void CheckIfFinish()
    {
    }

    private void UpdateHorsePositionRotation()
    {
        var time = horseRaceFirstPersonController.PredefinePath.SimplyPath.path.GetClosestTimeOnPath(horseTransform.position);
        var eulerAnglesY = horseRaceFirstPersonController.PredefinePath.SimplyPath.path.GetRotation(time).eulerAngles.y + 180 * horseRaceFirstPersonController.PredefinePath.Direction;
        horseTransform.rotation = Quaternion.Euler(0, eulerAnglesY, 0);

        var point = TargetGenerator.FromTimeToPoint(time, horseRaceFirstPersonController.Offset, horseRaceFirstPersonController.PredefinePath.SimplyPath);
        var velocity = (horseTransform.position - point);
        var dot = Vector3.Dot(velocity, horseTransform.right);

        var horizontalSpeed = 0.0f;
        if (Mathf.Abs(velocity.magnitude) > horseRaceFirstPersonController.OffsetRange && dot * horseRaceFirstPersonController.HorizontalDirection > 0)
        {
            horizontalSpeed = 0;
        }
        else
        {
            horizontalSpeed = horseRaceFirstPersonController.HorizontalSpeed;
        }
        
        rigidBody.velocity = horseTransform.rotation * new Vector3(horseRaceFirstPersonController.HorizontalDirection * horizontalSpeed, 0, horseRaceFirstPersonController.ForwardSpeed);
    }
}
