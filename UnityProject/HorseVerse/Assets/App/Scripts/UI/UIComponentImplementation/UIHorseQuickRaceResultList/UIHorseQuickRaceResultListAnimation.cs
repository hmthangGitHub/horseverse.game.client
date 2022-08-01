using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIHorseQuickRaceResultListAnimation : UISequenceAnimationBase
{
    public LayoutGroup layoutGroup;
    public UIComponentQuickRaceResultList quickRaceResultList;
    public CanvasGroup container;
    protected override Tween CreateInAnimation()
    {
        layoutGroup.enabled = false;
        return DOTween.Sequence()
            .Append(quickRaceResultList.instanceList.Select(x => x.RectTransform().DOAnchorPosYFrom(-1700, 0.2f))
                .ToArray().AsSequence(false,0.1f))
            .OnKill(() => layoutGroup.enabled = true);
    }

    protected override Tween CreateOutAnimation()
    {
        return container.RectTransform().DOFade(1.0f, 0.0f, 0.15f, true);
    }
    
    public override async UniTask AnimationIn()
    {
        container.alpha = 0.0f;
        await TryEnableLayoutBuilderAsync();
        container.alpha = 1.0f;
        await base.AnimationIn();
    }

    private async UniTask TryEnableLayoutBuilderAsync()
    {
        await UniTask.DelayFrame(2,PlayerLoopTiming.LastPostLateUpdate, cancellationToken: this.GetCancellationTokenOnDestroy());
        this.quickRaceResultList.gameObject.SetActive(false);
        this.quickRaceResultList.gameObject.SetActive(true);
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, this.GetCancellationTokenOnDestroy());
        this.quickRaceResultList.gameObject.SetActive(false);
        this.quickRaceResultList.gameObject.SetActive(true);
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, this.GetCancellationTokenOnDestroy());
    }
}
