using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public Collider collider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrainingHorse"))
        {
            var platform = GetComponentInParent<PlatformBase>();
            if (platform.IsReady)
            {
                platform.OnFinishPlatform.Invoke();
                collider.enabled = false;
            }
        }
    }
}
