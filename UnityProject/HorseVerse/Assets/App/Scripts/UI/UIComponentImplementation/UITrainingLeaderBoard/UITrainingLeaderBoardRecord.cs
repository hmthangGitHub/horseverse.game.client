using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrainingLeaderBoardRecord : UIComponent<UITrainingLeaderBoardRecord.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public string horseName;
	    public int rank;
	    public int highestScore;
	    public int maxRank;
	    public UITrainingLeaderBoardRankType.RankType rankContainer;
    }

    public FormattedTextComponent horseName;
    public FormattedTextComponent rank;
    public FormattedTextComponent userRank;
    public FormattedTextComponent highestScore;
    public FormattedTextComponent maxRank;
    public UITrainingLeaderBoardRankType rankContainer;
    
    protected override void OnSetEntity()
    {
	    horseName.SetEntity(this.entity.horseName);
	    rank.SetEntity(this.entity.rank);
	    userRank.SetEntity(this.entity.rank);
	    highestScore.SetEntity(this.entity.highestScore);
	    rankContainer.SetEntity(this.entity.rankContainer);
	    maxRank.SetEntity(this.entity.maxRank);
    }
}	