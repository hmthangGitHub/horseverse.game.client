using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITraditionalRoomType : UIComponent<UITraditionalRoomType.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public ButtonComponent.Entity btn;
	    public bool isLock;
    }

    public ButtonComponent btn;
    public IsVisibleComponent isLock;
    
    protected override void OnSetEntity()
    {
	    btn.SetEntity(this.entity.btn);
		isLock.SetEntity(this.entity.isLock);
    }
}	