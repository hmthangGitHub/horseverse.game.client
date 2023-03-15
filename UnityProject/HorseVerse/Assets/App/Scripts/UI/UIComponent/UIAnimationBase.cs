using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UIAnimationBase : MonoBehaviour
{
    private CancellationTokenSource cts;
    private Tween tweenAnimation;

    public UniTask PlayAnimationAsync(Func<Tween> animationFactory)
    {
        EndAnimation();
        tweenAnimation = animationFactory.Invoke();
        return tweenAnimation?.AwaitForComplete(TweenCancelBehaviour.CancelAwait, cancellationToken: cts.Token) ?? UniTask.CompletedTask;
    }

    private void EndAnimation()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        tweenAnimation?.Kill(true);
    }

    private void OnDestroy()
    {
        tweenAnimation?.Kill();
    }
}