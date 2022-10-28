using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugMasterColumnList : UIComponentList<UIDebugMasterColumn, UIDebugMasterColumn.Entity>
{
    public GridLayoutGroup gridLayoutGroup;

    protected override void OnSetEntity()
    {
        var headerNumber = this.entity.entities.Count(x => x.isHeader);
        gridLayoutGroup.constraintCount = headerNumber;
        base.OnSetEntity();
    }
}	