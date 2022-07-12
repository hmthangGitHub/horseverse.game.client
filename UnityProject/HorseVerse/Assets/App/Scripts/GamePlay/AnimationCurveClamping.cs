using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class AnimationCurveClamping
{
    public static AnimationCurve ClampingCurve(AnimationCurve curve, float length, float maximumDistance)
    {
        float maximumDeltaValueAtTime = maximumDistance / length;

        List<Keyframe> keys = new List<Keyframe>();
        for (int i = 0; i < curve.keys.Length; i++)
        {
            Keyframe item = curve.keys[i];
            item.value = Mathf.Clamp(item.value, item.time - maximumDeltaValueAtTime, item.time + maximumDeltaValueAtTime);
            
            keys.Add(item);
        }

        var newCurve = new AnimationCurve(keys.ToArray());
        for (int i = 0; i < newCurve.length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(newCurve, i, AnimationUtility.TangentMode.Auto);
        }
        return newCurve;
    }

    public static AnimationCurve GenerateCurve(float length, float maximumDistance, int innerKeyNumber)
    {
        List<Keyframe> keys = new List<Keyframe>();
        float maximumDeltaValueAtTime = maximumDistance / length;
        keys.Add(new Keyframe()
        {
            time = 1,
            value = 1
        });
        keys.Add(new Keyframe()
        {
            time = 0,
            value = 0
        });
        for (int i = 0; i < innerKeyNumber; i++)
        {
            float offset = UnityEngine.Random.Range(-0.05f, 0.05f);
            var time = ((i + 1) / ((float)innerKeyNumber + 1)) + offset;
            var value = time + UnityEngine.Random.Range(-maximumDeltaValueAtTime, maximumDeltaValueAtTime);
            keys.Add(new Keyframe()
            {
                time = time,
                value = value
            });
        }
        var newCurve = new AnimationCurve(keys.ToArray());
        for (int i = 0; i < newCurve.length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(newCurve, i, AnimationUtility.TangentMode.Auto);
        }
        return newCurve;
    }
}
