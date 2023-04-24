using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBreedSlot : UIComponent<UIBreedSlot.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIBreedSlotState.State state;
	    public UIComponentCountDownTimer.Entity breedingTimeLeft;
	    public ButtonComponent.Entity emptySlotBtn;
	    public ButtonComponent.Entity checkingFoalBtn;
    }

    public UIBreedSlotState state;
    public UIComponentCountDownTimer breedingTimeLeft;
    public ButtonComponent emptySlotBtn;
    public ButtonComponent checkingFoalBtn;
    
    protected override void OnSetEntity()
    {
	    state.SetEntity(entity.state);
	    breedingTimeLeft.SetEntity(entity.breedingTimeLeft);
	    emptySlotBtn.SetEntity(entity.emptySlotBtn);
	    checkingFoalBtn.SetEntity(entity.checkingFoalBtn);
    }
}	