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
    [SerializeField] private Collider collider;
    private Tween tween;
    public System.Action onDestroy { get; set; }

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
            collider.enabled = false;
            var horseTrainingControllerV2 = other.GetComponent<HorseTrainingControllerV2>();
            DOTween.Sequence()
                   .Append(MoveTo(other.transform, Vector3.up * 0.5f, 0.3f, Ease.InFlash))
                   .Append(DOTween.To(val =>
                   {
                       this.transform.position = horseTrainingControllerV2.GetCoinAnimationPoint(val);
                   }, 0.0f, 1.0f, 0.3f)
                                  .SetUpdate(UpdateType.Late)
                                  .SetEase(Ease.Linear))
                   .OnComplete(DestroyThis);
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

    public void DestroyThis()
    {
        if (onDestroy != default) onDestroy.Invoke();
        else Destroy(this.gameObject);
    }
}
