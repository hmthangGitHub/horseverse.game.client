using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

public class UISequenceAnimationBase : MonoBehaviour
{
    private CancellationTokenSource cts;
    private Tween tweenAnimation;

    protected async UniTask PlayAnimation(Func<Tween> animationFactory)
    {
        EndAnimation();
        tweenAnimation = animationFactory.Invoke();
        await tweenAnimation.AwaitForComplete(TweenCancelBehaviour.CancelAwait, cts.Token);
    }

    protected void EndAnimation()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        tweenAnimation?.Kill(true);
    }
}