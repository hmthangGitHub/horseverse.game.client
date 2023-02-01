using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRacingMode : PopupEntity<UIRacingMode.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public ButtonComponent.Entity traditionalBtn;
	    public ButtonComponent.Entity stableVsStableBtn;
	    public ButtonComponent.Entity rankBtn;
	    public ButtonComponent.Entity tournamentBtn;
	    public ButtonComponent.Entity historyBtn;
	    
	    public bool isTraditionalLock;
	    public bool isStableVsStableLock;
	    public bool isRankLock;
	    public bool isTournamentLock;
    }
    
    public ButtonComponent traditionalBtn;
    public ButtonComponent stableVsStableBtn;
    public ButtonComponent rankBtn;
    public ButtonComponent tournamentBtn;
    public ButtonComponent historyBtn;
    public IsVisibleComponent isTraditionalLock;
    public IsVisibleComponent isStableVsStableLock;
    public IsVisibleComponent isRankLock;
    public IsVisibleComponent isTournamentLock;

    protected override void OnSetEntity()
    {
	    traditionalBtn.SetEntity(this.entity.traditionalBtn);
	    stableVsStableBtn.SetEntity(this.entity.stableVsStableBtn);
	    rankBtn.SetEntity(this.entity.rankBtn);
	    tournamentBtn.SetEntity(this.entity.tournamentBtn);
	    historyBtn.SetEntity(this.entity.historyBtn);
	    isTraditionalLock.SetEntity(this.entity.isTraditionalLock);
	    isStableVsStableLock.SetEntity(this.entity.isStableVsStableLock);
	    isRankLock.SetEntity(this.entity.isRankLock);
	    isTournamentLock.SetEntity(this.entity.isTournamentLock);
    }
}	