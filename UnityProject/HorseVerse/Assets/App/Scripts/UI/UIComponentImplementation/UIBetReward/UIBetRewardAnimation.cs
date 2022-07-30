using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

public class UIBetRewardAnimation : UISequenceAnimationBase
{
    public RectTransform container;
    public RectTransform effectContainer;
    public FormattedTextComponent reward;
    public RectTransform icon;
    protected override Tween CreateInAnimation()
    {
        return DOTween.Sequence()
            .Append(container.DOFade(0.0f, 1.0f, 0.25f))
            .Append(effectContainer.DOFade(0.0f, 1.0f, 0.25f))
            .Join(reward.RectTransform().DOAnchorPosYFrom(-100, 0.25f))
            .Join(icon.DOAnchorPosYFrom(-100, 0.25f))
            .Join(reward.CreateNumberAnimation<int>(0.35f));
    }

    protected override Tween CreateOutAnimation()
    {
        return container.DOFade(1.0f, 0.0f, 0.25f, true);
    }
}
