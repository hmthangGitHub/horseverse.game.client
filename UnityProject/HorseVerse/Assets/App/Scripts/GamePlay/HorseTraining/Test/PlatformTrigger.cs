using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public PlatformBase platform;
    public Collider collider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrainingHorse"))
        {
            platform.OnFinishPlatform.Invoke();
            collider.enabled = false;
        }
    }
}
