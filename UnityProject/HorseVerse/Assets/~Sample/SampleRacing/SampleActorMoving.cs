using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleActorMoving : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] Animator anim;
    [SerializeField] ControlPointsAuthoring spline;
    [Space, Header("Setting")]
    [SerializeField] float BMS;
    [SerializeField] float BA;
    [SerializeField] float MMS;
    [SerializeField] float MA;
    [SerializeField] float MDuration = 10.0f;

    [SerializeField] float clampDistance = 10.0f;
    [SerializeField] float verticalDistance = 5.0f;

    [SerializeField] Transform fakePoint;

    float currentMS;
    float targetMS;
    float currentA;
    Vector3[] controlPoints;

    RacingRank rank;
    RacingLapProgress lapProgress;

    bool isMoving = false;
    bool isSprint = false;
    int counter = 0;
    float duration = 0;
    private void Start()
    {
        controlPoints = spline.getControlPoint();
    }

    private void Update()
    {
        updateAcceclation();
        if (isMoving)
        {
            if (isSprint)
                Sprinting();
            else
                Moving();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            StartMoving();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSprint();
        }
    }

    void StartMoving()
    {
        isMoving = !isMoving;
        if(isMoving == true)
        {
            targetMS = BMS;
        }
    }

    void Moving()
    {
        if (counter == 0)
        {
            calculateMove();
            counter = 5;
        }
        else counter--;
        var dir = getDirection();
        currentMS = getVelocity(Time.deltaTime);
        rigid.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        rigid.velocity = currentMS * rigid.transform.forward;
    }

    void StartSprint()
    {
        duration = 0;
        isSprint = !isSprint;
        if (isSprint == true)
        {
            targetMS = MMS;
        }
    }

    void StopSprint()
    {
        isSprint = false;
        if (isMoving == true)
        {
            targetMS = BMS;
        }
        else
        {
            targetMS = 0;
        }
    }

    void Sprinting()
    {
        if (counter == 0)
        {
            calculateMove();
            counter = 3;
        }
        else 
            counter--;

        var dir = getDirection();
        currentMS = getVelocity(Time.deltaTime);
        rigid.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        rigid.velocity = currentMS * rigid.transform.forward;

        if (duration < MDuration)
        {
            duration += Time.deltaTime;
        }
        else
            StopSprint();
    }

    void calculateMove()
    {
        var carPosition = rigid.transform.position;

        // Find where the car is closest to the track
        ComputeClosest(controlPoints, carPosition, out var closestDistance, out var closestSegmentIndex,
            out var closestPointOnSegment);
        
        var currentPoint = controlPoints[closestSegmentIndex];
        var nextPoint = controlPoints[(closestSegmentIndex + 1) % controlPoints.Length];
        var currentSegmentProgress = Vector3.Distance(closestPointOnSegment, currentPoint) /
                                     Vector3.Distance(currentPoint, nextPoint);
        var controlPointProgress = closestSegmentIndex + currentSegmentProgress;

        // is this the beginning of the race and we just crossed the line?
        if (lapProgress.CurrentControlPoint == -1 && controlPointProgress < 0.0f)
        {
            return;
        }

        // did we just complete a lap?
        if (controlPointProgress < 1f && lapProgress.CurrentControlPoint >= controlPoints.Length - 1)
        {
            lapProgress.CurrentLap++;
            rank.LastLapTime = Time.time; //(float) (time.ElapsedTime - race.RaceStartTime);
        }

        lapProgress.CurrentControlPoint = closestSegmentIndex;
        lapProgress.CurrentControlPointProgress = currentSegmentProgress;
        lapProgress.TotalProgress = lapProgress.CurrentLap * 1000f + controlPointProgress;
    }

    Vector3 getDirection()
    {
        var controlPointIndex = lapProgress.CurrentControlPoint;
        var controlPointProgress = lapProgress.CurrentControlPointProgress;
        var firstControlPointIndex = controlPointIndex == 0 ? controlPoints.Length - 1 : controlPointIndex - 1;
        var closestPoint = SplineUtils.GetPoint(controlPoints, firstControlPointIndex, controlPointProgress);
        var tangent = SplineUtils.GetTangent(controlPoints, firstControlPointIndex, controlPointProgress);
        var targetPoint = closestPoint + tangent * clampDistance + Vector3.Cross(tangent, Vector3.up) * verticalDistance;

        var current = new Vector3(rigid.transform.position.x, 0, rigid.transform.position.z);
        var target = new Vector3(targetPoint.x, 0, targetPoint.z); 
        var wantedDirection = (target - current).normalized;
        var dis = (target - current).sqrMagnitude;
        var currentDirection = new Vector3(rigid.transform.forward.x, 0, rigid.transform.forward.z);


        //var angleCurrentDirection = Mathf.Atan2(currentDirection.y, currentDirection.x);
        //angleCurrentDirection = Mathf.Rad2Deg * (angleCurrentDirection);
        //var angleWantedDirection = Mathf.Atan2(wantedDirection.y, wantedDirection.x);
        //angleWantedDirection = Mathf.Rad2Deg * (angleWantedDirection);
        //var angleDiff = angleWantedDirection - angleCurrentDirection;
        if (fakePoint != default) fakePoint.transform.position = targetPoint;
        return dis < 1.0f ? tangent.normalized : wantedDirection;
    }

    void updateAcceclation()
    {
        if (currentMS < targetMS)
        {
            currentA = isSprint ? MA : BA;
        }
        else
        {
            currentA = -MA;
        }
    }

    float getVelocity(float deltaTime)
    {
        var v = currentMS + currentA * deltaTime;
        return v > targetMS ? targetMS : v;
    }


    private static void ComputeClosest(Vector3[] controlPoints,
            Vector3 carPosition,
            out float closestDistance, out int closestSegmentIndex, out Vector3 closestPointOnSegment)
    {
        closestDistance = float.MaxValue;
        closestSegmentIndex = -1;
        closestPointOnSegment = Vector3.zero;

        for (var i = 0; i < controlPoints.Length; i++)
        {
            var current = controlPoints[i];
            var next = controlPoints[(i + 1) % controlPoints.Length];
            var pointOnSegment = GetClosestPointOnSegment(carPosition, current, next);
            var distanceToSegment = Vector3.Distance(carPosition, pointOnSegment);

            if (distanceToSegment < closestDistance)
            {
                closestSegmentIndex = i;
                closestDistance = distanceToSegment;
                closestPointOnSegment = pointOnSegment;
            }
        }
    }

    public static Vector3 GetClosestPointOnSegment(Vector3 subject, Vector3 pA, Vector3 pB)
    {
        var AP = subject - pA;
        var AB = pB - pA;

        var magnitudeAB = distancesq(pA, pB);
        var ABAPproduct = Vector3.Dot(AP, AB);
        var distance = ABAPproduct / magnitudeAB;

        if (distance < 0)
        {
            return pA;
        }

        if (distance > 1)
        {
            return pB;
        }

        return pA + AB * distance;
    }

    private static float distancesq(Vector3 a, Vector3 b)
    {
        return Mathf.Pow((a.x - b.x), 2f) + Mathf.Pow((a.y - b.y), 2f) + Mathf.Pow((a.z - b.z), 2f); 
    }


}
