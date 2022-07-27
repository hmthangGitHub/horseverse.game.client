using DG.Tweening;
using PathCreation;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HorseController : MonoBehaviour
{
    private HorseInGameData horseInGameData;
    public void SetHorseData(HorseInGameData horseInGameData)
    {
        this.horseInGameData = horseInGameData;

        playerIndicator.SetActive(horseInGameData.IsPlayer);
        CalculateRotation(0);
        CalculatePosition(0);
        OnFinishTrackEvent += horseInGameData.OnFinishTrack;
    }

    public bool IsPlayer => horseInGameData.IsPlayer;
    private float CurrentOffset => horseInGameData.CurrentOffset;
    public int TopInRaceMatch => horseInGameData.TopInRaceMatch;
    public float CurrentRaceProgressWeight => currentTargetIndex * 1000 - DistanceToCurrentTarget();
    public int InitialLane => horseInGameData.InitialLane;
    private PathCreator PathCreator => horseInGameData.PathCreator;
    private (Vector3 target, float time)[] PredefineTargets => horseInGameData?.PredefineTargets;

    private event Action OnFinishTrackEvent = ActionUtility.EmptyAction.Instance;

    private Transform _transform;
    private Transform Transform => _transform ?? this.transform;
    private int currentTargetIndex = -1;
    private bool isStart;
    private Animator animator;

    [SerializeField] private GameObject playerIndicator;
    [SerializeField] NavMeshAgent navMeshAgent;

#if UNITY_EDITOR
    private GameObject target;
    public GameObject Target => target ??= new GameObject($"{gameObject.name}_Target");
    Color? color;
    Color Color => color ??= UnityEngine.Random.ColorHSV();
#endif

    private void Start()
    {
        animator = GetComponentInChildren<Animator>(true);
        animator.SetFloat("Speed", 0.0f);
        animator.Play("Idle");
    }

    public void StartRace()
    {
        animator.Play("Movement", 0, UnityEngine.Random.insideUnitCircle.x);
        animator.SetFloat("Speed", 1.0f);
        isStart = true;
        ChangeTarget();
    }

    private void CalculatePosition(float time)
    {
        var pos = PathCreator.path.GetPointAtTime(time, EndOfPathInstruction.Loop);
        transform.position = Vector3.Scale(new Vector3(1, 0, 1), (pos + transform.right * CurrentOffset));
    }

    private void CalculateRotation(float time)
    {
        Quaternion rotationAtDistance = PathCreator.path.GetRotation(time, EndOfPathInstruction.Loop);
        transform.rotation = rotationAtDistance;
        transform.rotation = Quaternion.Euler(0, rotationAtDistance.eulerAngles.y, 0);
    }

    public void Skip()
    {
        //OnFinishTrack();//TODO
    }

    public void StartRun()
    {
        isStart = true;
        ChangeTarget();
    }

    private void Update()
    {
        if (isStart)
        {
            if (IsReachTarget())
            {
                if (IsLastWayPoint())
                {
                    OnFinishTrackEvent();
                    OnFinishTrackEvent -= horseInGameData.OnFinishTrack;
                }
                ChangeTarget();
            }

        }
    }

#if UNITY_EDITOR
    [SerializeField] private float totalTime = -1;
    private float TotalTime => totalTime < 0 ? totalTime = (PredefineTargets?.Sum(x => x.time) ?? 0) : totalTime;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && UnityEditor.Selection.gameObjects.Contains(this.gameObject))
        {
            Gizmos.color = Color;
            PredefineTargets?.ForEach(x => Gizmos.DrawSphere(x.target, 0.5f));
            UnityEditor.Handles.Label(this.transform.position, $"Total Time {TotalTime}");
            Debug.DrawLine(this.transform.position, Target.transform.position, Color);
        }
    }
#endif
    private bool IsReachTarget()
    {
        return DistanceToCurrentTarget() < 1f;
    }

    private float DistanceToCurrentTarget()
    {
        return (Transform.position - PredefineTargets[currentTargetIndex].target).XZ().magnitude;
    }

    private bool IsLastWayPoint()
    {
        return currentTargetIndex == PredefineTargets.Length - 1;
    }

    private void ChangeTarget()
    {
        currentTargetIndex++;
        currentTargetIndex %= PredefineTargets.Length;
        navMeshAgent.destination = PredefineTargets[currentTargetIndex].target;
        navMeshAgent.speed = (Transform.position - PredefineTargets[currentTargetIndex].target).magnitude / PredefineTargets[currentTargetIndex].time;
#if UNITY_EDITOR
        Target.transform.position = PredefineTargets[currentTargetIndex].target;
#endif
    }
}
