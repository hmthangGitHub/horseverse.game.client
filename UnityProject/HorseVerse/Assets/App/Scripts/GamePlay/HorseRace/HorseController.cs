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
    private Transform Transform => _transform ??= this.transform;
    private int currentTargetIndex = -1;
    private bool isStart;
    private Animator animator;

    [SerializeField] private GameObject playerIndicator;
    [SerializeField] NavMeshAgent navMeshAgent;
    private Vector2 timeRange;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private float animationSpeed; 
    private float animationHorizontal; 
    private float targetHorizontalSpeed; 
    private float targetAnimationSpeed; 

#if UNITY_EDITOR
    private GameObject target;
    public GameObject Target => target ??= new GameObject($"{gameObject.name}_Target");
    Color? color;
    Color Color => color ??= UnityEngine.Random.ColorHSV();
#endif

    private void Start()
    {
        animator = GetComponentInChildren<Animator>(true);
        animator.SetFloat(Speed, 0.0f);
    }

    public void StartRace()
    {
        isStart = true;
        timeRange = new Vector2(this.horseInGameData.PredefineTargets.targets.Min(x => x.time),
            this.horseInGameData.PredefineTargets.targets.Max(x => x.time));
        ChangeTarget();
        DOTween.To(val =>
        {
            animator.SetFloat(Speed, val);
        }, 0.0f, 1.0f, 1.0f);
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

            animationSpeed = Mathf.Lerp(animationSpeed, targetAnimationSpeed, Time.deltaTime * 10.0f);
            animationHorizontal = Vector3Extensions.Map( Mathf.Clamp(navMeshAgent.velocity.x, -12.0f, 12.0f), -12.0f, 12.0f, -1.0f, 1.0f);
            animator.SetFloat(Speed, animationSpeed);
            animator.SetFloat(Horizontal, animationHorizontal);
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

        
        SetAnimation(PredefineTargets[currentTargetIndex].time);
        SetHorizontalSpeed(PredefineTargets[currentTargetIndex].target);
#if UNITY_EDITOR
        Target.transform.position = PredefineTargets[currentTargetIndex].target;
#endif
    }

    private void SetHorizontalSpeed(Vector3 target)
    {
        var delta = target - this.transform.position;
        var angle = Vector3.SignedAngle(Transform.forward, delta, Vector3.up);
        targetHorizontalSpeed = Vector3Extensions.Map( Mathf.Clamp(navMeshAgent.velocity.x, -30.0f, 30.0f), -30.0f, 30, -1.0f, 1.0f);
    }

    private void SetAnimation(float time)
    {
        targetAnimationSpeed = Vector3Extensions.Map(time, timeRange.x - 0.000001f, timeRange.y + 0.00001f, 1.1f, 0.8f);
    }
}
