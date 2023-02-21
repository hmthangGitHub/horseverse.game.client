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

    public void Active()
    {
        isStart = true;
        isReachedTarget = false;
        TriggerPoint.gameObject.SetActive(false);
        Rigid.useGravity = false;
        OnActiveToDropPoint();
    }

    private void Update()
    {
        //Test
        if (Input.GetKeyDown(KeyCode.Space)) Active();
        //

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
        direction = Collider.transform.position - DropPoint.position;
        direction = direction.normalized;
        Rigid.velocity = -direction * ExposeSpeed;
    }

    protected virtual bool IsReachTarget()
    {
        if (Vector3.Distance(Collider.transform.position, DropPoint.position) <= Collider.radius + 1.0f)
            return true;
        return false;
    }

    private bool IsLastWayPoint()
    {
        return false;
    }

    private void TriggerTarget()
    {
        direction = MovingPoint.position - DropPoint.position;
        direction = direction.normalized;
        Rigid.useGravity = true;
        Rigid.velocity = Vector3.zero;
        Rigid.AddForce(direction * RollingSpeed, ForceMode.Impulse);
    }

    private void OnFinishEvent()
    {
        isStart = false;
    }

}
