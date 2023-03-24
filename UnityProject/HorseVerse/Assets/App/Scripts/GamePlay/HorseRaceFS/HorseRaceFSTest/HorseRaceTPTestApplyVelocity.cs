using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRaceTPTestApplyVelocity : MonoBehaviour
{
    public Vector3 velocity;
    public Rigidbody rigidbody;
    public Transform horseTransform;
    public TargetGenerator targetGenerator;

    private void Update()
    {
        UpdateHorsePositionRotation();
    }

    private void Reset()
    {
        rigidbody = GetComponent<Rigidbody>();
        horseTransform = GetComponent<Transform>();
    }
    
    private void UpdateHorsePositionRotation()
    {
        var time = targetGenerator.PredefinePath.GetClosestTime(horseTransform.position);
        CalculateRotation(time);
        CalculateVelocity(time);
    }

    private void CalculateVelocity(float time)
    {
        var pointInCurve = TargetGenerator.FromTimeToPoint(time, 0.0f, targetGenerator.PredefinePath);
        var toPosition = (horseTransform.position - pointInCurve);
        rigidbody.velocity = horseTransform.rotation *
                             new Vector3(Input.GetAxisRaw("Horizontal") * 3, 0, velocity.z);
    }

    private void CalculateRotation(float time)
    {
        var eulerAnglesY = targetGenerator.PredefinePath.GetRotation(time)
                                                        .eulerAngles.y;
        horseTransform.rotation = Quaternion.Euler(0, eulerAnglesY, 0);
    }
}
