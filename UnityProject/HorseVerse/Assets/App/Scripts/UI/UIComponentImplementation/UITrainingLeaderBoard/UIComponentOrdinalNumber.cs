using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentOrdinalNumber : UIComponent<UIComponentOrdinalNumber.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public int ordinalNumber;
    }

    public FormattedTextComponent ordinalNumber;
    
    protected override void OnSetEntity()
    {
	    ordinalNumber.SetEntity(this.entity.ordinalNumber, this.entity.ordinalNumber switch
	    {
		    1 => "ST",
		    2 => "ND",
		    3 => "RD",
		    _ => "TH"
	    });
    }

    public void SetEntity(int ordinalNumber)
    {
	    SetEntity(new Entity()
	    {
		    ordinalNumber = ordinalNumber
	    });
    }
}	