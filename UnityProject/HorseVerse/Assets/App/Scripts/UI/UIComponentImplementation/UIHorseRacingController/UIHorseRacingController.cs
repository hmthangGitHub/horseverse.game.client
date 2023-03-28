using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseRacingController : PopupEntity<UIHorseRacingController.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public float sprintBar;
	    public float sprintHealingProgress;
	    public ButtonComponent.Entity sprintBtn;
	    public UIComponentHoldImageBehavior.Entity cameraBtn;
    }

    public UIComponentProgressBar sprintBar;
    public UIComponentImageProgress sprintHealingProgress;
    public ButtonComponent sprintBtn;
    public UIComponentHoldImageBehavior cameraBtn;
    
    protected override void OnSetEntity()
    {
	     sprintBar.SetEntity(this.entity.sprintBar);
	     sprintHealingProgress.SetEntity(this.entity.sprintHealingProgress);
	     sprintBtn.SetEntity(this.entity.sprintBtn);
	     cameraBtn.SetEntity(this.entity.cameraBtn);
    }

    public void SetSprintTime(float currentSprintNormalizeTime)
    {
	    sprintBar.SetEntity(currentSprintNormalizeTime);
	    sprintBar.gameObject.SetActive(currentSprintNormalizeTime > 0);
    }
}