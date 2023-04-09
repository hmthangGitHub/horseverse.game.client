using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseRealmIntro : PopupEntity<UIHorseRealmIntro.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIComponentRealmIntro.Entity realm;
    }

    public UIComponentRealmIntro realm;
    
    protected override void OnSetEntity()
    {
	    realm.SetEntity(this.entity.realm);
    }
}	