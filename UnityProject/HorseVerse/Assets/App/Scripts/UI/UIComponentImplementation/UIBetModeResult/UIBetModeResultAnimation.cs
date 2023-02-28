using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class UIBetModeResultAnimation : UISequenceAnimationBase
{
    public RectTransform container;
    public UIBetModeResultPanel uIBetModeResultPanel;
    public UIComponentBetModeResultList uiComponentBetModeResultList;
    public UIBetModeMyResultPanel uIBetModeMyResultPanel;
    public UIComponentBetModeMyResultList uiComponentBetModeMyResultList;
    public LayoutGroup layoutGroup;
    public RectTransform nextButton;
    
    protected override Tween CreateInAnimation()
    {
        layoutGroup.enabled = false;
        return DOTween.Sequence()
            .Append(container.DOFade(0.0f, 1.0f, 0.35f))
            .Append(nextButton.DOFade(0.0f, 1.0f, 0.15f))
            .Append(DoShowBetList())
            .OnKill(() => layoutGroup.enabled = true)
            .SetUpdate(true);
    }

    protected override Tween CreateOutAnimation()
    {
        return container.DOFade(1.0f, 0.0f, 0.35f, true);
    }

    private Tween DoShowBetList()
    {
        return DOTween.Sequence().Append(uiComponentBetModeResultList.instanceList.Select(x => x.RectTransform().DOAnchorPosYFrom(-800, 0.1f))
                .ToArray()
                .AsSequence());
    }
}
