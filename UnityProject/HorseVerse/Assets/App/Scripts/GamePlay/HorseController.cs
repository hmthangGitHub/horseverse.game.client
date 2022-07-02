using DG.Tweening;
using PathCreation;
using System;
using UnityEngine;

// Moves along a path at constant speed.
// Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
public class HorseController : MonoBehaviour
{
    public PathCreator pathCreator;
    public float averageSpeed = 24;
    public float currentOffset = 0;

    public float rayCastDistance = 2.0f;
    public float sideRayCastDistance = 1.0f;
    public Vector3 offset = new Vector3(0.0f, 1.264721f, 0.0f);
    public float timeToFinish = 50.0f;
    public float currentTimeToFinish = 50.0f;
    public float lap = 1.0f;

    public int top;
    public float currentRaceTime = 0.0f;
    public AnimationCurve[] speedCurve;
    public AnimationCurve currentCurve;
    public AnimationCurve defaultCurve;
    public float averageTimeToFinish;
    public float timeOffset = 0.0f;

    public GameObject playerIndicator;

    private bool isPlayer = false;
    public float normalizePath;
    public event Action OnFinishTrackEvent = ActionUtility.EmptyAction.Instance;
    public bool IsPlayer { get => isPlayer; set { 
            isPlayer = value;
            playerIndicator.SetActive(isPlayer);
        } }

    public int Lane;

    private bool isStartRace = false;

    public void StartRace()
    {
        currentCurve = speedCurve[UnityEngine.Random.Range(0, speedCurve.Length - 1)];
        this.GetComponentInChildren<Animator>().Play("Movement", 0, UnityEngine.Random.insideUnitCircle.x);
        isStartRace = true;
    }

    void Update()
    {
        if (pathCreator != null && isStartRace)
        {
            currentRaceTime += Time.deltaTime;
            if (currentRaceTime > currentTimeToFinish && timeOffset == 0)
            {
                OnFinishTrack();
            }
            var linearT = ((currentRaceTime + timeOffset)/ (currentTimeToFinish));
            normalizePath = currentCurve.Evaluate(linearT % 1);
            
            var pos = pathCreator.path.GetPointAtTime(normalizePath * lap, EndOfPathInstruction.Loop);
            transform.position = Vector3.Scale(new Vector3(1, 0, 1), (pos + transform.right * currentOffset));
            Quaternion rotationAtDistance = pathCreator.path.GetRotation(normalizePath * lap, EndOfPathInstruction.Loop);
            transform.rotation = rotationAtDistance;
            transform.rotation = Quaternion.Euler(0, rotationAtDistance.eulerAngles.y, 0);
        }
#if UNITY_EDITOR
        DrawRay();
#endif
    }

    private void OnFinishTrack()
    {
        timeOffset = averageTimeToFinish - timeToFinish;
        currentTimeToFinish = averageTimeToFinish;
        currentCurve = defaultCurve;
        OnFinishTrackEvent.Invoke();
    }

    public void Skip()
    {
        OnFinishTrack();
        currentRaceTime = averageTimeToFinish + timeOffset;
    }

#if UNITY_EDITOR
    private void DrawRay()
    {
        Debug.DrawLine(this.transform.position + offset, this.transform.position + this.transform.forward * this.rayCastDistance);
    }
#endif
}
