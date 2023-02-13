using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHistoryRecordHorseRank : UIComponent<UIComponentHistoryRecordHorseRank.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public int rank;
    }

    public FormattedTextComponent otherRankText;
    public UIComponentEnumInt horseRank;
    
    protected override void OnSetEntity()
    {
	    otherRankText.SetEntity(this.entity.rank);
	    horseRank.SetEntity( Mathf.Min(this.entity.rank, 3) - 1);
    }
}	