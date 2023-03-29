using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseRacingController : PopupEntity<UIHorseRacingController.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public float sprintBar;
	    public UIHorseRacingSprintCharge.Entity sprintCharge; 
	    public ButtonComponent.Entity sprintBtn;
	    public UIComponentHoldImageBehavior.Entity cameraBtn;
    }

    public UIComponentProgressBar sprintBar;
    public UIHorseRacingSprintCharge sprintCharge;
    public ButtonComponent sprintBtn;
    public UIComponentHoldImageBehavior cameraBtn;
    
    protected override void OnSetEntity()
    {
	     sprintBar.SetEntity(this.entity.sprintBar);
	     sprintCharge.SetEntity(this.entity.sprintCharge);
	     sprintBtn.SetEntity(this.entity.sprintBtn);
	     cameraBtn.SetEntity(this.entity.cameraBtn);
    }

    public void SetSprintTime(float currentSprintNormalizeTime)
    {
	    sprintBar.SetEntity(currentSprintNormalizeTime);
	    sprintBar.gameObject.SetActive(currentSprintNormalizeTime > 0);
    }
}