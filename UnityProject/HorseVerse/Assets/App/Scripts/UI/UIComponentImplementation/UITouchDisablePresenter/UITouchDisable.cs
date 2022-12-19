using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITouchDisable : PopupEntity<UITouchDisable.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public bool loadingHorse;
    }

    public IsVisibleComponent loadingHorse;

    protected override void OnSetEntity()
    {
	    loadingHorse.SetEntity(this.entity.loadingHorse);
    }
}	