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
    public UIComponentOrdinalNumber rank;
    public UIComponentOrdinalNumber userRank;
    public FormattedTextComponent highestScore;
    public UIComponentOrdinalNumber maxRank;
    public UITrainingLeaderBoardRankType rankContainer;
    public IsVisibleComponent leaderBoardRecord1stBorder;
    public IsVisibleComponent leaderBoardRecordBorder;
    
    protected override void OnSetEntity()
    {
	    horseName.SetEntity(this.entity.horseName);
	    rank.SetEntity(this.entity.rank);
	    userRank.SetEntity(this.entity.rank);
	    highestScore.SetEntity(this.entity.highestScore);
	    maxRank.SetEntity(this.entity.maxRank);
	    leaderBoardRecord1stBorder.SetEntity(this.entity.rank != 1);
	    leaderBoardRecord1stBorder.SetEntity(this.entity.rank == 1);
	    rankContainer.SetEntity(this.entity.rankContainer);
    }
}	