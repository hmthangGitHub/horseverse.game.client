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
        for (int i = 0; i < instanceList.Count; i++)
        {
            var col = i % horseCount;
            var row = i / horseCount;
            instanceList[i].SetVisible(row < col);
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