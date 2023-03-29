using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHorseRacingSprintChargeItem : UIComponent<UIHorseRacingSprintChargeItem.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public Color color;
    }

    public Image image;
    
    protected override void OnSetEntity()
    {
	    image.color = this.entity.color;
    }
}	