using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentDoubleBetList : UIComponentList<UIComponentBetSlot, UIComponentBetSlot.Entity>
{
    public UnityEngine.UI.GridLayoutGroup gridLayoutGroup;
    protected override void OnSetEntity()
    {
        base.OnSetEntity();
        ArrangeSlot();
    }

    public void ArrangeSlot()
    {
        var horseCount = gridLayoutGroup.constraintCount;
        for (int i = 1; i < this.transform.childCount; i++)
        {
            var col = (i - 1) % horseCount;
            var row = (i - 1) / horseCount;
            this.transform.GetChild(i).GetComponent<UIComponentBetSlot>().SetVisible(row <= col);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("ArrangeSlot")]
    public void ArrangeSlotEditor()
    {
        ArrangeSlot();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}	