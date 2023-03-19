using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UILoadingFadingAnimation : UISequenceAnimationBase
{
    public CanvasGroup canvasGroup;
    protected override Tween CreateInAnimation()
    {
        return canvasGroup.DOFade(0.0f, 1.0f, 0.5f).SetUpdate(true);
    }

    protected override Tween CreateOutAnimation()
    {
        return canvasGroup.DOFade(1.0f, 0.0f, 0.5f).SetUpdate(true);;
    }
}
