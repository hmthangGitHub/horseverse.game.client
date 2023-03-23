using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIHorseRacingPreGameTiming : TestUIScript<UIHorseRacingPreGameTiming, UIHorseRacingPreGameTiming.Entity>
{
    protected override void OnGUI()
    {
        base.OnGUI();
        if (GUILayout.Button("StartTiming"))
        {
            uiTest.StartTiming();
        }
        
        if (GUILayout.Button("StopTiming"))
        {
            uiTest.StopTimming();
        }
    }
}