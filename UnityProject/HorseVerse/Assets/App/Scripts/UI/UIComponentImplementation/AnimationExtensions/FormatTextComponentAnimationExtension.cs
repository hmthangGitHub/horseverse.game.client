using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FormatTextComponentAnimationExtension
{
    public static Tween CreateNumberAnimation<T>(this FormattedTextComponent formattedTextComponent, float duration, params int[] animationIndices)
    {
        var originalValues = formattedTextComponent.entity.param.Cast<T>().ToArray();
        return SequenceExtentions.To(val =>
        {
            for (int i = 0; i < originalValues.Length; i++)
            {
                if (animationIndices.Contains(i))
                {
                    formattedTextComponent.entity.param[i] = (Mathf.Lerp(0.0f, originalValues[i].ToFloat(), val)).NumericBoxing(originalValues[i].GetType());
                }
                formattedTextComponent.SetEntity(formattedTextComponent.entity);
            }
        }, 0.0f, 1.0f, duration);
    }

    public static Tween CreateNumberAnimation<T>(this FormattedTextComponent formattedTextComponent, float duration)
    {
        return CreateNumberAnimation<T>(formattedTextComponent, duration, 0);
    }

    private static object NumericBoxing(this float t, Type numericType)
    {
        return numericType.Name switch
        {
            nameof(System.Int64) => ((object)(long)t),
            nameof(System.Int32) => ((object)(int)t),
            nameof(Single) => t,
            _ => t
        };
    }

    private static float ToFloat(this object numericType)
    {
        return numericType.GetType().Name switch
        {
            nameof(System.Int64) => (long)numericType,
            nameof(System.Int32) => (int)numericType,
            nameof(Single) => (float)numericType,
            _ => (float)numericType
        };
    }
}
