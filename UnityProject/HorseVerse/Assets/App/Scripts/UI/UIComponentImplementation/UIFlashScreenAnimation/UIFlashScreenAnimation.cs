using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFlashScreenAnimation : PopupEntity<UIFlashScreenAnimation.Entity>
{
    [System.Serializable]
    public class Entity
    {
    }

    protected override void OnSetEntity()
    {
    }

    protected async override UniTask AnimationIn()
    {
        await base.AnimationIn();
        this.canvasGroup.alpha = 0.0f;
        await DOTween.Sequence().Append(this.canvasGroup.DOFade(1.0f, 0.05f).SetEase(Ease.OutQuint).SetUpdate(true))
                                .Append(this.canvasGroup.DOFade(0.0f, 0.15f).SetEase(Ease.InQuint).SetUpdate(true))
                                .AwaitForComplete();
    }
}