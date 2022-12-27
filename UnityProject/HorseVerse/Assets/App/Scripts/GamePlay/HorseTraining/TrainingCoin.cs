using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Plane = UnityEngine.Plane;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class TrainingCoin : MonoBehaviour
{
    private Tween tween;

    private void OnEnable()
    {
        Set(UnityEngine.Random.Range(0.0f, 1.0f));
    }

    public void Set(float delay)
    {
        tween = transform.DOLocalRotate(new Vector3(0, 18.0f, 0), 0.1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear)
            .SetDelay(delay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrainingHorse"))
        {
            tween?.Kill();
            var relative = other.GetComponent<HorseTrainingControllerV2>().GetRelativePoint();
            DOTween.Sequence()
                   .Append(MoveTo(other.transform, Vector3.up * 0.5f, 0.5f, Ease.InFlash))
                   .Append(MoveTo(other.transform, relative, 0.5f, Ease.Linear))
                   .OnComplete(() => Destroy(this.gameObject));
        }
    }

    private Tweener MoveTo(Transform other, 
                           Vector3 relative,
                           float duration,
                           Ease ease)
    {
        return DOTween.To(val =>
                      {
                          this.transform.position = Vector3.Lerp(this.transform.position, other.transform.position + relative, val);
                      }, 0.0f, 1.0f, duration)
                      .SetEase(ease);
    }
}
