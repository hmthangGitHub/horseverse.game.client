using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening.Core;
using System;

public static class DOTweenExtensions
{
    public static Sequence Join(this Sequence sequence, Tween[] tweens)
    {
        var result = sequence;
        tweens.ForEach(x => result.Join(x));
        return result;
    }

    public static Sequence Append(this Sequence sequence, Tween[] tweens)
    {
        var result = sequence;
        tweens.ForEach(x => result.Append(x));
        return result;
    }

    public static Sequence AsSequence(this Tween[] tweens, bool isSequence = true)
    {
        return isSequence ? DOTween.Sequence().Append(tweens)
                          : DOTween.Sequence().Join(tweens);
    }

    public static Tween DOAnchorPosXFrom(this RectTransform rect, float from, float duration)
    {
        return rect.DOAnchorPosX(from + rect.anchoredPosition.x, rect.anchoredPosition.x, duration);
    }

    public static Tween DOAnchorPosXToThenReverse(this RectTransform rect, float to, float duration)
    {
        return rect.DOAnchorPosX(rect.anchoredPosition.x, rect.anchoredPosition.x + to, duration, true);
    }

    public static Tween DOAnchorPosX(this RectTransform rect, float from, float to, float duration, bool reverseOnKill = false)
    {
        rect.anchoredPosition = new Vector2(from, rect.anchoredPosition.y);
        return DOTween.To(val => rect.anchoredPosition = new Vector2(val, rect.anchoredPosition.y), from, to, duration)
                      .OnKill(() => rect.anchoredPosition = reverseOnKill ? new Vector2(from, rect.anchoredPosition.y) : new Vector2(to, rect.anchoredPosition.y));
    }

    public static Tween DOFade(this RectTransform rectTransform, float from, float to, float duration, bool reverseOnKill = false)
    {
        var canvasGroup = rectTransform.gameObject.GetOrAddComponent<CanvasGroup>();
        return DOTween.To(val => canvasGroup.alpha = val, from, to, duration)
                      .OnKill(() => canvasGroup.alpha = reverseOnKill ? from : to);
    }

    public static Tween To(Action<float> setter, float from, float to, float duration, bool isWarmUp = true, bool reverseOnKill = false)
    {
        if(isWarmUp)
        {
            setter(from);
        }
        return DOTween.To(val => setter(val), from, to, duration)
                      .OnKill(() => setter(reverseOnKill ? from : to));
    }


}