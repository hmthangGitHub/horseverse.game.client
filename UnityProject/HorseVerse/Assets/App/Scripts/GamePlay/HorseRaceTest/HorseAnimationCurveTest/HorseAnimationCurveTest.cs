#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HorseAnimationCurveTest : MonoBehaviour
{
    public AnimationCurve curve;
    public float path = 900f;
    public float maximumDistance = 1.0f;
    public List<AnimationCurve> curves;

    [ContextMenu("ProcessCurve")]
    public void ProcessCurve()
    {
        curves.Add(AnimationCurveClamping.ClampingCurve(curve, path, maximumDistance));
        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Generate Curve")]
    public void GenerateCurve()
    {
        curves.Add(AnimationCurveClamping.GenerateCurve(path, 1.0f, UnityEngine.Random.Range(2, 5)));
        EditorUtility.SetDirty(this);
    }
}
#endif