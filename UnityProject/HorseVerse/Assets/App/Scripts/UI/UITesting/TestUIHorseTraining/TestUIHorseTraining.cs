using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIHorseTraining : TestUIScript<UIHorseTraining, UIHorseTraining.Entity>
{
    public override void SetEntity()
    {
        this.entity.prepareState.toTraningBtn = new ButtonComponent.Entity(() => {
            this.uiTest.ChangeState(UIComponentTraningState.TraningState.Processing);
        });
        base.SetEntity();
    }
}