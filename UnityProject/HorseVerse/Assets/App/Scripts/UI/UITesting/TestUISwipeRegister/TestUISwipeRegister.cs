using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUISwipeRegister : TestUIScript<UISwipeRegister, UISwipeRegister.Entity>
{
    protected override void OnGUI()
    {
        entity.OnSwipeDirection = OnSwipeDirection;
        entity.OnHorizontalDirection = OnSwipeDirection;
        base.OnGUI();
    }

    private void OnSwipeDirection(UISwipeRegister.Direction d)
    {
        Debug.Log(d);
    }
}