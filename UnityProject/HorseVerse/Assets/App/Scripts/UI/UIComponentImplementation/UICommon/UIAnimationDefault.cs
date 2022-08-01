using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIAnimationDefault : UISequenceAnimationBase
{
    public RectTransform container;
    public float duration = 0.25f;
    protected override Tween CreateInAnimation()
    {
        return container.DOFade(0.0f, 1.0f, duration);
    }

    protected override Tween CreateOutAnimation()
    {
        return container.DOFade(1.0f, 0.0f, duration, true);
    }
}
