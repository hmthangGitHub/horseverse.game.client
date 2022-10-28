using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UITrainingPressAnyKeyTextFadingAnimation : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    private Sequence tween;

    private void OnEnable()
    {
        tween?.Kill(true);
        textMeshProUGUI.alpha = 1.0f;
        tween = DOTween.Sequence().Append(textMeshProUGUI.DOFade(0.0f, 1.0f))
            .AppendInterval(0.5f)
            .Append(textMeshProUGUI.DOFade(1.0f, 1.0f))
            .AppendInterval(0.5f)
            .SetLoops(-1);
    }
}
