using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAloneLine : MonoBehaviour
{
    [SerializeField] BezierCurve curse;
    [SerializeField] float Speed;
    [SerializeField] GameObject sphere;

    private bool isMovingBack = false;
    private bool isMoving = false;
    private float duration = 0;

    private float totalLen = 0;
    private float currentDistance = 0;

    private Vector3 tangenPoint;
    private bool isEnd;
    // Start is called before the first frame update
    void Start()
    {
        if (curse != default)
        {
            curse.init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartMoving();
        }

        if (isMoving) Moving();
    }

    bool IsReachTarget()
    {
        if (totalLen > 0)
        {
            if (currentDistance == totalLen && !isMovingBack) return true;
            if (currentDistance == 0 && isMovingBack) return true;
        }
        return false;
    }

    void StartMoving()
    {
        isMoving = !isMoving;
        duration = 0;
        currentDistance = 0;
        Vector3 dir;
        bool isEnd;
        tangenPoint = curse.findTheClosedPoint(this.transform.position, out dir, out isEnd);
        sphere.transform.position = tangenPoint;
    }

    void Moving()
    {
        if(IsReachTarget())
        {
            isMovingBack = !isMovingBack;
        }
        else
        {
            //this.transform.localPosition = getCurrentPosition(Time.fixedDeltaTime);
            if (!isEnd)
            {
                Vector3 dir;
                var point = curse.transform.InverseTransformPoint(this.transform.position);
                tangenPoint = curse.findTheClosedPoint(point, out dir, out isEnd);
                //Vector3 direction = curse.GetTangent(currentDistance, totalLen);
                sphere.transform.position = tangenPoint;
                var _dir = curse.transform.TransformVector(dir);
                this.transform.localPosition += _dir * Speed * Time.deltaTime;
            }
            else
                this.transform.localPosition += this.transform.forward * Speed * Time.deltaTime;
        }
    }

    Vector3 getCurrentPosition(float delta)
    {
        if(curse != default)
        {
            if(totalLen == 0)
                totalLen = curse.GetTotalLength();
            float distance = Speed * delta;
            if (isMovingBack)
                currentDistance -= distance;
            else
                currentDistance += distance;
            if (currentDistance > totalLen) currentDistance = totalLen;
            if (currentDistance < 0) currentDistance = 0;
            Vector3 v = curse.DeCasteljausAlgorithmWithDistance(currentDistance, totalLen);
            Vector3 direction = curse.GetTangent(currentDistance, totalLen);
            this.transform.forward = direction;
            return v;
        }
        return Vector3.zero;
    }

}
