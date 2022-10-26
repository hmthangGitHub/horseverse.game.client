using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public PlatformTest platformTest;
    public Collider collider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrainingHorse"))
        {
            platformTest.OnJump.Invoke();
            collider.enabled = false;
        }
    }
}
