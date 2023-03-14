using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAloneLine : MonoBehaviour
{
    [SerializeField] BezierCurve curse;
    [SerializeField] float Speed;

    private bool isMovingBack = false;
    private bool isMoving = false;
    private float duration = 0;

    private float totalLen = 0;
    private float currentDistance = 0;
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
        if (duration >= 1  && !isMovingBack) return true;
        if (duration <= 0 && isMovingBack) return true;
        return false;
    }


    void StartMoving()
    {
        isMoving = !isMoving;
        duration = 0; Debug.Log("Moving " + isMoving + " -- " + totalLen);
        currentDistance = 0;
    }

    void Moving()
    {
        if(IsReachTarget())
        {
            isMovingBack = !isMovingBack;
        }
        else
        {
            this.transform.localPosition = getCurrentPosition(Time.fixedDeltaTime);
        }

    }

    Vector3 getCurrentPosition(float delta)
    {
        if(curse != default)
        {
            totalLen = curse.GetTotalLength();
            if (isMovingBack)
                duration -= delta;
            else
                duration += delta;
            if (duration > 1) duration = 1;
            if (duration < 0) duration = 0;
            float distance = Speed * delta;
            currentDistance += distance;
            Vector3 v = curse.DeCasteljausAlgorithmWithDistance(currentDistance, totalLen);
            return v;
        }
        return Vector3.zero;
    }
}
