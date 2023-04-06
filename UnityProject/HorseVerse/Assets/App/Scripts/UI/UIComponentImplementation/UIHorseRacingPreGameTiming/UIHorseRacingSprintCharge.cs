using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIHorseRacingSprintCharge : UIComponent<UIHorseRacingSprintCharge.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public int chargeNumber;
	    public float progress;
    }

    public UIHorseRacingSprintChargeList sprintCharge;
    public UIComponentImageProgress emptyCharge;
    public UIComponentImageProgress chargeProgress;
    public int part;
    public float offset;
    public int maxCharge;
    private float percentagePerCharge;
    private float RawPercentagePerCharge => (1.0f - part * offset)/ part;
    
    protected override void OnSetEntity()
    {
	    percentagePerCharge = 1.0f / this.entity.chargeNumber;
	    emptyCharge.SetEntity(RawPercentagePerCharge * this.entity.chargeNumber + (this.entity.chargeNumber - 1) * offset);
	    SetProgress(this.entity.progress);
    }

    public void SetProgress(float progress)
    {
	    var normalizeOffset = Mathf.Floor(progress / percentagePerCharge) * this.offset;
	    chargeProgress.SetEntity(Mathf.Lerp(0, RawPercentagePerCharge * this.entity.chargeNumber, progress) + normalizeOffset);
    }
}	