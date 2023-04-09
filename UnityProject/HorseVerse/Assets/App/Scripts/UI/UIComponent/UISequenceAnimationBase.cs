using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

public abstract class UISequenceAnimationBase : UIAnimationBase
{
    public virtual UniTask AnimationIn()
    {
        return PlayAnimationAsync(CreateInAnimation);
    }

    protected abstract Tween CreateInAnimation();

    protected abstract Tween CreateOutAnimation();

    public virtual UniTask AnimationOut()
    {
        return PlayAnimationAsync(CreateOutAnimation);
    }
}