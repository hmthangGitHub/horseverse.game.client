using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseStableBreedingPreview : PopupEntity<UIHorseStableBreedingPreview.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public float commonOdd;
	    public float uncommonOdd;
	    public float rareOdd;
	    public float epicOdd;
	    public float legendOdd;
	    public float speed;
	    public float sprintSpeed;
	    public float stamina;
	    public int rubyNeedToBreed;
	    public UIHorseBreedingCandidate.Entity maleBreedingHorse;
	    public UIHorseBreedingCandidate.Entity femaleBreedingHorse;
	    public ButtonComponent.Entity breedingBtn;
    }

    public FormattedTextComponent rubyNeedToBreed;
    public FormattedTextComponent commonOdd;
    public FormattedTextComponent uncommonOdd;
    public FormattedTextComponent rareOdd;
    public FormattedTextComponent epicOdd;
    public FormattedTextComponent legendOdd;
    public FormattedTextComponent speed;
    public FormattedTextComponent sprintSpeed;
    public FormattedTextComponent stamina;
    public UIHorseBreedingCandidate maleBreedingHorse;
    public UIHorseBreedingCandidate femaleBreedingHorse;
    public ButtonComponent breedingBtn;
    
    protected override void OnSetEntity()
    {
	    commonOdd.SetEntity(Math.Round(entity.commonOdd, 2));
	    uncommonOdd.SetEntity(Math.Round(entity.uncommonOdd, 2));
	    rareOdd.SetEntity(Math.Round(entity.rareOdd, 2));
	    epicOdd.SetEntity(Math.Round(entity.epicOdd, 2));
	    legendOdd.SetEntity(Math.Round(entity.legendOdd, 2));
	    speed.SetEntity(Math.Round(entity.speed, 2));
	    sprintSpeed.SetEntity(Math.Round(entity.sprintSpeed, 2));
	    stamina.SetEntity(Math.Round(entity.stamina, 2));
	    maleBreedingHorse.SetEntity(entity.maleBreedingHorse);
	    femaleBreedingHorse.SetEntity(entity.femaleBreedingHorse);
	    breedingBtn.SetEntity(entity.breedingBtn);
	    rubyNeedToBreed.SetEntity(entity.rubyNeedToBreed);
    }
}	