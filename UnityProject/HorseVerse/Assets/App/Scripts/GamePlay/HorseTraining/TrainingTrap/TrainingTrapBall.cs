using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TrainingTrapBall : TrainingTrap<TrainingTrapBall.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public Position Target;
        public Position Direction;
        public Position Trigger;
        public int TriggerSize;
        public bool TriggerZoneFullBlock;

        public static Entity Parse(string data)
        {
            return JsonConvert.DeserializeObject(data, typeof(Entity)) as Entity ?? null;
        }
    }

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

    protected override void OnSetEntity()
    {
        MovingPoint.localPosition = entity.Direction.ToVector3();
        TriggerPoint.localPosition = entity.Trigger.ToVector3();
        Debug.Log("SET ENTITY " + entity.Trigger.ToVector3());
    }

    public override Entity ParseData(string data)
    {
        return Entity.Parse(data);
    }

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
        Rigid.velocity = this.transform.TransformVector(direction * RollingSpeed);
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
