using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentRealmIntro : UIComponent<UIComponentRealmIntro.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public string realm;
	    public string track;
    }

    public FormattedTextComponent realm;
    public FormattedTextComponent track;

    protected override void OnSetEntity()
    {
	    realm.SetEntity(this.entity.realm);
	    track.SetEntity(this.entity.track);
    }
}	