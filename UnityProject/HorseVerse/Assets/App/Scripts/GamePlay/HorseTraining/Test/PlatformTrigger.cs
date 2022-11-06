using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public Platform platform;
    public Collider collider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrainingHorse"))
        {
            platform.OnJump.Invoke();
            collider.enabled = false;
        }
    }
}
