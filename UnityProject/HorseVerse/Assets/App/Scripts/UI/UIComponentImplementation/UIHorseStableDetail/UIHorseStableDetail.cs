using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHorseStableDetail : PopupEntity<UIHorseStableDetail.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIComponentHorseDetailInfo.Entity info;
	    public UIHorseStableBriefInfo.Entity briefInfo;
	    public UIComponentHorseStableDetailTab.Entity tab;
    }

    public UIComponentHorseDetailInfo info;
    public UIHorseStableBriefInfo briefInfo;
    public UIComponentHorseStableDetailTab tab;
    
    protected override void OnSetEntity()
    {
	    info.SetEntity(this.entity.info);
	    tab.SetEntity(this.entity.tab);
	    briefInfo.SetEntity(this.entity.briefInfo);
    }
}	