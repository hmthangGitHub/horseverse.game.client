using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTrapBall : TrainingTrap
{
    [SerializeField] public float RollingSpeed;
    [SerializeField] TrainingTrapBallObjectController ball;
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
        ball.Reset();
    }

    public override void Active()
    {
        isStart = true;
        isReachedTarget = false;
        Collider.enabled = true;
        TriggerPoint.gameObject.SetActive(false);
        Rigid.useGravity = false;
        ball.OnDeadEvent = OnDead;
        OnActiveToDropPoint();
    }

    private void Update()
    {
        //Test 
        if (Input.GetKeyDown(KeyCode.J)) Active();

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
        ball.IsReady = true;
        direction = MovingPoint.localPosition - DropPoint.localPosition;
        direction = direction.normalized; 
        Rigid.useGravity = true;
        Rigid.velocity = direction * RollingSpeed;
        Rigid.WakeUp();


    }

    private void OnFinishEvent()
    {
        isStart = false;
    }

    private void OnDead()
    {
        Destroy(this.gameObject);
    }

}
