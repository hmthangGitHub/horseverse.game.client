using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

public class UISequenceAnimationBase : UIAnimationBase
{
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