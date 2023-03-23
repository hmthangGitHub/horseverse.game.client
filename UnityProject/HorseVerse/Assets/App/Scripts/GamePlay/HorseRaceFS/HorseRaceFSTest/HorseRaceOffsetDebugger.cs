using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRaceOffsetDebugger : MonoBehaviour
{
    public Transform transform1;
    public Transform transform2;

    private void Update()
    {
        Debug.DrawLine(transform1.position, transform2.position, Color.blue, 5.0f);
    }
}
