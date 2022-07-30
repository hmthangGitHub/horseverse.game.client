using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIHorseDetailAnimation : UISequenceAnimationBase
{
    public UIComponentHorseDetailAnimation horseDetailAnimation;
    public RectTransform levelUpBtn;
    protected override Tween CreateInAnimation()
    {
        return DOTween.Sequence()
            .Append(horseDetailAnimation.RectTransform().DOAnchorPosXFrom(900, 0.20f))
            .Append(horseDetailAnimation.CreateAnimation())
            .Append(levelUpBtn.DOFade(0.0f, 1.0f, 0.25f));
    }

    protected override Tween CreateOutAnimation()
    {
        return DOTween.Sequence()
            .Append(levelUpBtn.DOFade(1.0f, 0.0f, 0.25f, true))
            .Append(horseDetailAnimation.RectTransform().DOAnchorPosXToThenReverse(900, 0.20f));
    }
}
