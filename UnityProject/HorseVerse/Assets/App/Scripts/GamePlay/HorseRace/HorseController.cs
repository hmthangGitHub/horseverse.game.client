using DG.Tweening;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public partial class HorseController : MonoBehaviour, IHorseRaceInGameStatus
{
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] UIComponentEnumInt laneContainer;
    [SerializeField] private float angleMax;
    [SerializeField] private float totalTime = -1;
    
    private HorseInGameData horseInGameData;
    private event Action OnFinishTrackEvent = ActionUtility.EmptyAction.Instance;
    private Vector2 timeRange;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private float animationSpeed; 
    private float animationHorizontal;
    private float targetAnimationHorizontal;
    private float targetAnimationSpeed; 
    private Transform _transform;
    private Transform Transform => _transform ??= this.transform;
    private int currentTargetIndex = -1;
    private int progressiveTargetIndex = -1;
    private bool isStarted;
    private float runTime;
    private Animator animator;
    
    public bool IsPlayer => horseInGameData.IsPlayer;
    public float CurrentRaceProgressWeight => isStarted ? IsFinishRace ? 1000000.0f - totalTime : (progressiveTargetIndex + 1)* 1000 - DistanceToCurrentTarget() : default;
    public int InitialLane => horseInGameData.InitialLane;
    private float CurrentOffset => horseInGameData.CurrentOffset;
    private (Vector3 target, Quaternion rotation, float time)[] PredefineTargets => horseInGameData?.PredefineTargets.targets;
    private int FinishIndex => horseInGameData.PredefineTargets.finishIndex;
    private bool IsFinishRace => progressiveTargetIndex >= FinishIndex; 
    public string Name => horseInGameData.Name;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>(true);
        animator.SetFloat(Speed, 0.0f);
    }

    public async UniTask StartRaceAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(this.horseInGameData.Delay), cancellationToken: this.GetCancellationTokenOnDestroy());
        isStarted = true;
        laneContainer.gameObject.SetActive(true);
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
        isStarted = true;
        ChangeTarget();
    }

    private void Update()
    {
        if (isStarted)
        {
            UpdatePosition();
            UpdateAnimator();
            UpdateLaneIndicator();
        }
    }

    private void UpdatePosition()
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

    private void UpdateLaneIndicator()
    {
        laneContainer.transform.rotation = Quaternion.LookRotation(laneContainer.transform.position -
                                                                   this.horseInGameData.MainCamera.transform.position);
    }

    private void UpdateAnimator()
    {
        animationSpeed = Mathf.Lerp(animationSpeed, targetAnimationSpeed, Time.deltaTime * 10.0f);
        animator.SetFloat(Speed, animationSpeed);

        var delta = PredefineTargets[currentTargetIndex]
            .target - transform.position;
        var angle = Vector3.SignedAngle(delta, transform.forward, Vector3.up);
        if (Math.Abs(angle) > 2.5f)
        {
            targetAnimationHorizontal
                = Vector3Extensions.Map(Mathf.Clamp(-angle, -angleMax, angleMax), -angleMax, angleMax, -1.0f, 1.0f);
        }

        animationHorizontal = Mathf.Lerp(animationHorizontal, targetAnimationHorizontal, Time.deltaTime * 5.0f);
        animator.SetFloat(Horizontal, animationHorizontal);
    }

    private void FixedUpdate()
    {
        if (!isStarted) return;
        if (progressiveTargetIndex > FinishIndex) return;
        
        runTime += Time.deltaTime;
        var totalTimeToCurrent = PredefineTargets.Take(currentTargetIndex + 1)
                                                 .Sum(x => x.time);
        var timeLeft = totalTimeToCurrent - runTime;
        if (timeLeft > 0.0f)
        {
            var speed = DistanceToCurrentTarget() / timeLeft;
            navMeshAgent.speed = Mathf.Min(speed, 25.0f);
        }
    }

    private bool IsReachTarget()
    {
        return Vector3.Dot((PredefineTargets[currentTargetIndex].target - Transform.position), PredefineTargets[currentTargetIndex].rotation * Vector3.forward) <= 0.0f
               || DistanceToCurrentTarget() <= 1.0f;
    }

    private float DistanceToCurrentTarget()
    {
        return (Transform.position - PredefineTargets[currentTargetIndex].target).XZ().magnitude;
    }

    private bool IsLastWayPoint()
    {
        return progressiveTargetIndex == FinishIndex;
    }

    private void ChangeTarget()
    {
        progressiveTargetIndex++;
        currentTargetIndex = progressiveTargetIndex % PredefineTargets.Length;
        navMeshAgent.destination = PredefineTargets[currentTargetIndex].target;
        navMeshAgent.speed = (Transform.position - PredefineTargets[currentTargetIndex].target).magnitude / PredefineTargets[currentTargetIndex].time;
        
        SetAnimation(PredefineTargets[currentTargetIndex].time);
#if UNITY_EDITOR
        Target.transform.position = PredefineTargets[currentTargetIndex].target;
        Target.transform.rotation = PredefineTargets[currentTargetIndex].rotation;
#endif
    }

    private void SetAnimation(float time)
    {
        targetAnimationSpeed = Vector3Extensions.Map(time, timeRange.x, timeRange.y, 1.1f, 0.8f);
    }
    
    public void SetHorseData(HorseInGameData horseInGameData)
    {
        this.horseInGameData = horseInGameData;
        CalculateRotation();
        CalculatePosition();
        OnFinishTrackEvent += horseInGameData.OnFinishTrack;
        laneContainer.SetEntity(new UIComponentEnumInt.Entity()
        {
            number = this.horseInGameData.InitialLane
        });
        laneContainer.gameObject.SetActive(false);
        laneContainer.GetComponent<Canvas>().worldCamera = this.horseInGameData.MainCamera.GetComponent<Camera>();
        totalTime = PredefineTargets.Sum(x => x.time) + horseInGameData.Delay;
    }
}
