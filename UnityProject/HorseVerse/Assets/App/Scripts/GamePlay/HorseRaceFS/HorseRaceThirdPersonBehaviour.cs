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
    [SerializeField] private Transform horseTransform;
    [SerializeField] private float offsetRange = 5.5f;

    private HorseRaceThirdPersonData horseRaceThirdPersonData;
    
    private bool isStart;
    private float currentTargetForwardSpeed;
    private float currentSprintTime;
    private int currentSprintNumber;
    private float currentTimeToNextSprint;
    public float CurrentForwardSpeed { get; private set; }
    public bool IsAbleToSprint => currentTimeToNextSprint == 0 && currentSprintNumber < horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintNumber;
    
    public float CurrentSprintNormalizeTime => currentSprintTime / horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintTime;
    public bool IsPlayer => horseRaceThirdPersonData.HorseRaceThirdPersonStats.IsPlayer;
    public float CurrentRaceProgressWeight => GetCurrentProgress();
    public int InitialLane => horseRaceThirdPersonData.HorseRaceThirdPersonStats.InitialLane;
    public string Name { get; set; }
    
    public int HorizontalDirection { get;  set; }
    public bool IsStart => isStart;

    public void StartRace(float normalizeSpeed)
    {
        isStart = true;
        currentTargetForwardSpeed = Mathf.Lerp(horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.x, horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.y, normalizeSpeed);
        currentSprintTime = horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintTime;
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
        playerRelatedGameObjects.ForEach(x => x.SetActive(horseRaceThirdPersonData.HorseRaceThirdPersonStats.IsPlayer));
        notPlayerRelatedGameObjects.ForEach(x => x.SetActive(horseRaceThirdPersonData.HorseRaceThirdPersonStats.IsPlayer == false));
        playerRelatedGameComponents.enabled = horseRaceThirdPersonData.HorseRaceThirdPersonStats.IsPlayer;
        notPlayerRelatedGameObjectsComponents.enabled = !horseRaceThirdPersonData.HorseRaceThirdPersonStats.IsPlayer;
        horseTransform.position = TargetGenerator.FromTimeToPoint(
            horseRaceThirdPersonData.TargetGenerator.PredefinePath.StartTime,
            horseRaceThirdPersonData.TargetGenerator.GetOffsetFromLane(horseRaceThirdPersonData.HorseRaceThirdPersonStats.InitialLane),
            horseRaceThirdPersonData.TargetGenerator.SimplyPath);
    }

    private float GetCurrentProgress()
    {
        return Mathf.InverseLerp(horseRaceThirdPersonData.PredefinePath.StartTime, 
            horseRaceThirdPersonData.PredefinePath.EndTime, 
            horseRaceThirdPersonData.PredefinePath.SimplyPath.path.GetClosestTimeOnPath(horseTransform.position));
    }
    
    private void FixedUpdate()
    {
        if (!IsStart) return;
        UpdateSpeed();
        UpdateSprintTime();
        UpdateSprintHealingTime();
    }

    private void UpdateSprintHealingTime()
    {
        if (!(currentTimeToNextSprint > 0.0f)) return;
        currentTimeToNextSprint = Mathf.MoveTowards(currentTimeToNextSprint, 0.0f, Time.deltaTime);
        if (currentTimeToNextSprint <= 0.0f)
        {
            currentTargetForwardSpeed = horseRaceThirdPersonData.HorseRaceThirdPersonStats.ForwardSpeedRange.x;
        }
    }

    private void UpdateSprintTime()
    {
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
}