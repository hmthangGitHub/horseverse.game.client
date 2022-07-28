using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraBlendingAnimation : MonoBehaviour
{
    public UnityEngine.UI.Image image;
    public void OnCameraActivate()
    {
        DOTween.Sequence()
            .Append(image.DOFade(1.0f, 0.15f))
            .Append(image.DOFade(0.0f, 0.75f));
    }
}
