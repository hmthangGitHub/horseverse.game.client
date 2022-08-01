using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIHorseSelectionAnimation : MonoBehaviour
{
    public UIComponentTraningHorseSelectSumaryList uiComponentTraningHorseSelectSumaryList;
    public LayoutGroup layoutGroup;

    public Tween CreateInAnimation()
    {
        layoutGroup.enabled = false;
        return DOTween.Sequence()
            .Append(uiComponentTraningHorseSelectSumaryList.instanceList
                .Select(x => x.RectTransform().DOAnchorPosXFrom(700, 0.1f)).ToArray().AsSequence())
            .OnKill(() => layoutGroup.enabled = true);
    }
}
