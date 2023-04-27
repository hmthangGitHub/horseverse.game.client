using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIDebugMenuObjectModifier : TestUIScript<UIDebugMenuObjectModifier, UIDebugMenuObjectModifier.Entity>
{
    protected override void SetEntity()
    {
        entity.objectToInspect = this;
        base.SetEntity();
    }
}