using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HorseRaceCameraTest : MonoBehaviour
{
    public float spacing = 1.0f;
    public float baseOffset = -5.0f;
    public Transform[] horseControllers;
    public float t;
    public PathCreation.PathCreator path;
    public TargetGenerator targetGenerator;

    void Update()
    {
        for (int i = 0; i < horseControllers.Length; i++)
        {
            var currentOffset = baseOffset + i * spacing;
            CalculatePosition(t, horseControllers[i], currentOffset);
            CalculateRotation(t, horseControllers[i]);
        }
    }

    public void CalculatePosition(float time, Transform transform, float currentOfsset)
    {
        var pos = path.path.GetPointAtTime(time, EndOfPathInstruction.Loop);
        transform.position = Vector3.Scale(new Vector3(1, 0, 1), (pos + transform.right * currentOfsset));
    }

    public void CalculateRotation(float time, Transform transform)
    {
        Quaternion rotationAtDistance = path.path.GetRotation(time, EndOfPathInstruction.Loop);
        transform.rotation = rotationAtDistance;
        transform.rotation = Quaternion.Euler(0, rotationAtDistance.eulerAngles.y, 0);
    }
}
