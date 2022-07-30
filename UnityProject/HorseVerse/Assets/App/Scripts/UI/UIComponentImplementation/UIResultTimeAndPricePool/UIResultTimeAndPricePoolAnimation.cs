using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIResultTimeAndPricePoolAnimation : UISequenceAnimationBase
{
    public RectTransform container;
    public FormattedTextComponent prizePool;
    public UIComponentTimeSpan timeSpan;
    protected override Tween CreateInAnimation()
    {
        return DOTween.Sequence()
            .Append(container.DOFade(0.0f, 1.0f, 0.15f))
            .Append(timeSpan.CreateAnimation(0.5f))
            .Append(prizePool.CreateNumberAnimation<int>(0.15f));
    }

    protected override Tween CreateOutAnimation()
    {
        return DOTween.Sequence()
            .Append(container.DOFade(1.0f, 0.0f, 0.15f, true));
    }
}
