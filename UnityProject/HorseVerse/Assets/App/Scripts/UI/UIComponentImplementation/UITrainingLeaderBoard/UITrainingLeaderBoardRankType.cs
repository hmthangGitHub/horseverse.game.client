using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrainingLeaderBoardRankType : UIComponentEnum<UITrainingLeaderBoardRankType.RankType>
{
	[System.Serializable]
    public enum RankType
    {
	    None,
	    Rank,
	    UserRank,
	    OutOfLeaderBoard
    }
}