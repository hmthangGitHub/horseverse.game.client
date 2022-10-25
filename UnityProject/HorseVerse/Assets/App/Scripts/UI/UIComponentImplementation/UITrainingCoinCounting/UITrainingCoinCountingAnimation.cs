using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using UnityEngine;

public class UITrainingCoinCountingAnimation : UISequenceAnimationBase
{
    public UnityEngine.UI.Image bg;
    protected override Tween CreateOutAnimation()
    {
        return bg.DOFade(0.0f, 1.0f, 2.0f).SetEase(Ease.OutExpo);
    }
    
    protected override Tween CreateInAnimation()
    {
        return bg.DOFade(1.0f, 0.0f, 0.5f);
    }
}
