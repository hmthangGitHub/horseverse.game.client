using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIHorseStableDetailAnimation : UISequenceAnimationBase
{
    public RectTransform detailContainer;
    public RectTransform briefInfo;
    public float duration = 0.25f;
    public float offsetDetailContainer = 1000.0f;
    public float offsetBriefInfoContainer = 500.0f;

    protected override Tween CreateInAnimation()
    {
        return DOTween.Sequence()
                      .Append(detailContainer.DOAnchorPosX(detailContainer.anchoredPosition.x + offsetDetailContainer, detailContainer.anchoredPosition.x, duration)
                                                 .SetEase(Ease.OutBack))
                      .Join(briefInfo.DOAnchorPosX(briefInfo.anchoredPosition.x + offsetBriefInfoContainer, briefInfo.anchoredPosition.x, duration)
                                       .SetEase(Ease.OutBack))
               .SetUpdate(true);
    }

    protected override Tween CreateOutAnimation()
    {
        return DOTween.Sequence()
                      .Append(detailContainer.DOAnchorPosX(detailContainer.anchoredPosition.x , detailContainer.anchoredPosition.x + offsetDetailContainer, duration, true)
                                             .SetEase(Ease.OutBack))
                      .Join(briefInfo.DOAnchorPosX(briefInfo.anchoredPosition.x, briefInfo.anchoredPosition.x + offsetBriefInfoContainer, duration, true)
                                     .SetEase(Ease.OutBack))
                      .SetUpdate(true);
    }
}
