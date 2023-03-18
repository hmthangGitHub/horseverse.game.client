using System;
using Lean.Touch;
using UnityEngine;
using UnityEngine.AI;

public partial class HorseRaceThirdPersonBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject[] playerRelatedGameObjects;
    [SerializeField] private GameObject[] notPlayerRelatedGameObjects;
    [SerializeField] private Collider playerRelatedGameComponents;
    [SerializeField] private NavMeshAgent notPlayerRelatedGameObjectsComponents;
    [SerializeField] private Transform horseTransform;
    [SerializeField] private float offsetRange = 5.5f;

    private HorseRaceThirdPersonMasterData horseRaceThirdPersonMasterData;
    
    private bool isStart;
    private float currentTargetForwardSpeed;
    private float currentSprintTime;
    private int currentSprintNumber;
    private float currentTimeToNextSprint;
    public float CurrentForwardSpeed { get; private set; }
    public bool IsAbleToSprint => currentTimeToNextSprint == 0 && currentSprintNumber < HorseRaceThirdPersonMasterData.SprintNumber;
    
    public float CurrentSprintNormalizeTime => currentSprintTime / horseRaceThirdPersonMasterData.SprintTime;
    public float CurrentRaceProgressWeight => GetCurrentProgress();
    public float currentRaceProgressWeight;
    
    public int HorizontalDirection { get;  set; }
    public bool IsStart => isStart;

    public void StartRace(float normalizeSpeed)
    {
        isStart = true;
        currentTargetForwardSpeed = Mathf.Lerp(HorseRaceThirdPersonMasterData.ForwardSpeedRange.x, HorseRaceThirdPersonMasterData.ForwardSpeedRange.y, normalizeSpeed);
        currentSprintTime = HorseRaceThirdPersonMasterData.SprintTime;
    }

    public HorseRaceThirdPersonMasterData HorseRaceThirdPersonMasterData
    {
        get => horseRaceThirdPersonMasterData;
        set
        {
            horseRaceThirdPersonMasterData = value;
            OnSetData();
        }
    }

    public float OffsetRange => offsetRange;

    private void OnSetData()
    {
        playerRelatedGameObjects.ForEach(x => x.SetActive(horseRaceThirdPersonMasterData.IsPlayer));
        notPlayerRelatedGameObjects.ForEach(x => x.SetActive(horseRaceThirdPersonMasterData.IsPlayer == false));
        playerRelatedGameComponents.enabled = horseRaceThirdPersonMasterData.IsPlayer;
        notPlayerRelatedGameObjectsComponents.enabled = !horseRaceThirdPersonMasterData.IsPlayer;
        horseTransform.position = TargetGenerator.FromTimeToPoint(
            horseRaceThirdPersonMasterData.TargetGenerator.PredefinePath.StartTime,
            horseRaceThirdPersonMasterData.TargetGenerator.GetOffsetFromLane(horseRaceThirdPersonMasterData.InitialLane),
            horseRaceThirdPersonMasterData.TargetGenerator.SimplyPath);
    }

    private float GetCurrentProgress()
    {
        return Mathf.InverseLerp(horseRaceThirdPersonMasterData.PredefinePath.StartTime, 
            horseRaceThirdPersonMasterData.PredefinePath.EndTime, 
            horseRaceThirdPersonMasterData.PredefinePath.SimplyPath.path.GetClosestTimeOnPath(horseTransform.position));
    }
    
    private void FixedUpdate()
    {
        if (!IsStart) return;
        UpdateSpeed();
        UpdateSprintTime();
        UpdateSprintHealingTime();
        currentRaceProgressWeight = CurrentRaceProgressWeight;
    }

    private void UpdateSprintHealingTime()
    {
        if (!(currentTimeToNextSprint > 0.0f)) return;
        currentTimeToNextSprint = Mathf.MoveTowards(currentTimeToNextSprint, 0.0f, Time.deltaTime);
        if (currentTimeToNextSprint <= 0.0f)
        {
            currentTargetForwardSpeed = horseRaceThirdPersonMasterData.ForwardSpeedRange.x;
        }
    }

    private void UpdateSprintTime()
    {
        if (!(currentSprintTime > 0.0f)) return;
        
        currentSprintTime = Mathf.MoveTowards(currentSprintTime, 0.0f, Time.deltaTime);
        if (currentSprintTime <= 0.0f)
        {
            currentTargetForwardSpeed = horseRaceThirdPersonMasterData.ForwardSpeedRange.x;
        }

    }

    private void UpdateSpeed()
    {
        CurrentForwardSpeed = Mathf.MoveTowards(
        CurrentForwardSpeed,
        currentTargetForwardSpeed,
        HorseRaceThirdPersonMasterData.Acceleration * Time.deltaTime);
    }

    public void Sprint()
    {
        if (!IsAbleToSprint) return;
        currentSprintNumber++;
        currentTimeToNextSprint = horseRaceThirdPersonMasterData.SprintHealingTime;
        currentSprintTime = HorseRaceThirdPersonMasterData.SprintTime;
        currentTargetForwardSpeed = horseRaceThirdPersonMasterData.ForwardSpeedRange.y;
    }
}