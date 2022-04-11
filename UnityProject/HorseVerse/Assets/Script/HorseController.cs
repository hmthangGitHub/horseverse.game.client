using DG.Tweening;
using PathCreation;
using UnityEngine;

// Moves along a path at constant speed.
// Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
public class HorseController : MonoBehaviour
{
    public PathCreator pathCreator;
    public float averageSpeed = 24;
    public float offsetPerLane = -1;
    public float targetlane = 0;
    public float laneNumber = 10;
    public float currentOffset = 0;
    public float changeLaneTime = 5.0f;
    public float currentChangeLaneTime = 0.0f;

    public float rayCastDistance = 2.0f;
    public float sideRayCastDistance = 1.0f;
    public Vector3 offset = new Vector3(0.0f, 1.264721f, 0.0f);
    public float timeToFinish = 50.0f;
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

    public bool IsPlayer { get => isPlayer; set { 
            isPlayer = value;
            playerIndicator.SetActive(isPlayer);
        } }

    private void Start()
    {
        currentCurve = speedCurve[UnityEngine.Random.Range(0, speedCurve.Length - 1)];
        this.GetComponentInChildren<Animator>().Play("Movement", 0, UnityEngine.Random.insideUnitCircle.x);
    }

    void Update()
    {
        if (pathCreator != null)
        {
            currentRaceTime += Time.deltaTime;
            if (currentRaceTime > timeToFinish && timeOffset == 0)
            {
                timeOffset = averageTimeToFinish - timeToFinish;
                timeToFinish = averageTimeToFinish;
                currentCurve = defaultCurve;
            }
            var linearT = ((currentRaceTime + timeOffset)/ (timeToFinish));
            var t = currentCurve.Evaluate(linearT % 1);
            
            var pos = pathCreator.path.GetPointAtTime(t * lap, EndOfPathInstruction.Loop);
            transform.position = Vector3.Scale(new Vector3(1, 0, 1), (pos + transform.right * currentOffset));
            Quaternion rotationAtDistance = pathCreator.path.GetRotation(t * lap, EndOfPathInstruction.Loop);
            transform.rotation = rotationAtDistance;
            transform.rotation = Quaternion.Euler(0, rotationAtDistance.eulerAngles.y, 0);
        }
        currentChangeLaneTime += Time.deltaTime;
#if UNITY_EDITOR
        DrawRay();
#endif
    }

#if UNITY_EDITOR
    private void DrawRay()
    {
        Debug.DrawLine(this.transform.position + offset, this.transform.position + this.transform.forward * this.rayCastDistance);
    }
#endif
}
