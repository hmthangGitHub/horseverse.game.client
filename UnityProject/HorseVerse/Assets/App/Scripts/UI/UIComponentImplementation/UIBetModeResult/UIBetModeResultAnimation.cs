using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class UIBetModeResultAnimation : UISequenceAnimationBase
{
    public RectTransform container;
    public UIComponentBetModeResultList uiComponentBetModeResultList;
    public LayoutGroup layoutGroup;
    public RectTransform nextButton;
    protected override Tween CreateInAnimation()
    {
        layoutGroup.enabled = false;
        return DOTween.Sequence()
            .Append(container.DOFade(0.0f, 1.0f, 0.35f))
            .Append(uiComponentBetModeResultList.instanceList.Select(x => x.RectTransform().DOAnchorPosYFrom(-800, 0.1f))
                .ToArray()
                .AsSequence())
            .Append(nextButton.DOFade(0.0f, 1.0f, 0.15f))
            .OnKill(() => layoutGroup.enabled = true)
            .SetUpdate(true);
    }

    protected override Tween CreateOutAnimation()
    {
        return container.DOFade(1.0f, 0.0f, 0.35f, true);
    }
}
