using System;
using Lean.Touch;
using UnityEngine;
using UnityEngine.AI;

public partial class HorseRaceThirdPersonBehaviour : MonoBehaviour, IHorseRaceInGameStatus
{
    [SerializeField] private GameObject[] playerRelatedGameObjects;
    [SerializeField] private GameObject[] notPlayerRelatedGameObjects;
    [SerializeField] private Collider playerRelatedGameComponents;
    [SerializeField] private NavMeshAgent notPlayerRelatedGameObjectsComponents;
    [SerializeField] private UIComponentEnumInt laneContainer;
    [SerializeField] private WorldSpaceCanvasBillBoard worldSpaceCanvasBillBoard;
    [SerializeField] private Transform horseTransform;
    [SerializeField] private float offsetRange = 5.5f;
    [SerializeField] private GameObject cameraFront;
    [SerializeField] private GameObject cameraBack;
    private GameObject sprintEffect;
    public event Action OnFinishRace = ActionUtility.EmptyAction.Instance;

    private HorseRaceThirdPersonData horseRaceThirdPersonData;
    private int lap = 0;
    private float currentRaceProgressWeight;
    private bool isStart;
    private float currentTargetForwardSpeed;
    private float currentSprintTime;
    private int currentSprintNumber;
    private float currentTimeToNextSprint;
    private float delayTime = 1.0f;
    public float CurrentForwardSpeed { get; private set; }
    public bool IsAbleToSprint => currentTimeToNextSprint <= 0.0f && currentSprintNumber < horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintNumber;
    
    public float CurrentSprintNormalizeTime => currentSprintTime / horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintTime;
    public float CurrentSprintHealingNormalizeTime => Mathf.InverseLerp(1.0f, 0.0f ,
        currentTimeToNextSprint / horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintHealingTime) ;
    public bool IsPlayer => horseRaceThirdPersonData.IsPlayer;
    
    public float CurrentRaceProgressWeight => currentRaceProgressWeight + lap;
    public int InitialLane => horseRaceThirdPersonData.InitialLane;
    public string Name { get; set; }
    
    public int HorizontalDirection { get;  set; }
    public bool IsStart => isStart;

    public void StartRace(float normalizeSpeed)
    {
        isStart = true;
        currentTargetForwardSpeed = Mathf.Lerp(horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.x, horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.y, normalizeSpeed);
        currentSprintTime = horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintTime;
        currentTimeToNextSprint = horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintHealingTime;
    }

    public HorseRaceThirdPersonData HorseRaceThirdPersonData
    {
        get => horseRaceThirdPersonData;
        set
        {
            horseRaceThirdPersonData = value;
            OnSetData();
        }
    }

    public float OffsetRange => offsetRange;

    private void OnSetData()
    {
        playerRelatedGameObjects.ForEach(x => x.SetActive(horseRaceThirdPersonData.IsPlayer));
        notPlayerRelatedGameObjects.ForEach(x => x.SetActive(horseRaceThirdPersonData.IsPlayer == false));
        playerRelatedGameComponents.enabled = horseRaceThirdPersonData.IsPlayer;
        notPlayerRelatedGameObjectsComponents.enabled = !horseRaceThirdPersonData.IsPlayer;
        horseTransform.position = TargetGenerator.FromTimeToPoint(
            horseRaceThirdPersonData.TargetGenerator.PredefinePath.StartTime,
            horseRaceThirdPersonData.TargetGenerator.GetOffsetFromLane(horseRaceThirdPersonData.InitialLane),
            horseRaceThirdPersonData.PredefinePath);

        horseTransform.rotation = horseRaceThirdPersonData.TargetGenerator.PredefinePath.StartRotation;
        laneContainer.SetEntity(horseRaceThirdPersonData.InitialLane);
        worldSpaceCanvasBillBoard.CameraTransform = horseRaceThirdPersonData.Camera;
        sprintEffect = GetComponentInChildren<HorseObjectReferences>().raceModeSprintParticle;
    }

    private float GetCurrentProgress()
    {
        return Mathf.InverseLerp( horseRaceThirdPersonData.PredefinePath.StartTime, 
            horseRaceThirdPersonData.PredefinePath.EndTime,
            horseRaceThirdPersonData.PredefinePath.GetClosestTime(horseTransform.position));
    }
    
    private void Update()
    {
        if (!IsStart) return;
        UpdateSpeed();
        UpdateSprintTime();
        UpdateSprintHealingTime();
        UpdateCurrentProgress();
    }

    private void UpdateCurrentProgress()
    {
        delayTime -= Time.deltaTime;
        if (delayTime >= 0.0f) return;
        
        currentRaceProgressWeight = GetCurrentProgress();
        if (currentRaceProgressWeight >= 1.0f && lap == 0)
        {
            lap++;
            OnFinishRace.Invoke();
        }
    }

    private void UpdateSprintHealingTime()
    {
        if (!(currentTimeToNextSprint > 0.0f)) return;
        currentTimeToNextSprint = Mathf.MoveTowards(currentTimeToNextSprint, 0.0f, Time.deltaTime);
    }

    private void UpdateSprintTime()
    {
        sprintEffect.SetActive(currentSprintTime > 0);
        if (!(currentSprintTime > 0.0f)) return;
        currentSprintTime = Mathf.MoveTowards(currentSprintTime, 0.0f, Time.deltaTime);
        if (currentSprintTime <= 0.0f)
        {
            currentTargetForwardSpeed = horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.x;
        }
    }

    private void UpdateSpeed()
    {
        CurrentForwardSpeed = Mathf.MoveTowards(
        CurrentForwardSpeed,
        currentTargetForwardSpeed,
        horseRaceThirdPersonData.HorseRaceThirdPersonStats.Acceleration * Time.deltaTime);
    }

    public void Sprint()
    {
        if (!IsAbleToSprint) return;
        currentSprintNumber++;
        currentTimeToNextSprint = horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintHealingTime;
        currentSprintTime = horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintTime;
        currentTargetForwardSpeed = horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.y;
    }

    public void EnableCamera(bool isBackCamera)
    {
        cameraFront.SetActive(!isBackCamera);
        cameraBack.SetActive(isBackCamera);
    }
}