using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class TrainingCoin : MonoBehaviour
{
    private Tween tween;
    public void Set(float delay)
    {
        tween = transform.DOLocalRotate(new Vector3(0, 18.0f, 0), 0.1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear)
            .SetDelay(delay);
        
        // DOTween.Sequence()
        //     .SetDelay(0.1f)
        //     .Join(transform.DOMoveFrom(transform.position + Vector3.up * (-1.0f), transform.position, 1.0f).SetEase(Ease.OutElastic))
        //     .Join(transform.DOScaleFrom(transform.localScale * 0.0f, transform.localScale, 1.0f).SetEase(Ease.OutElastic))
        //     .SetDelay(UnityEngine.Random.Range(0.1f, 0.6f))
        //     .SetUpdate(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrainingHorse"))
        {
            tween?.Kill();
            transform.DOMove(transform.position + Vector3.up * 1, 0.2f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => Destroy(this.gameObject));    
        }
    }
}
