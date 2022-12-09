using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIPopUpAnimation : UISequenceAnimationBase
{
    public Transform container;
    public float duration = 0.25f;

    protected override Tween CreateInAnimation()
    {
        return DOTweenExtensions.To(SetScale, 0.0f, 1.0f, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    protected override Tween CreateOutAnimation()
    {
        return DOTweenExtensions.To(SetScale, 1.0f, 0.0f, duration, reverseOnKill : true)
            .SetEase(Ease.InBack)
            .SetUpdate(true);
    }

    private void SetScale(float val)
    {
        container.localScale = Vector3.one * val;
    }
}
