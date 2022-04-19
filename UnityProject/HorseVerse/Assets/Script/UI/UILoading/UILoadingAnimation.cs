using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;

public class UILoadingAnimation : MonoBehaviour
{
    public RectTransform[] indicators;
    private Vector2[] originalPoints = default;
    private Sequence sequence;

    public void OnEnable()
    {
        originalPoints ??= GetOriginalPoint();
        sequence?.Kill(true);
        sequence = DOTween.Sequence();

        for (int i = 0; i < indicators.Length; i++)
        {
            indicators[i].anchoredPosition = originalPoints[i];
            sequence.AppendInterval(0.25f * i);
            sequence.Join(indicators[i].DOAnchorPosY(originalPoints[i].y + 20, 0.5f).SetEase(Ease.OutExpo))
                    .Append(indicators[i].DOAnchorPosY(originalPoints[i].y, 0.5f).SetEase(Ease.InExpo));
        }
        sequence.SetLoops(-1);
    }

    public void OnDisable()
    {
        sequence?.Kill(true);
    }

    private Vector2[] GetOriginalPoint()
    {
        return indicators.Select(x => x.anchoredPosition).ToArray();
    }
}
