using System;
using PathCreation;
using UnityEngine;

public class BridgeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrainingHorse"))
        {
            this.gameObject.SetActive(false);
        }
    }
}
