using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseRacingThirdPersonResult : PopupEntity<UIHorseRacingThirdPersonResult.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public int position;
	    public int raceTime;
	    public ButtonComponent.Entity outerBtn;
	    public UIComponentRealmIntro.Entity realm;
    }

    public UIComponentOrdinalNumber position;
    public UIComponentTimeSpan raceTime;
    public ButtonComponent outerBtn;
    public UIComponentRealmIntro realm;
    
    protected override void OnSetEntity()
    {
	    position.SetEntity(this.entity.position);
	    raceTime.SetEntity(this.entity.raceTime);
	    outerBtn.SetEntity(this.entity.outerBtn);
	    realm.SetEntity(this.entity.realm);
    }
}	