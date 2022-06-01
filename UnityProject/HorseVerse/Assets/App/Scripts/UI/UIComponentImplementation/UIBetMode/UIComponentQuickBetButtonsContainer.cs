using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentQuickBetButtonsContainer : UIComponent<UIComponentQuickBetButtonsContainer.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public Action<int> onBetAll = _ => { };
        public Action<int> onBetVertical = _ => { };
        public Action<int> onBetHorizontal = _ => { };
    }

    protected override void OnSetEntity()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var quickBetButton = this.transform.GetChild(i).GetComponentInChildren<UIComponentQuickBetButtons>();
            quickBetButton.SetEntity(new UIComponentQuickBetButtons.Entity()
            {
                betAllBtn = new ButtonComponent.Entity(() => this.entity.onBetAll.Invoke(i)),
                horizontalBetBtn = new ButtonComponent.Entity(() => this.entity.onBetHorizontal.Invoke(i)),
                verticalBetBtn = new ButtonComponent.Entity(() => this.entity.onBetVertical.Invoke(i)),
            });
        }
    }

#if UNITY_EDITOR
    public UnityEngine.UI.GridLayoutGroup betslotGridLayoutGroup;
    [ContextMenu("Arrange Quick Bet Buttons")]
    public void ArrangeQuickBetButtons()
    {
        var horseCount = betslotGridLayoutGroup.constraintCount;
        for (int i = 0; i < horseCount; i++)
        {
            var betSlotChildIndex = (i + 1) * horseCount + i;
            var pos = (betslotGridLayoutGroup.transform.GetChild(betSlotChildIndex).transform as RectTransform).anchoredPosition;
            (this.transform.GetChild(i).transform as RectTransform).anchoredPosition = pos;
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}