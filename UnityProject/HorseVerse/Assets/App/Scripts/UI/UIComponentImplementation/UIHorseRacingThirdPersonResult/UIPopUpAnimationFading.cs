using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIPopUpAnimationFading : UISequenceAnimationBase
{
    public CanvasGroup canvasGroup;
    public float duration;
    
    protected override Tween CreateInAnimation()
    {
        return canvasGroup.DOFade(0.0f, 1.0f, duration);
    }

    protected override Tween CreateOutAnimation()
    {
        return canvasGroup.DOFade(1.0f, 0.0f, duration);
    }
}
