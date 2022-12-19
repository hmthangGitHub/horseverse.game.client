using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UIHeaderAnimation : UISequenceAnimationBase
{
    public RectTransform container;

    protected override Tween CreateOutAnimation()
    {
        return container.DOAnchorPosYToThenReverse(200, 0.25f);
    }

    protected override Tween CreateInAnimation()
    {
        return container.DOAnchorPosYFrom(200, 0.25f);
    }
}
