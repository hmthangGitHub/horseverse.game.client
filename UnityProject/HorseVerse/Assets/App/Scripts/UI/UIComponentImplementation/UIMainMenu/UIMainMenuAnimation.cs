using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;
using System;

public class UIMainMenuAnimation : UISequenceAnimationBase
{
    public GameObject userInfoAndhorseBreed;
    public GameObject[] buttons;

    public UIComponentHorseBreedProgressList horseProgressList;
    public UIHorseDetailAnimation horseDetailAnimation;
    public UIComponentProgressBar levelProgress;
    public FormattedTextComponent currentExp;
    public FormattedTextComponent energy;

    public UniTask AnimationIn()
    {
        return PlayAnimation(CreatInAnimation);
    }

    private Sequence CreatInAnimation()
    {
        return DOTween.Sequence()
                      .Append(((RectTransform)userInfoAndhorseBreed.transform).DOAnchorPosXFrom(-700, 0.25f))
                      .Join(((RectTransform)userInfoAndhorseBreed.transform).DOFade(0.0f, 1.0f, 0.25f))
                      .Append(horseProgressList.instanceList.Select(x => DOTweenExtensions.To(x.SetProgress, 0, x.entity.progress, 0.25f)).ToArray().AsSequence(false))
                      .Join(horseDetailAnimation.CreateAnimation())
                      .Join(DOTweenExtensions.To(levelProgress.SetProgress, 0.0f, levelProgress.entity.progress, 0.25f))
                      .Join(currentExp.CreateNumberAnimation<int>(0.25f, 0, 1))
                      .Join(energy.CreateNumberAnimation<int>(0.25f, 0, 1))
                      .Append(buttons.Select(x => ((RectTransform)x.transform).DOAnchorPosXFrom(600, 0.25f)).ToArray().AsSequence());
    }

    public UniTask AnimationOut()
    {
        return PlayAnimation(CreatOutAnimation);
    }

    private Tween CreatOutAnimation()
    {
        return DOTween.Sequence()
                      .Append(((RectTransform)userInfoAndhorseBreed.transform).DOAnchorPosXToThenReverse(-700, 0.25f))
                      .Join(((RectTransform)userInfoAndhorseBreed.transform).DOFade(1.0f, 0.0f, 0.5f, true))
                      .Join(buttons.Select(x => ((RectTransform)x.transform).DOAnchorPosXToThenReverse(600, 0.25f)).ToArray().AsSequence());
    }
}
