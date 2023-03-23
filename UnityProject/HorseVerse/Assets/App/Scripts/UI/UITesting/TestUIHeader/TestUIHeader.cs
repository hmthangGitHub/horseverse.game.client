using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIHeader : TestUIScript<UIHeader, UIHeader.Entity>
{
    protected override void SetEntity()
    {
        entity.backBtn = new ButtonComponent.Entity(() =>
        {
            Debug.Log("Lmao");
        });
        base.SetEntity();
    }
}
