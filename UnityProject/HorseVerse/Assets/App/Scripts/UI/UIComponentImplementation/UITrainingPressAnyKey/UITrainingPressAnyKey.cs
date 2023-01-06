using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

public class UITrainingPressAnyKey : PopupEntity<UITrainingPressAnyKey.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public ButtonComponent.Entity outerBtn;
    }
    
    public ButtonComponent outerBtn;

    protected override void OnSetEntity()
    {
	    outerBtn.SetEntity(this.entity.outerBtn);
    }
}	