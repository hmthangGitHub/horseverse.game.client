using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIRaceResultListAnimation : UISequenceAnimationBase
{
    public UIComponentHorseResultList uiComponentHorseResultList;
    public LayoutGroup layoutGroup;
    public RectTransform selfResult;
    public RectTransform confirmBtn;
    public RectTransform container;
    
    protected override Tween CreateInAnimation()
    {
        layoutGroup.enabled = false;
        return uiComponentHorseResultList
            .CreateSequenceAnimation(x => x.RectTransform().DOAnchorPosYFrom(-800, 0.25f), false, 0.1f)
            .Append(selfResult.DOAnchorPosXFrom(-1200, 0.15f))
            .Append(confirmBtn.DOFade(0.0f, 1.0f, 0.15f))
            .OnKill(() => layoutGroup.enabled = true)
            .SetUpdate(true);
    }

    protected override Tween CreateOutAnimation()
    {
        return container.DOFade(1.0f, 0.0f, 0.15f, true);
    }
}
