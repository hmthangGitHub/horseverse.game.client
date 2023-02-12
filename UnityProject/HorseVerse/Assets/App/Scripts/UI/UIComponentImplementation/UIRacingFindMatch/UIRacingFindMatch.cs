using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRacingFindMatch : PopupEntity<UIRacingFindMatch.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public bool findMatchPopUpVisible;
	    public bool gameStartingPopupVisible;
	    public int waitingTime;
	    public int numberConnectPlayer;
	    public ButtonComponent.Entity cancelBtn;
    }

    public IsVisibleComponent findMatchPopUpVisible;
    public IsVisibleComponent gameStartingPopupVisible;
    public FormattedTextComponent waitingTime;
    public FormattedTextComponent numberConnectPlayer;
    public ButtonComponent cancelBtn;
    
    protected override void OnSetEntity()
    {
	    findMatchPopUpVisible.SetEntity(this.entity.findMatchPopUpVisible);
	    gameStartingPopupVisible.SetEntity(this.entity.gameStartingPopupVisible);
	    SetWaitingTime(this.entity.waitingTime);
	    numberConnectPlayer.SetEntity(this.entity.numberConnectPlayer);
	    cancelBtn.SetEntity(this.entity.cancelBtn);
    }

    public void SetWaitingTime(int time)
    {
	    var ts = TimeSpan.FromSeconds(time);
	    waitingTime.SetEntity($"{ts.Minutes}:{ts.Seconds}");
    }
}	