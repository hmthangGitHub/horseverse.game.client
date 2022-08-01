using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIComponentHorseDetailAnimation : MonoBehaviour
{
    public UIComponentProgressBarWithBonus[] progressBarWithBonuses;
    public FormattedTextComponent earning;

    public Tween CreateAnimation()
    {
        return DOTween.Sequence()
                      .Join(progressBarWithBonuses.Select(x => DOTweenExtensions.To(x.progressBar.SetProgress,
                                                                                     0,
                                                                                     x.entity.progressBar.progress,
                                                                                     0.25f)).ToArray().AsSequence(false))
                      .Join(progressBarWithBonuses.Select(x => x.bonus.CreateNumberAnimation<float>(0.25f)).ToArray().AsSequence(false))
                      .Join(earning.CreateNumberAnimation<int>(0.25f, 0));
    }
}
