using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIBetMode : TestUIScript<UIBetMode, UIBetMode.Entity>
{
    public override void SetEntity()
    {
        this.entity.quickBetButtonsContainer.onBetAll += i => Debug.Log($"onBetAll {i}");
        base.SetEntity();
    }
}