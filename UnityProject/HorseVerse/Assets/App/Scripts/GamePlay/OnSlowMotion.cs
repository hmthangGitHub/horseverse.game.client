using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSlowMotion : MonoBehaviour
{
    public float timeScale = 0.5f;
    public float duration = 0.5f;
    private Tween tween;
    private bool isIn = false;
    private bool isInAnimation = false;
    private bool finishSlowMotion = false;
    
    public void Slow()
    {
        if (!isIn && !isInAnimation && !finishSlowMotion)
        {
            isInAnimation = true;
            isIn = true;
            DOTween.Kill(tween, false);
            tween = DOTween.To(val =>
            {
                Time.timeScale = val;
            }, 1.0f, timeScale, duration).SetUpdate(true).OnComplete(() =>
            {
                isInAnimation = false;
            });
        }    
    }

    public void Normal()
    {
        if (isIn && isInAnimation == false && !finishSlowMotion)
        {
            isInAnimation = true;
            isIn = false;
            DOTween.Kill(tween, false);
            tween = DOTween.To(val =>
            {
                Time.timeScale = val;
            }, timeScale, 1.0f, duration).SetUpdate(true).OnComplete(() =>
            {
                isInAnimation = false;
                finishSlowMotion = true;
            });
        }    
    }
}
