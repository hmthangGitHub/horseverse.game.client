using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIHorseStableAnimation : UISequenceAnimationBase
{
    public LayoutGroup layoutGroup;
    public UIComponentHorseStableAvatarList stableHorseAvatarList;
    protected override Tween CreateInAnimation()
    {
        layoutGroup.enabled = false;
        return stableHorseAvatarList.CreateSequenceAnimation(x => x.RectTransform().DOAnchorPosXFrom(-500, 0.15f).SetEase(Ease.OutBack), false, 0.1f)
            .OnKill(() => layoutGroup.enabled = true);
    }

    protected override Tween CreateOutAnimation()
    {
        layoutGroup.enabled = false;
        return stableHorseAvatarList.CreateSequenceAnimation(x => x.RectTransform().DOAnchorPosXToThenReverse(-500, 0.10f).SetEase(Ease.InBack), false, 0.05f)
            .OnKill(() => layoutGroup.enabled = true);
    }
}
