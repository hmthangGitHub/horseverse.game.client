using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseTraningMapSelection : UIComponent<UIComponentHorseTraningMapSelection.Entity>
{
	[System.Serializable]
    public class Entity
    {
        public UIComponentToggleGroup.Entity mapToggleGroup;
    }

    public UIComponentToggleGroup mapToggleGroup;

    protected override void OnSetEntity()
    {
        mapToggleGroup.SetEntity(this.entity.mapToggleGroup);
    }
}	