using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrainingHorseDebugProperties : PopupEntity<UITrainingHorseDebugProperties.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UITrainingDebugHorsePropertyList.Entity propertyList;
    }

    public UITrainingDebugHorsePropertyList propertyList;
    
    protected override void OnSetEntity()
    {
	    propertyList.SetEntity(this.entity.propertyList);
    }
}	