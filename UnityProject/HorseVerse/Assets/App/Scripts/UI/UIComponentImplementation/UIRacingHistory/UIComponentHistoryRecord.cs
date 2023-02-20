using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHistoryRecord : UIComponent<UIComponentHistoryRecord.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public long matchId;
	    public long time;
	    public int coinNumber;
	    public int chestNumber;
	    public UIComponentHistoryRecordHorseRank.Entity horseRank;
	    public int horseIndex;
	    public string horseName;
	    public ButtonComponent.Entity viewRaceScriptBtn;
	    public ButtonComponent.Entity viewResultBtn;
    }
    
    public FormattedTextComponent matchId;
    public UIComponentHistoryRecordTimeStamp time;
    public FormattedTextComponent coinNumber;
    public FormattedTextComponent chestNumber;
    public FormattedTextComponent horseName;
    public UIComponentHistoryRecordHorseRank horseRank;
    public UIComponentEnumInt horseIndex;
    public ButtonComponent viewRaceScriptBtn;
    public ButtonComponent viewResultBtn;

    protected override void OnSetEntity()
    {
	    matchId.SetEntity(this.entity.matchId);
	    time.SetEntity(this.entity.time);
	    coinNumber.SetEntity(this.entity.coinNumber);
	    chestNumber.SetEntity(this.entity.chestNumber);
	    horseRank.SetEntity(this.entity.horseRank);
	    horseIndex.SetEntity(this.entity.horseIndex);
	    viewRaceScriptBtn.SetEntity(this.entity.viewRaceScriptBtn);
	    viewResultBtn.SetEntity(this.entity.viewResultBtn);
	    horseName.SetEntity(this.entity.horseName);
    }
}	