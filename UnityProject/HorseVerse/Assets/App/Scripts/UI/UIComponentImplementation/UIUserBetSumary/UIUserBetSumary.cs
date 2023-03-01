using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUserBetSumary : PopupEntity<UIUserBetSumary.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public long matchId;
	    public long time;
	    public UIComponentHorseInfoHistory.Entity firstHorse;
	    public UIComponentHorseInfoHistory.Entity secondHorse;
	    public UIComponentBetModeMyResult.Entity[] userBetRecordList;
	    public ButtonComponent.Entity closeBtn;
    }
    
    public FormattedTextComponent matchId;
    public UIComponentHistoryRecordTimeStamp time;
    public UIComponentHorseInfoHistory firstHorse;
    public UIComponentHorseInfoHistory secondHorse;
    public UIComponentBetModeMyResultList userBetRecordList;
    public ButtonComponent closeBtn;

    protected override void OnSetEntity()
    {
	    matchId.SetEntity(this.entity.matchId);
	    time.SetEntity(this.entity.time);
	    firstHorse.SetEntity(this.entity.firstHorse);
	    secondHorse.SetEntity(this.entity.secondHorse);
	    userBetRecordList.SetEntity(this.entity.userBetRecordList);
	    closeBtn.SetEntity(this.entity.closeBtn);
    }
}	