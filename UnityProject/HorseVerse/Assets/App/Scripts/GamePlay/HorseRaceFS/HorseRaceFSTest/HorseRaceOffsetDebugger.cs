using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRaceOffsetDebugger : MonoBehaviour
{
    public Transform transform1;
    public Transform transform2;

    private void LateUpdate()
    {
        Debug.DrawLine(transform1.position + Vector3.up * 0.5f, transform2.position + Vector3.up * 0.5f, Color.blue, 5.0f);
    }
}
