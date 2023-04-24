using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseBreedingCandidate : UIComponent<UIHorseBreedingCandidate.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public HorseLoader.Entity horseLoader;
	    public UIHorseStableBriefInfo.Entity briefInfo;
	    public ButtonComponent.Entity chooseHorseBtn;
	    public int breedCount;
	    public int maxBreedCount;
	    public bool isShowBriefInfo;
    }

    public HorseLoader horseLoader;
    public UIHorseStableBriefInfo briefInfo;
    public FormattedTextComponent breedCount;
    public ButtonComponent chooseHorseBtn;
    public IsVisibleComponent isShowBriefInfo;
    public IsVisibleComponent isShowBreedCount;
    public IsVisibleComponent emptyHorseSymbol;
    
    protected override void OnSetEntity()
    {
	    horseLoader.SetEntity(entity.horseLoader);
	    briefInfo.SetEntity(entity.briefInfo);
	    breedCount.SetEntity(entity.breedCount, entity.maxBreedCount);
	    chooseHorseBtn.SetEntity(entity.chooseHorseBtn);
	    SetVisibleHorse();
    }

    private void SetVisibleHorse()
    {
	    isShowBriefInfo.SetEntity(entity.isShowBriefInfo);
	    isShowBreedCount.SetEntity(entity.isShowBriefInfo);
	    emptyHorseSymbol.SetEntity(!entity.isShowBriefInfo);
    }
}	