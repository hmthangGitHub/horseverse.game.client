using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBetHistoryRecord : UIComponent<UIBetHistoryRecord.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public long matchId;
	    public long time;
	    public UIComponentHorseInfoHistory.Entity firstHorse;
	    public UIComponentHorseInfoHistory.Entity secondHorse;
	    public ButtonComponent.Entity viewBetDetailBtn;
	    public ButtonComponent.Entity viewResultBtn;
	    public ButtonComponent.Entity viewRaceScriptBtn;
    }
    
    public FormattedTextComponent matchId;
    public UIComponentHistoryRecordTimeStamp time;
    public UIComponentHorseInfoHistory firstHorse;
    public UIComponentHorseInfoHistory secondHorse;
    public ButtonComponent viewBetDetailBtn;
    public ButtonComponent viewResultBtn;
    public ButtonComponent viewRaceScriptBtn;

    protected override void OnSetEntity()
    {
	    matchId.SetEntity(this.entity.matchId);
	    time.SetEntity(this.entity.time);
	    firstHorse.SetEntity(this.entity.firstHorse);
	    secondHorse.SetEntity(this.entity.secondHorse);
	    viewBetDetailBtn.SetEntity(this.entity.viewBetDetailBtn);
	    viewResultBtn.SetEntity(this.entity.viewResultBtn);
	    viewRaceScriptBtn.SetEntity(this.entity.viewRaceScriptBtn);
    }
}	