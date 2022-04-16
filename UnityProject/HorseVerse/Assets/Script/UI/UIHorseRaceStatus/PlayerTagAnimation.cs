using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTagAnimation : MonoBehaviour
{
    public RectTransform rectTransform;
    private Sequence sequence;

    private void OnEnable()
    {
        sequence?.Kill(true);
        var originalPos = this.rectTransform.anchoredPosition.y;
        sequence = DOTween.Sequence()
               .Append(rectTransform.DOAnchorPosY(originalPos + 15, 1.0f).SetEase(Ease.OutExpo))
               .Append(rectTransform.DOAnchorPosY(originalPos, 0.25f).SetEase(Ease.InExpo))
               .SetLoops(-1, LoopType.Restart);
    }
}
