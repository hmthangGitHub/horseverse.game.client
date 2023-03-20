using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRaceFirstPersonPlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private HorseRaceThirdPersonBehaviour horseRaceThirdPersonBehaviour;
    [SerializeField] private Transform horseTransform;
    
    public void MoveHorizontal(int direction)
    {
        if (!horseRaceThirdPersonBehaviour.IsStart) return;
        horseRaceThirdPersonBehaviour.HorizontalDirection += direction;
        horseRaceThirdPersonBehaviour.HorizontalDirection = Mathf.Clamp(horseRaceThirdPersonBehaviour.HorizontalDirection, -1, 1);
    }

    private void FixedUpdate()
    {
        if (!horseRaceThirdPersonBehaviour.IsStart) return;
        
        UpdateHorsePositionRotation();
        CheckIfFinish();
    }

    private void CheckIfFinish()
    {
    }

    private void UpdateHorsePositionRotation()
    {
        var time = horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefinePath.SimplyPath.path.GetClosestTimeOnPath(horseTransform.position);
        CalculateRotation(time);
        CalculateVelocity(time);
    }

    private void CalculateVelocity(float time)
    {
        var pointInCurve = TargetGenerator.FromTimeToPoint(time, 0.0f, horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefinePath.SimplyPath);
        var toPosition = (horseTransform.position - pointInCurve);
        var isGoingToWall = Vector3.Dot(toPosition, horseTransform.right) * horseRaceThirdPersonBehaviour.HorizontalDirection > 0;

        var horizontalSpeed = 0.0f;
        horizontalSpeed = Mathf.Abs(toPosition.magnitude) >= horseRaceThirdPersonBehaviour.OffsetRange && isGoingToWall
            ? 0
            : horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.HorseRaceThirdPersonStats.HorizontalSpeed;

        rigidBody.velocity = horseTransform.rotation * new Vector3(horseRaceThirdPersonBehaviour.HorizontalDirection * horizontalSpeed, 0, horseRaceThirdPersonBehaviour.CurrentForwardSpeed);
    }

    private void CalculateRotation(float time)
    {
        var eulerAnglesY = horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefinePath.SimplyPath.path.GetRotation(time)
                                                         .eulerAngles.y + 180 * horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefinePath.Direction;
        horseTransform.rotation = Quaternion.Euler(0, eulerAnglesY, 0);
    }

    public void Sprint()
    {
        horseRaceThirdPersonBehaviour.Sprint();
    }
}
