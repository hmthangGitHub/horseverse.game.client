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
    public string[] excludeCameraBlending;
    public CinemachineBrain cinemachineBrain;

    public void OnCameraActivate()
    {
        PlayCameraFadingAnimationAsync().Forget();
    }

    public async UniTask PlayCameraFadingAnimationAsync()
    {
        //await FadeInAnimationAsync();
        //await FadeOutAnimationAsync();
    }

    public async UniTask FadeInAnimationAsync()
    {
        await image.DOFade(1.5f, durationOut).SetEase(Ease.OutQuart);
    }
    
    public async void FadeInAnimationAsync2()
    {
        if (!excludeCameraBlending.Contains(cinemachineBrain.ActiveVirtualCamera.Name))
        {
            await image.DOFade(1.5f, durationOut).SetEase(Ease.OutQuart);    
        }
    }
    
    public async void FadeOutAnimationAsync2()
    {
        if (!excludeCameraBlending.Contains(cinemachineBrain.ActiveVirtualCamera.Name))
        {
            await image.DOFade(0.0f, durationIn).SetEase(Ease.InQuart);    
        }
    }

    public async UniTask FadeOutAnimationAsync()
    {
        await image.DOFade(0.0f, durationIn).SetEase(Ease.InQuart);
    }
}
