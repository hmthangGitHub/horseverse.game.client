using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBetHistory : PopupEntity<UIBetHistory.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIBetHistoryRecord.Entity[] historyContainer;
	    
    }

    public UIBetHistoryRecordList historyContainer;
    
    protected override void OnSetEntity()
    {
	    historyContainer.SetEntity(this.entity.historyContainer);
    }
}	