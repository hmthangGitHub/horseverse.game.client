using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UIElementAnimationDefault : UIElementAnimationBase
{
    
    
    public float durationIn = 0.05f;
    public float durationOut = 0.15f;
    public float scale = 0.8f;

    protected override Tween OnClickAnimation()
    {
        return DOTween.Sequence()
            .Append(animationTransform.DOScaleFrom(1.0f, scale, durationIn).SetEase(Ease.Linear))
            .Append(animationTransform.DOScaleFrom(scale, 1.0f, durationOut).SetEase(Ease.OutElastic));
    }
}