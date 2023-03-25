using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class CameraBlendingAnimation : MonoBehaviour
{
    public UnityEngine.UI.Image image;
    public float durationOut = 0.5f;
    public float durationIn = 0.25f;

    public async UniTask FadeInAnimationAsync()
    {
        await image.DOFade(1.5f, durationOut).SetEase(Ease.OutQuart);
    }

    public async UniTask FadeOutAnimationAsync()
    {
        await image.DOFade(0.0f, durationIn).SetEase(Ease.InQuart);
    }
}
