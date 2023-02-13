using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentRaceRoomInfo : UIComponent<UIComponentRaceRoomInfo.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UIComponentHorseRankRequirement.HorseRank horseRankRequirement;
	    public UIComponentRaceRewardGroup.Entity rewardGroup1st;
	    public UIComponentRaceRewardGroup.Entity rewardGroup2nd;
	    public UIComponentRaceRewardGroup.Entity rewardGroup3rd;
    }
    
    public UIComponentHorseRankRequirement horseRankRequirement;
    public UIComponentRaceRewardGroup rewardGroup1st;
    public UIComponentRaceRewardGroup rewardGroup2nd;
    public UIComponentRaceRewardGroup rewardGroup3rd;

    protected override void OnSetEntity()
    {
	    horseRankRequirement.SetEntity(this.entity.horseRankRequirement);
	    rewardGroup1st.SetEntity(this.entity.rewardGroup1st);
	    rewardGroup2nd.SetEntity(this.entity.rewardGroup2nd);
	    rewardGroup3rd.SetEntity(this.entity.rewardGroup3rd);
    }
}	