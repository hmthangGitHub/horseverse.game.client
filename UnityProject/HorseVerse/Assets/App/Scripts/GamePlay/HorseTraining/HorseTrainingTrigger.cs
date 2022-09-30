using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseTrainingTrigger : MonoBehaviour
{
    private const string Obstacle = "Obstacle";
    private const string Coin = "Coin";
    
    public event Action OnTakeCoin = ActionUtility.EmptyAction.Instance;
    public event Action OnTouchObstacle = ActionUtility.EmptyAction.Instance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Obstacle))
        {
            OnTouchObstacle.Invoke();
        }

        if (other.CompareTag(Coin))
        {
            OnTakeCoin.Invoke();
        }
    }
}
