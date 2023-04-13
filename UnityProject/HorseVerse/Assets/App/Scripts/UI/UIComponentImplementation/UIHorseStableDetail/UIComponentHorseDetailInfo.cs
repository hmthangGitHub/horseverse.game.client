using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentHorseDetailInfo : UIComponent<UIComponentHorseDetailInfo.Entity>
{
	[System.Serializable]
        public class Entity
        {
    	    public float speed;
    	    public float acceleration;
    	    public float endurance;
    	    public float coinCollected;
    	    public float coinCollectedMax;
    	    public float breedCount;
    	    public float breedCountMax;
    	    public float sprintNumber;
    	    public float sprintSpeed;
    	    public float sprintTime;
    	    public float sprintEnergy;
    	    public float sprintRegen;
        }
    
        public FormattedTextComponent speed;
        public FormattedTextComponent acceleration;
        public FormattedTextComponent endurance;
        public FormattedTextComponent coinCollected;
        public FormattedTextComponent breedCount;
        public FormattedTextComponent sprintNumber;
        public FormattedTextComponent sprintSpeed;
        public FormattedTextComponent sprintTime;
        public FormattedTextComponent sprintEnergy;
        public FormattedTextComponent sprintRegen;
        
        protected override void OnSetEntity()
        {
    	    speed.SetEntity(this.entity.speed);
    	    acceleration.SetEntity(this.entity.acceleration);
    	    endurance.SetEntity(this.entity.endurance);
    	    coinCollected.SetEntity(this.entity.coinCollected, this.entity.coinCollectedMax);
    	    breedCount.SetEntity(this.entity.breedCount, this.entity.breedCountMax);
    	    sprintNumber.SetEntity(this.entity.sprintNumber);
    	    sprintSpeed.SetEntity(this.entity.sprintSpeed);
    	    sprintTime.SetEntity(this.entity.sprintTime);
    	    sprintEnergy.SetEntity(this.entity.sprintEnergy);
    	    sprintRegen.SetEntity(this.entity.sprintRegen);
        }
}	