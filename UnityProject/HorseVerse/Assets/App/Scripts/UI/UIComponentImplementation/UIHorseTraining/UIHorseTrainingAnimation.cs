using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class UIHorseTrainingAnimation : UISequenceAnimationBase
{
    public UIHorseSelectionAnimation horseSelectAnimation;
    public UIComponentHorseDetailAnimation horseDetailAnimation;
    public RectTransform trainingStates;
    
    protected override Tween CreateInAnimation()
    {
        return DOTween.Sequence()
            .Append(((RectTransform)horseSelectAnimation.transform).DOAnchorPosXFrom(-900, 0.20f))
            .Append(horseSelectAnimation.CreateInAnimation())
            .Join(DOTween.Sequence()
                .AppendInterval(0.1f)
                .Append(((RectTransform)horseDetailAnimation.transform).DOAnchorPosXFrom(900, 0.20f))
                .Append(horseDetailAnimation.CreateAnimation())
                .Append(trainingStates.DOFade(0.0f, 1.0f, 0.15f)));
    }

    protected override Tween CreateOutAnimation()
    {
        return DOTween.Sequence()
            .Append(trainingStates.DOFade(1.0f, 0.0f, 0.15f, true))
            .Append(((RectTransform)horseDetailAnimation.transform).DOAnchorPosXToThenReverse(900, 0.15f))
            .Append(((RectTransform)horseSelectAnimation.transform).DOAnchorPosXToThenReverse(-900, 0.15f));
    }
}
