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
    public GameObject[] buttons;

    public UIComponentHorseBreedProgressList horseProgressList;
    public UIComponentHorseDetailAnimation horseDetailAnimation;
    public UIComponentProgressBar levelProgress;
    public FormattedTextComponent currentExp;
    public FormattedTextComponent energy;
    public VerticalLayoutGroup buttonLayOutGroup;

    public async UniTask AnimationIn()
    {
        await PlayAnimationAsync(CreatInAnimation);
    }

    private Sequence CreatInAnimation()
    {
        return DOTween.Sequence()
            .AppendCallback(() => buttonLayOutGroup.enabled = false)
            .Append(((RectTransform)userInfoAndhorseBreed.transform).DOAnchorPosXFrom(-700, 0.25f))
            .Join(((RectTransform)userInfoAndhorseBreed.transform).DOFade(0.0f, 1.0f, 0.25f))
            .Append(horseProgressList.instanceList
                .Select(x => DOTweenExtensions.To(x.SetProgress, 0, x.entity.progress, 0.25f)).ToArray()
                .AsSequence(false))
            .Join(horseDetailAnimation.CreateAnimation())
            .Join(DOTweenExtensions.To(levelProgress.SetProgress, 0.0f, levelProgress.entity.progress, 0.25f))
            .Join(currentExp.CreateNumberAnimation<int>(0.25f, 0, 1))
            .Join(energy.CreateNumberAnimation<int>(0.25f, 0, 1))
            .Append(buttons.Select(x => ((RectTransform)x.transform).DOAnchorPosXFrom(600, 0.25f)).ToArray()
                .AsSequence(false, 0.1f))
            .OnKill(() => buttonLayOutGroup.enabled = true);
    }

    public UniTask AnimationOut()
    {
        return PlayAnimationAsync(CreatOutAnimation);
    }

    private Tween CreatOutAnimation()
    {
        return DOTween.Sequence()
            .AppendCallback(() => buttonLayOutGroup.enabled = false)
            .Append(((RectTransform)userInfoAndhorseBreed.transform).DOAnchorPosXToThenReverse(-700, 0.25f))
            .Join(((RectTransform)userInfoAndhorseBreed.transform).DOFade(1.0f, 0.0f, 0.5f, true))
            .Join(buttons.Select(x => ((RectTransform)x.transform).DOAnchorPosXToThenReverse(600, 0.25f)).ToArray()
                .AsSequence(false, 0.1f))
            .OnKill(() => buttonLayOutGroup.enabled = true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            AnimationIn().Forget();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            AnimationOut().Forget();
        }
    }
}
