using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransformTapRegister : PopupEntity<UITransformTapRegister.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIComponentButtonPin.Entity[] buttonPinList;
    }

    public UIComponentButtonPinList buttonPinList;
    
    protected override void OnSetEntity()
    {
	    buttonPinList.SetEntity(this.entity.buttonPinList);
    }
}	