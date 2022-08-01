using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using UnityEngine;

public class UIHorse3DViewAnimation : UISequenceAnimationBase
{
    public UIDissolve uiDissolve;
    protected override Tween CreateInAnimation()
    {
        return DOTweenExtensions.To(val => uiDissolve.effectFactor = val, 1.0f, 0.0f, 1.0f);

    }

    protected override Tween CreateOutAnimation()
    {
        return DOTweenExtensions.To(val => uiDissolve.effectFactor = val, 0.0f, 1.0f, 0.5f,reverseOnKill: true);
    }
}
