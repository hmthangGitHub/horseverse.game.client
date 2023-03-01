using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseInfoHistory : UIComponent<UIComponentHorseInfoHistory.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public string horseName;
	    public int horseIndex;
	    public int horseRank;
    }

    public UIComponentEnumInt horseIndex;
    public FormattedTextComponent horseName;
    public UIComponentHistoryRecordHorseRank horseRank;
    
    protected override void OnSetEntity()
    {
	    horseIndex.SetEntity(this.entity.horseIndex);
	    horseName.SetEntity(this.entity.horseName);
	    horseRank.SetEntity(this.entity.horseRank);
    }
}	