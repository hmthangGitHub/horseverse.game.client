using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIHorseRacingController : TestUIScript<UIHorseRacingController, UIHorseRacingController.Entity>
{
    protected override void SetEntity()
    {
        this.entity.cameraBtn.onDown = () => { Debug.Log("Down"); };
        this.entity.cameraBtn.onUp = () => { Debug.Log("Up"); };
        base.SetEntity();
    }

    protected override void OnGUI()
    {
        base.OnGUI();
    }
}