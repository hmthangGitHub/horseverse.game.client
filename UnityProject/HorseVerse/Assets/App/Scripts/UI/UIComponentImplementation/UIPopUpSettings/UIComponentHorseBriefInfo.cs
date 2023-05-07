using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseBriefInfo : UIComponent<UIComponentHorseBriefInfo.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public string horseName;
	    public float agility;
	    public float acceleration;
	    public float speed;
	    public float stamina;
    }
    
    public FormattedTextComponent horseName;
    public FormattedTextComponent agility;
    public FormattedTextComponent acceleration;
    public FormattedTextComponent speed;
    public FormattedTextComponent stamina;

    protected override void OnSetEntity()
    {
	    horseName.SetEntity(this.entity.horseName);
	    agility.SetEntity(this.entity.agility);
	    acceleration.SetEntity(this.entity.acceleration);
	    speed.SetEntity(this.entity.speed);
	    stamina.SetEntity(this.entity.stamina);
    }
}	