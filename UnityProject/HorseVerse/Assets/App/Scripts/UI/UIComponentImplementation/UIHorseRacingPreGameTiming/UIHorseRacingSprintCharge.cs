using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodiceApp;
using UnityEngine;

public class UIHorseRacingSprintCharge : UIComponent<UIHorseRacingSprintCharge.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public int chargeNumber;
	    public float progress;
    }

    public Color colorMin;
    public Color colorMax;
    public UIHorseRacingSprintChargeList sprintCharge;
    public UIComponentImageProgress progress;
    
    protected override void OnSetEntity()
    {
	    sprintCharge.SetEntity(Enumerable.Range(0, this.entity.chargeNumber).Select((x, i) => new UIHorseRacingSprintChargeItem.Entity()
	    {
		    color = Color.Lerp(colorMin, colorMax, (float)(i + 1) / (this.entity.chargeNumber))
	    }).ToArray());
	    progress.SetEntity(1f - this.entity.progress);
	    progress.transform.SetAsLastSibling();
    }

    public void SetProgress(float progress)
    {
	    this.progress.SetEntity(1f - progress);
    }
}	