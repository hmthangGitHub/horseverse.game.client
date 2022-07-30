using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UIHeaderAnimation : UISequenceAnimationBase
{
    public RectTransform container;

    public UniTask AnimationIn()
    {
        return PlayAnimationAsync(CreateInAnimation);
    }

    public UniTask AnimationOut()
    {
        return PlayAnimationAsync(CreateOutAnimation);
    }

    private Tween CreateOutAnimation()
    {
        return container.DOAnchorPosYToThenReverse(200, 0.25f);
    }

    private Tween CreateInAnimation()
    {
        return container.DOAnchorPosYFrom(200, 0.25f);
    }
}
