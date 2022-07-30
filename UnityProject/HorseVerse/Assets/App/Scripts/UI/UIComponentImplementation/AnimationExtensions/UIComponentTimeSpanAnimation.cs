using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class UIComponentTimeSpanAnimation
{
    public static Tween CreateAnimation(this UIComponentTimeSpan timeSpan, float duration)
    {
        return DOTweenExtensions.To(timeSpan.SetEntity, 0.0f, timeSpan.entity.totalSecond, duration);
    }
}
