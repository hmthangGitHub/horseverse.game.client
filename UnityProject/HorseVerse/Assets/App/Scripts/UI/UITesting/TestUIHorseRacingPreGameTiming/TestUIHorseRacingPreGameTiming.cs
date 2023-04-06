using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIHorseRacingPreGameTiming : TestUIScript<UIHorseRacingPreGameTiming, UIHorseRacingPreGameTiming.Entity>
{
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            uiTest.StartTiming();
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            uiTest.StopTimming();
        }
    }
}