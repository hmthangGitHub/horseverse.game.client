using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrainingLeaderBoard : PopupEntity<UITrainingLeaderBoard.Entity>
{
	[System.Serializable]
    public class Entity
    {
	    public UITrainingLeaderBoardRecord.Entity[] leaderBoard;
	    public UITrainingLeaderBoardRecord.Entity userRank;
	    public ButtonComponent.Entity closeBtn;
    }

    public UITrainingLeaderBoardRecord userRank;
    public ButtonComponent closeBtn;
    public UITrainingLeaderBoardRecordList leaderBoard;
    
    protected override void OnSetEntity()
    {
	    userRank.SetEntity(this.entity.userRank);
		leaderBoard.SetEntity(this.entity.leaderBoard);
		closeBtn.SetEntity(this.entity.closeBtn);
    }
}	