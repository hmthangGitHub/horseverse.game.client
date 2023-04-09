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
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIMainMenuAnimation : UISequenceAnimationBase
{
    public GameObject userInfoAndhorseBreed;
    public CanvasGroup Banner;

    public UIComponentHorseDetailAnimation horseDetailAnimation;

    protected override Tween CreateInAnimation()
    {
        return DOTween.Sequence()
            .Append(((RectTransform)userInfoAndhorseBreed.transform).DOAnchorPosXFrom(-700, 0.25f))
            .Join(((RectTransform)userInfoAndhorseBreed.transform).DOFade(0.0f, 1.0f, 0.25f))
            .Join(horseDetailAnimation.CreateAnimation())
            .Join(((RectTransform)Banner.transform).DOAnchorPosXFrom(1200, 0.25f));
    }

    protected override Tween CreateOutAnimation()
    {
        return DOTween.Sequence()
            .Append(((RectTransform)userInfoAndhorseBreed.transform).DOAnchorPosXToThenReverse(-700, 0.25f))
            .Join(((RectTransform)userInfoAndhorseBreed.transform).DOFade(1.0f, 0.0f, 0.5f, true))
            .Join(((RectTransform)Banner.transform).DOAnchorPosXToThenReverse(1200, 0.25f));
    }
}
