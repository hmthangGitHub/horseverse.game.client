using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseRealmIntro : PopupEntity<UIHorseRealmIntro.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIComponentRealmIntro.Entity realm;
        public ButtonComponent.Entity btnSkip;
    }

    public UIComponentRealmIntro realm;
    public ButtonComponent btnSkip;

    protected override void OnSetEntity()
    {
	    realm.SetEntity(this.entity.realm);
        btnSkip.SetEntity(this.entity.btnSkip);
    }
}	