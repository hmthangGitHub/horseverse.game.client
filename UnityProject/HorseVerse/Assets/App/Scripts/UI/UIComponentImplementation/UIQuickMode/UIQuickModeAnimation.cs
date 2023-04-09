using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIQuickModeAnimation : UISequenceAnimationBase
{
    public UIHorseSelectionAnimation horseSelectAnimation;
    public UIComponentHorseDetailAnimation horseDetailAnimation;
    public RectTransform btnContainer;
    
    protected override Tween CreateInAnimation()
    {
        return DOTween.Sequence()
            .Append((horseSelectAnimation.RectTransform()).DOAnchorPosXFrom(-900, 0.20f))
            .Append(horseSelectAnimation.CreateInAnimation())
            .Join(DOTween.Sequence()
                .AppendInterval(0.1f)
                .Append(((RectTransform)horseDetailAnimation.transform).DOAnchorPosXFrom(900, 0.20f))
                .Append(horseDetailAnimation.CreateAnimation())
                .Append(btnContainer.DOFade(0.0f, 1.0f, 0.15f)));
    }

    protected override Tween CreateOutAnimation()
    {
        return default;
    }
}
