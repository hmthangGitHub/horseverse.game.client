using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoading : PopupEntity<UILoading.Entity>
{
    protected override async UniTask AnimationOut()
    {
        await this.canvasGroup.DOFade(0.0f, 0.5f).AsyncWaitForCompletion();
    }

    protected override void OnSetEntity()
    {
    }

    [Serializable]
    public class Entity
    {
    }
}
