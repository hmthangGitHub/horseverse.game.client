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
        return DOTweenExtensions.To(val =>
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

    public static Tween CreateScrambleAnimation(this FormattedTextComponent formattedTextComponent, float duration)
    {
        var original = formattedTextComponent.text.text;
        return DOTweenExtensions.To(val =>
        {
            var revealLength = (int)Mathf.Lerp(0.0f, formattedTextComponent.text.text.Length, val);
            formattedTextComponent.text.text = $"{original.Substring(0, revealLength)}{RandomString(original.Length - revealLength)}";
        }, 0.0f, 1.0f, duration);
    }
    
    private static System.Random random = new System.Random();
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
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
