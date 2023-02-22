using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTrapBall : TrainingTrap
{
    [SerializeField] public float RollingSpeed;
    [SerializeField] public SphereCollider Collider;
    [SerializeField] public Rigidbody Rigid;
    [SerializeField] Transform DropPoint;
    [SerializeField] Transform MovingPoint;
    [SerializeField] Transform TriggerPoint;

    [SerializeField] public float ExposeSpeed;

    private Vector3 direction;

    private bool isStart;
    private bool isReachedTarget;

    public void OnEnable()
    {
        
    }

    public override void Active()
    {
        isStart = true;
        isReachedTarget = false;
        Collider.enabled = true;
        TriggerPoint.gameObject.SetActive(false);
        Rigid.useGravity = false;
        OnActiveToDropPoint();
    }

    private void Update()
    {
        if (!isStart) return;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (IsReachTarget() && !isReachedTarget)
        {
            isReachedTarget = true; 
            TriggerTarget();
        }

        if (IsLastWayPoint())
        {
            OnFinishEvent();
        }
    }

    protected virtual void OnActiveToDropPoint()
    {
        //direction = Collider.transform.localPosition - DropPoint.localPosition;
        //direction = direction.normalized;
        //Rigid.velocity = -direction * ExposeSpeed;
        Rigid.transform.localPosition = DropPoint.localPosition;
    }

    protected virtual bool IsReachTarget()
    {
        if (Vector3.Distance(Collider.transform.localPosition, DropPoint.localPosition) <= Collider.radius + 1.0f)
            return true;
        return false;
    }

    private bool IsLastWayPoint()
    {
        return false;
    }

    private void TriggerTarget()
    {
        direction = MovingPoint.localPosition - DropPoint.localPosition;
        direction = direction.normalized; Debug.Log("Trigger Target " + direction);
        Rigid.useGravity = true;
        Rigid.velocity = Vector3.zero;
        Rigid.WakeUp();
        Rigid.AddForce(direction * RollingSpeed , ForceMode.Impulse);
        
    }

    private void OnFinishEvent()
    {
        isStart = false;
    }

}
