using System;
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
    public int CurrentLap { get; private set; } = 1;
    private float currentRaceProgressWeight;
    private float previousRaceProgressWeight;
    private bool isStart;
    private float currentTargetForwardSpeed;
    private int currentSprintContinuousTime;
    private float currentSprintTime;
    private float currentSprintChargeNumber;
    private float delayTime = 1.0f;
    private float currentSprintBonusTime = 0.0f;
    public float CurrentForwardSpeed { get; private set; }
    public bool IsAbleToSprint => CurrentSprintChargeNumber >= 1 && (currentSprintBonusTime > 0 || currentSprintTime <= 0.0f);
    
    public float CurrentSprintNormalizeTime => currentSprintTime / horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintTime;

    public float CurrentChargeNormalize => CurrentSprintChargeNumber /
                                           horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintChargeNumber;
    public bool IsPlayer => horseRaceThirdPersonData.IsPlayer;
    
    public float CurrentRaceProgressWeight => currentRaceProgressWeight + CurrentLap;
    public int InitialLane => horseRaceThirdPersonData.InitialLane;
    public float CurrentAcceleration { get; private set; }
    public float CurrentSprintChargeNumber => currentSprintChargeNumber;
    public string Name { get; set; }
    
    public int HorizontalDirection { get;  set; }
    public bool IsStart => isStart;
    public int SprintChargeNumber => horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintChargeNumber;

    public void StartRace(float normalizeSpeed, bool isSprint)
    {
        isStart = true;
        CurrentForwardSpeed = normalizeSpeed * horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.x;
        currentTargetForwardSpeed = horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.y;
        if (isSprint)
        {
            Sprint(true);
        }
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
        
        horseTransform.rotation = horseRaceThirdPersonData.TargetGenerator.PredefinePath.StartRotation;
        horseTransform.position = TargetGenerator.FromTimeToPoint(
            horseRaceThirdPersonData.TargetGenerator.PredefinePath.StartTime,
            horseRaceThirdPersonData.TargetGenerator.GetOffsetFromLane(horseRaceThirdPersonData.InitialLane),
            horseRaceThirdPersonData.PredefinePath);
        horseTransform.position -= horseTransform.forward * 1.0f;
        
        laneContainer.SetEntity(horseRaceThirdPersonData.InitialLane);
        worldSpaceCanvasBillBoard.CameraTransform = horseRaceThirdPersonData.Camera;
        sprintEffect = GetComponentInChildren<HorseObjectReferences>().raceModeSprintParticle;
        gameObject.name = horseRaceThirdPersonData.Name;
    }

    private float GetCurrentProgress()
    {
        return horseRaceThirdPersonData.PredefinePath.GetClosestTime(horseTransform.position) - horseRaceThirdPersonData.PredefinePath.StartTime;
    }
    
    private void Update()
    {
        if (!IsStart) return;
        UpdateAcceleration();
        UpdateSpeed();
        UpdateSprintTime();
        UpdateSprintHealingTime();
        UpdateCurrentProgress();
    }

    private void UpdateAcceleration()
    {
        CurrentAcceleration = CurrentForwardSpeed <= horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.x
                ? horseRaceThirdPersonData.HorseRaceThirdPersonStats.AccelerationRange.y
                : horseRaceThirdPersonData.HorseRaceThirdPersonStats.AccelerationRange.x;
    }

    private void UpdateCurrentProgress()
    {
        delayTime -= Time.deltaTime;
        if (delayTime >= 0.0f) return;
        
        currentRaceProgressWeight = GetCurrentProgress();
        if (previousRaceProgressWeight - currentRaceProgressWeight > 0.0f && currentRaceProgressWeight <= 0.01f)
        {
            CurrentLap++;
            if (CurrentLap == 3)
            {
                OnFinishRace.Invoke();
            }
        }
        previousRaceProgressWeight = currentRaceProgressWeight;
    }

    private void UpdateSprintHealingTime()
    {
        currentSprintChargeNumber = CurrentSprintChargeNumber + Time.deltaTime * (1.0f / horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintHealingTime);
        currentSprintChargeNumber = Mathf.Min(CurrentSprintChargeNumber,horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintChargeNumber);
    }

    private void UpdateSprintTime()
    {
        sprintEffect.SetActive(currentSprintTime > 0);
        if (!(currentSprintTime > 0.0f)) return;
        currentSprintTime = Mathf.MoveTowards(currentSprintTime, 0.0f, Time.deltaTime);
        currentSprintBonusTime = Mathf.MoveTowards(currentSprintBonusTime, 0.0f, Time.deltaTime);
        if (currentSprintTime <= 0.0f)
        {
            currentTargetForwardSpeed = horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.y;
        }
        
        if (currentSprintBonusTime <= 0.0f)
        {
            currentSprintContinuousTime = 0;
            currentTargetForwardSpeed = horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.y;
        }
    }

    private void UpdateSpeed()
    {
        CurrentForwardSpeed = Mathf.MoveTowards(
        CurrentForwardSpeed,
        currentTargetForwardSpeed,CurrentAcceleration * Time.deltaTime);
    }

    public void Sprint(bool initialSprint = false)
    {
        if (!IsAbleToSprint && !initialSprint) return;
        currentSprintChargeNumber--;
        currentSprintTime = horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintTime;
        currentTargetForwardSpeed = horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.y;
        currentSprintContinuousTime++;
        var bonusPercentage = currentSprintContinuousTime > 1f ? currentSprintContinuousTime * horseRaceThirdPersonData.HorseRaceThirdPersonStats.PercentageSpeedBonusBoostWhenSprintContinuously : 0.0f;
        var speedIncreasePercentage = currentSprintContinuousTime * horseRaceThirdPersonData.HorseRaceThirdPersonStats.PercentageSpeedBoostWhenSprint;
        CurrentForwardSpeed += (speedIncreasePercentage + bonusPercentage) * CurrentForwardSpeed;
        if (CurrentForwardSpeed > horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.y)
        {
            currentTargetForwardSpeed = CurrentForwardSpeed;
        }
        currentSprintBonusTime = HorseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintBonusTime;
    }

    public void EnableCamera(bool isBackCamera)
    {
        cameraFront.SetActive(!isBackCamera);
        cameraBack.SetActive(isBackCamera);
    }
}