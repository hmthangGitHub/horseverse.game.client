using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

public class UISequenceAnimationBase : MonoBehaviour
{
    private CancellationTokenSource cts;
    private Tween tweenAnimation;
    
    public UniTask PlayAnimationAsync(Func<Tween> animationFactory)
    {
        EndAnimation();
        tweenAnimation = animationFactory.Invoke();
        return tweenAnimation?.AwaitForComplete(TweenCancelBehaviour.CancelAwait, cts.Token) ?? UniTask.CompletedTask;
    }

    private void EndAnimation()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        tweenAnimation?.Kill(true);
    }

    public virtual UniTask AnimationIn()
    {
        return PlayAnimationAsync(CreateInAnimation);
    }

    protected virtual Tween CreateInAnimation()
    {
        return default;
    }

    protected virtual Tween CreateOutAnimation()
    {
        return default;
    }

    public virtual UniTask AnimationOut()
    {
        return PlayAnimationAsync(CreateOutAnimation);
    }
}