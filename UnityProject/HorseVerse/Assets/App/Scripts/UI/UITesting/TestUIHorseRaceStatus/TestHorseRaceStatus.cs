using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestHorseRaceStatus : TestUIScript<UIHorseRaceStatus, UIHorseRaceStatus.Entity>
{
    public int changePos;
    public int changeTo;

    protected override void OnGUI()
    {
        base.OnGUI();
        if (GUILayout.Button("Change Pos"))
        {
            var randomList = Enumerable.Range(0, this.entity.playerList.horseIdInLane.Length)
                                       .Shuffle()
                                       .ToArray();
            for (int i = 0; i < this.uiTest.playerList.horseRaceStatusPlayerList.Count; i++)
            {
                this.uiTest.playerList.ChangePosition(i, randomList[i]);
            }
        }
    }
}
