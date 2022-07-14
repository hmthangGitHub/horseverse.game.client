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
        return newCurve;
    }

    public static AnimationCurve GenerateCurve(float length, float maximumDistance, int innerKeyNumber)
    {
        List<Keyframe> keys = new List<Keyframe>();
        float maximumDeltaValueAtTime = maximumDistance / length;
        
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
                value = value,
            });
        }

        keys.Add(new Keyframe()
        {
            time = 1,
            value = 1
        });

        for (int i = 0; i < keys.Count; i++)
        {
            Keyframe keyframe = keys[i];
            keyframe.inTangent = CalculateIntangent(keys, i, keyframe);
            keyframe.outTangent = CalculateOuttangent(keys, i, keyframe);
            keys[i] = keyframe;
        }
        return new AnimationCurve(keys.ToArray());
    }

    private static float CalculateIntangent(List<Keyframe> keys, int i, Keyframe keyframe)
    {
        if (i == 0)
        {
            return 1;
        }
        else
        {
            Keyframe previousFrame = keys[i - 1];
            var previousPoint = new Vector2(previousFrame.time, previousFrame.value);
            var currentPoint = new Vector2(keyframe.time, keyframe.value);
            var normalizePoint = new Vector2(keyframe.time, keyframe.time);

            return Vector2.Dot((currentPoint - previousPoint).normalized, (normalizePoint - previousPoint).normalized);
        }
    }

    private static float CalculateOuttangent(List<Keyframe> keys, int i, Keyframe keyframe)
    {
        if (i == keys.Count - 1)
        {
            return 1;
        }
        else
        {
            Keyframe nextFrame = keys[i + 1];
            var nextPoint = new Vector2(nextFrame.time, nextFrame.value);
            var currentPoint = new Vector2(keyframe.time, keyframe.value);
            var normalizePoint = new Vector2(keyframe.time, keyframe.time);

            return Vector2.Dot((currentPoint - nextPoint).normalized, (normalizePoint - nextPoint).normalized);
        }
    }
}
