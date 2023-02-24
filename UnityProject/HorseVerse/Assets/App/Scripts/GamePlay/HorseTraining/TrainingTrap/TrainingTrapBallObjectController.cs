using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTrapBallObjectController : MonoBehaviour
{
    private const string Obstacle = "Obstacle";
    private bool isDead = false;
    private bool isReady = false;
    public bool IsDead => isDead;
    public bool IsReady { get => isReady; set { isReady = value; } }
    public System.Action OnDeadEvent { get; set; }

    public void Reset()
    {
        isDead = false;
        isReady = false;
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isReady) return;
        if (other.gameObject.CompareTag(Obstacle) && !isDead)
        {
            OnDead();
        }
    }

    private void OnDead()
    {
        isDead = true;
        OnDeadEvent?.Invoke();
    }
}
