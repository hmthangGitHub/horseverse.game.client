using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class TrainingCoin : MonoBehaviour
{
    public void Set(float delay)
    {
        DOTween.Sequence()
            .SetDelay(delay)
            .Append(transform.DOLocalRotate(new Vector3(0, 360, 0), 2.0f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1))
            .Join(transform.DOMoveFrom(transform.position + Vector3.up * (-1.0f), transform.position, 1.0f).SetEase(Ease.OutElastic))
            .Join(transform.DOScaleFrom(transform.localScale * 0.0f, transform.localScale, 1.0f).SetEase(Ease.OutElastic));
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }
}
