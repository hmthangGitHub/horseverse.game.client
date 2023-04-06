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
	    public int currentPosition;
	    public int maxPosition;
	    public int currentLap;
	    public int maxLap;
    }

    public UIComponentProgressBar sprintBar;
    public UIHorseRacingSprintCharge sprintCharge;
    public ButtonComponent sprintBtn;
    public UIComponentHoldImageBehavior cameraBtn;
    public FormattedTextComponent position;
    public FormattedTextComponent lap;
    
    protected override void OnSetEntity()
    {
	     sprintBar.SetEntity(this.entity.sprintBar);
	     sprintCharge.SetEntity(this.entity.sprintCharge);
	     sprintBtn.SetEntity(this.entity.sprintBtn);
	     cameraBtn.SetEntity(this.entity.cameraBtn);
	     position.SetEntity(this.entity.currentPosition, this.entity.maxPosition);
	     lap.SetEntity(this.entity.currentLap, this.entity.maxLap);
    }

    public void SetSprintTime(float currentSprintNormalizeTime)
    {
	    sprintBar.SetEntity(currentSprintNormalizeTime);
	    sprintBar.gameObject.SetActive(currentSprintNormalizeTime > 0);
    }

    public void SetCurrentPosition(int currentPosition)
    {
	    this.entity.currentPosition = currentPosition;
	    position.SetEntity(this.entity.currentPosition, this.entity.maxPosition);
    }
    
    public void SetCurrentLap(int currentLap)
    {
	    this.entity.currentLap = currentLap;
	    lap.SetEntity(this.entity.currentLap, this.entity.maxLap);
    }
}