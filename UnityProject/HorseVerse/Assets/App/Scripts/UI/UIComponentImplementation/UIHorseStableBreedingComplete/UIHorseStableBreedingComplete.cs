using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseStableBreedingComplete : PopupEntity<UIHorseStableBreedingComplete.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public HorseLoader.Entity horseLoader;
	    public ButtonComponent.Entity cancelBtn;
	    public ButtonComponent.Entity moveToStableBtn;
	    public ButtonComponent.Entity moveToYardBtn;
	    public int currentSlot;
	    public int maxSlot;
	    public UIHorseStableBriefInfo.Entity briefInfo;
    }
    
    public HorseLoader horseLoader;
    public ButtonComponent cancelBtn;
    public ButtonComponent moveToStableBtn;
    public ButtonComponent moveToYardBtn;
    public FormattedTextComponent emptySlotText;
    public UIHorseStableBriefInfo briefInfo;

    protected override void OnSetEntity()
    {
	    horseLoader.SetEntity(this.entity.horseLoader);
		    cancelBtn.SetEntity(this.entity.cancelBtn);
	    moveToStableBtn.SetEntity(this.entity.moveToStableBtn);
		    moveToYardBtn.SetEntity(this.entity.moveToYardBtn);
	    emptySlotText.SetEntity(this.entity.currentSlot, this.entity.maxSlot);
	    briefInfo.SetEntity(this.entity.briefInfo);
    }
}	