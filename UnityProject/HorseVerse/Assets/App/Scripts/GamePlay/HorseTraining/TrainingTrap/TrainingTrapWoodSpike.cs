using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public class TrainingTrapWoodSpike : TrainingTrap<TrainingTrapWoodSpike.Entity>
{
    [System.Serializable]
    public class Entity
    {
        public List<Position> Direction;
        public Position Trigger;
        public int TriggerSize;
        public bool TriggerZoneFullBlock;
        public bool IsPingPong;

        public static Entity Parse(string data)
        {
            return JsonConvert.DeserializeObject(data, typeof(Entity)) as Entity ?? null;
        }
    }

    [SerializeField] public float MovingSpeed;
    [SerializeField] public TrainingTrapWoodenSpikeController spike;
    [SerializeField] public CapsuleCollider Collider;
    [SerializeField] Transform TriggerPoint;
    [SerializeField] public List<Vector3> Directions;
    [SerializeField] public bool IsPingPong;

    private bool isStart;
    private bool isReachedTarget;
    private bool isReachedEnd;
    private int currentTarget = 0;
    private bool isTweenBack = false;
    public override Entity ParseData(string data)
    {
        return Entity.Parse(data);
    }

    protected override void OnSetEntity()
    {
        Directions.Clear();
        Directions.AddRange(entity.Direction.Select(x => x.ToVector3()).ToArray());
        TriggerPoint.localPosition = entity.Trigger.ToVector3();
    }

    public override void Active()
    {
        isStart = true;
        isReachedTarget = false;
        Collider.enabled = true;
        currentTarget = 0;
        isReachedEnd = false;
        isTweenBack = false;
        TriggerPoint.gameObject.SetActive(false);
        spike.IsReady = true;
        //OnActiveToDropPoint();
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
        if (IsReachTarget())
        {
            if (!isReachedTarget)
            {
                isReachedTarget = true;
                TriggerTarget();
            }
        }

        if (IsLastWayPoint())
        {
            OnFinishEvent();
        }
    }

    protected virtual bool IsReachTarget()
    {
        if (Directions.Count == 0) return true;
        var pos = Directions[currentTarget];
        if (Vector3.Distance(spike.transform.localPosition, pos) < 1)
        {
            return true;
        }
        //var dir = pos - rigid.transform.localPosition;
        //rigid.velocity = dir.normalized * MovingSpeed;
        var dir = pos - spike.transform.localPosition;
        spike.transform.localPosition += dir.normalized * MovingSpeed * Time.deltaTime;
        return false;
    }

    private void TriggerTarget()
    {
        if (Directions.Count < 2)
        {
            isReachedEnd = true;
            return;
        }

        if (IsPingPong)
        {
            if(isTweenBack)
            {
                if (currentTarget > 0)
                {
                    currentTarget--;
                }
                else
                {
                    isTweenBack = false;
                    currentTarget++;
                }
            }
            else
            {
                if (currentTarget < Directions.Count - 1)
                {
                    currentTarget++;
                }
                else
                {
                    isTweenBack = true;
                    currentTarget--;
                }
            }
            isReachedTarget = false;
        }
        else
        {
            if (currentTarget < Directions.Count - 1)
            {
                currentTarget++;
                isReachedTarget = false;
            }
            else isReachedEnd = true;
        }
    }

    private bool IsLastWayPoint()
    {
        return false;
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
