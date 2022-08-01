using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIHorseIntroInfoAnimation : UISequenceAnimationBase
{
    public RectTransform container;
    public RectTransform horseInfoContainer;
    public RectTransform gate;
    public FormattedTextComponent horseName;
    public RectTransform stats;
    
    protected override Tween CreateInAnimation()
    {
        return DOTween.Sequence()
            .Append(horseInfoContainer.DOFade(0.0f, 1.0f, 0.25f))
            .Append(gate.DOAnchorPosXFrom(700, 0.25f))
            .Join(horseName.RectTransform().DOAnchorPosXFrom(-700, 0.25f))
            .Append(horseName.CreateScrambleAnimation(0.25f))
            .Append(stats.DOFade(0.0f, 1.0f, 0.25f));
    }

    protected override Tween CreateOutAnimation()
    {
        return container.DOFade(1.0f, 0.0f, 0.25f, true);
    }
}
