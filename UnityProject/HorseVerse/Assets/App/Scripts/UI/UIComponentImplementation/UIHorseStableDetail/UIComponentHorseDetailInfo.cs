using System;
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
    	    public float coinCollected;
    	    public float coinCollectedMax;
    	    public float breedCount;
    	    public float breedCountMax;
    	    public float agility;
    	    public float sprintTime;
    	    public float stamina;
    	    public float staminaRecovery;
        }
    
        public FormattedTextComponent speed;
        public FormattedTextComponent acceleration;
        public FormattedTextComponent agility;
        public FormattedTextComponent coinCollected;
        public FormattedTextComponent breedCount;
        public FormattedTextComponent sprintTime;
        public FormattedTextComponent stamina;
        public FormattedTextComponent staminaRecovery;
        
        protected override void OnSetEntity()
        {
    	    speed.SetEntity(Math.Round(this.entity.speed, 1));
    	    acceleration.SetEntity(Math.Round(this.entity.acceleration, 1));
    	    agility.SetEntity(Math.Round(this.entity.agility, 1));
    	    coinCollected.SetEntity(this.entity.coinCollected, this.entity.coinCollectedMax);
    	    breedCount.SetEntity(this.entity.breedCount, this.entity.breedCountMax);
    	    sprintTime.SetEntity(Math.Round(this.entity.sprintTime, 1));
    	    stamina.SetEntity(Math.Round(this.entity.stamina, 1));
    	    staminaRecovery.SetEntity(Math.Round(this.entity.staminaRecovery, 1));
        }
}	