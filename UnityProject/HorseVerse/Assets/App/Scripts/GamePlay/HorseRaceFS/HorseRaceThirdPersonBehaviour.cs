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
    private int lap = 0;
    private float currentRaceProgressWeight;
    private bool isStart;
    private float currentTargetForwardSpeed;
    private int currentSprintContinuousTime;
    private float currentSprintTime;
    private float currentSprintChargeNumber;
    private float currentTimeToNextSprint;
    private float delayTime = 1.0f;
    public float CurrentForwardSpeed { get; private set; }
    public bool IsAbleToSprint => CurrentSprintChargeNumber >= 1;
    
    public float CurrentSprintNormalizeTime => currentSprintTime / horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintTime;

    public float CurrentChargeNormalize => CurrentSprintChargeNumber /
                                           horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintChargeNumber;
    public bool IsPlayer => horseRaceThirdPersonData.IsPlayer;
    
    public float CurrentRaceProgressWeight => currentRaceProgressWeight + lap;
    public int InitialLane => horseRaceThirdPersonData.InitialLane;
    public float CurrentAcceleration { get; private set; }
    public float CurrentSprintChargeNumber => currentSprintChargeNumber;
    public string Name { get; set; }
    
    public int HorizontalDirection { get;  set; }
    public bool IsStart => isStart;
    public int SprintChargeNumber => horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintChargeNumber;

    public void StartRace(float normalizeSpeed)
    {
        isStart = true;
        // currentTargetForwardSpeed = Mathf.Lerp(horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.x, horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.y, normalizeSpeed);
        currentTargetForwardSpeed = horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.y;
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
        gameObject.name = horseRaceThirdPersonData.Name;
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
        if (currentRaceProgressWeight >= 1.0f && lap == 0)
        {
            lap++;
            OnFinishRace.Invoke();
        }
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
        if (currentSprintTime <= 0.0f)
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

    public void Sprint()
    {
        if (!IsAbleToSprint) return;
        currentSprintChargeNumber--;
        currentTimeToNextSprint = horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintHealingTime;
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
    }

    public void EnableCamera(bool isBackCamera)
    {
        cameraFront.SetActive(!isBackCamera);
        cameraBack.SetActive(isBackCamera);
    }
}