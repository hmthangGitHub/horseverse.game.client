using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIComponentQuickBetButtonsContainer : UIComponent<UIComponentQuickBetButtonsContainer.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public Action<int> onBetAll;
        public Action<int> onBetVertical;
        public Action<int> onBetHorizontal;
    }

    public Action<int> onBetAll = ActionUtility.EmptyAction<int>.Instance;
    public Action<int> onBetVertical = ActionUtility.EmptyAction<int>.Instance;
    public Action<int> onBetHorizontal = ActionUtility.EmptyAction<int>.Instance;

    protected override void OnSetEntity()
    {
        onBetAll = ActionUtility.EmptyAction<int>.Instance;
        onBetVertical = ActionUtility.EmptyAction<int>.Instance;
        onBetHorizontal = ActionUtility.EmptyAction<int>.Instance;

        onBetAll += this.entity.onBetAll;
        onBetVertical += this.entity.onBetVertical;
        onBetHorizontal += this.entity.onBetHorizontal;

        for (int i = 0; i < this.transform.childCount; i++)
        {
            var horseIndex = i + 1;
            var quickBetButton = this.transform.GetChild(i).GetComponentInChildren<UIComponentQuickBetButtons>();
            quickBetButton.SetEntity(new UIComponentQuickBetButtons.Entity()
            {
                betAllBtn = new ButtonComponent.Entity(() => this.onBetAll.Invoke(horseIndex)),
            });
        }
    }

    public void SetInteractable(bool isInteractable)
    {
        transform.GetComponentsInChildren<UIComponentQuickBetButtons>()
                 .ForEach(x => x.betAllBtn.SetInteractable(isInteractable));
    }

    private void OnEnable()
    {
        if (betslotGridLayoutGroup.transform.childCount > 1)
        {
            ArrangeQuickBetButtons().Forget();
        }
    }

    private async UniTaskVoid ArrangeQuickBetButtons()
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        var horseCount = betslotGridLayoutGroup.constraintCount;
        for (int i = 0; i < horseCount; i++)
        {
            var betSlotChildIndex = i * horseCount + (i + 1);
            var pos = (betslotGridLayoutGroup.transform.GetChild(betSlotChildIndex).transform as RectTransform).anchoredPosition;
            (this.transform.GetChild(i).transform as RectTransform).anchoredPosition = pos;
        }
    }

    public UnityEngine.UI.GridLayoutGroup betslotGridLayoutGroup;

#if UNITY_EDITOR
    [ContextMenu("Arrange Quick Bet Buttons")]
    public void ArrangeQuickBetButtonsEditor()
    {
        ArrangeQuickBetButtons().Forget();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}