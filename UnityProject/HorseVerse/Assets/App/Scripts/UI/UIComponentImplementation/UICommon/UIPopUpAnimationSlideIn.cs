using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIPopUpAnimationSlideIn : UISequenceAnimationBase
{
    public Transform container;
    public float duration = 0.25f;
    public float offset = 100.0f;

    protected override Tween CreateInAnimation()
    {
        var rectTransform = (container as RectTransform);
        return rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x + offset, rectTransform.anchoredPosition.x, duration)
                            .SetEase(Ease.OutBack)
                            .SetUpdate(true);
    }

    protected override Tween CreateOutAnimation()
    {
        var rectTransform = (container as RectTransform);
        return rectTransform.DOAnchorPosX( rectTransform.anchoredPosition.x , rectTransform.anchoredPosition.x + offset, duration, reverseOnKill: true)
                            .SetEase(Ease.InBack)
                            .SetUpdate(true);
    }
}
