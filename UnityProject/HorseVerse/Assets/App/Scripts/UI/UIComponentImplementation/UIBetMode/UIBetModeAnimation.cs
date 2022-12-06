using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;

public class UIBetModeAnimation : UISequenceAnimationBase
{
    public UIComponentSingleBetSlotList singleSlots;
    public RectTransform singleBet;
    public GridLayoutGroup singleBetHorizontalLayout;
    public UIComponentDoubleBetList doubleBetSlots;
    public RectTransform doubleBet;
    public GridLayoutGroup doubleBetHorizontalLayout;
    public CanvasGroup canvasGroup;
    public RectTransform quickBetButtonsContainer;
    public RectTransform betAmouthsContainer;
        
    public async UniTask AnimationIn()
    {
        await PlayAnimationAsync(CreateAnimationIn);
    }

    public async UniTask AnimationOut()
    {
        Debug.Log("OUTTX XXXX ");
        await PlayAnimationAsync(CreateAnimationOut);
    }

    private Tween CreateAnimationOut()
    {
        return DOTween.Sequence()
            
            .AppendCallback(() =>
            {
                doubleBetHorizontalLayout.enabled = false;
                singleBetHorizontalLayout.enabled = false;
            })
            .Append(betAmouthsContainer.DOAnchorPosYToThenReverse(-500.0f, 0.15f))
            .Append(quickBetButtonsContainer.DOFade(1.0f, 0.0f, 0.15f))
            .Append(singleSlots.instanceList.Select(x => ((RectTransform)x.transform).DOAnchorPosXToThenReverse(2500, 0.15f)).ToArray().AsSequence(false))
            .Join(doubleBetSlots.instanceList.Select(x => ((RectTransform)x.transform).DOAnchorPosXToThenReverse(2500, 0.15f)).ToArray().AsSequence(false))
            .Join(singleBet.DOAnchorPosXToThenReverse(-600.0f, 0.15f))
            .Join(doubleBet.DOAnchorPosXToThenReverse(-600.0f, 0.15f))
            .Append(((RectTransform)canvasGroup.transform).DOFade(1.0f, 0.0f, 0.5f, true))
            .OnKill(() =>
            {
                doubleBetHorizontalLayout.enabled = true;
                singleBetHorizontalLayout.enabled = true;
            });
    }

    private Sequence CreateAnimationIn()
    {
        return DOTween.Sequence()
            .AppendCallback(() => canvasGroup.alpha = 0)
            .AppendCallback(() =>
            {
                doubleBetHorizontalLayout.enabled = false;
                singleBetHorizontalLayout.enabled = false;
            })
            .Append(((RectTransform)canvasGroup.transform).DOFade(0.0f, 1.0f, 0.5f))
            .Append(singleSlots.instanceList.Select(x => ((RectTransform)x.transform).DOAnchorPosXFrom(2500, 0.15f)).ToArray().AsSequence(false))
            .Join(singleBet.DOAnchorPosXFrom(-600.0f, 0.15f))
            .Join(doubleBet.DOAnchorPosXFrom(-600.0f, 0.15f))
            .Join(doubleBetSlots.instanceList.Select(x => ((RectTransform)x.transform).DOAnchorPosXFrom(2500, 0.15f)).ToArray().AsSequence(false))
            .Append(quickBetButtonsContainer.DOFade(0.0f, 1.0f, 0.15f))
            .Append(betAmouthsContainer.DOAnchorPosYFrom(-500.0f, 0.15f))
            .OnKill(() =>
            {
                doubleBetHorizontalLayout.enabled = true;
                singleBetHorizontalLayout.enabled = true;
            });
    }
}
