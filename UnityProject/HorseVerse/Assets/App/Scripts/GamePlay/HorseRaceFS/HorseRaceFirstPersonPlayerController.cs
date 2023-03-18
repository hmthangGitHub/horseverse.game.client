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
        CalculateRotation(time);
        CalculateVelocity(time);
    }

    private void CalculateVelocity(float time)
    {
        var pointInCurve = TargetGenerator.FromTimeToPoint(time, 0.0f, horseRaceFirstPersonController.PredefinePath.SimplyPath);
        var toPosition = (horseTransform.position - pointInCurve);
        var isGoingToWall = Vector3.Dot(toPosition, horseTransform.right) * horseRaceFirstPersonController.HorizontalDirection > 0;

        var horizontalSpeed = 0.0f;
        horizontalSpeed = Mathf.Abs(toPosition.magnitude) >= horseRaceFirstPersonController.OffsetRange && isGoingToWall
            ? 0
            : horseRaceFirstPersonController.HorizontalSpeed;

        rigidBody.velocity = horseTransform.rotation * new Vector3(horseRaceFirstPersonController.HorizontalDirection * horizontalSpeed, 0, horseRaceFirstPersonController.ForwardSpeed);
    }

    private void CalculateRotation(float time)
    {
        var eulerAnglesY = horseRaceFirstPersonController.PredefinePath.SimplyPath.path.GetRotation(time)
                                                         .eulerAngles.y + 180 * horseRaceFirstPersonController.PredefinePath.Direction;
        horseTransform.rotation = Quaternion.Euler(0, eulerAnglesY, 0);
    }
}
