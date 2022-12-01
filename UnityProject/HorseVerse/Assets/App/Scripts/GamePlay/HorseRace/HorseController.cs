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
        CalculateRotation();
        CalculatePosition();
        OnFinishTrackEvent += horseInGameData.OnFinishTrack;
    }

    public bool IsPlayer => horseInGameData.IsPlayer;
    private float CurrentOffset => horseInGameData.CurrentOffset;
    public int TopInRaceMatch => horseInGameData.TopInRaceMatch;
    public float CurrentRaceProgressWeight => currentTargetIndex * 1000 - DistanceToCurrentTarget();
    public int InitialLane => horseInGameData.InitialLane;
    private PathCreator PathCreator => horseInGameData.TargetGenerator.SimplyPath;
    private (Vector3 target, float time)[] PredefineTargets => horseInGameData?.PredefineTargets.targets;
    private int FinishIndex => horseInGameData.PredefineTargets.finishIndex;

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

    private void CalculatePosition()
    {
        var pos = this.horseInGameData.TargetGenerator.StartPosition;
        transform.position = Vector3.Scale(new Vector3(1, 0, 1), (pos + transform.right * CurrentOffset));
    }

    private void CalculateRotation()
    {
        Quaternion rotationAtDistance = horseInGameData.TargetGenerator.StartRotation;
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
                    Debug.Log($"Finish Track : {Transform.name}");
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
            
            if (PredefineTargets != default)
            {
                for (int i = 0; i < PredefineTargets.Length; i++)
                {
                    Gizmos.color = i <= FinishIndex ? Color : Color.red;
                    var x = PredefineTargets[i];
                    Gizmos.DrawSphere(x.target, 0.5f);
                }    
            }
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
        return currentTargetIndex == FinishIndex;
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
