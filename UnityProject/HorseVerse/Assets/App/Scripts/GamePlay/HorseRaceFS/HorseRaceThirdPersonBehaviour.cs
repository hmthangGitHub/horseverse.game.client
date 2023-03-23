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
    [SerializeField] private GameObject cameraFront;
    [SerializeField] private GameObject cameraBack;

    private HorseRaceThirdPersonData horseRaceThirdPersonData;
    
    private bool isStart;
    private float currentTargetForwardSpeed;
    private float currentSprintTime;
    private int currentSprintNumber;
    private float currentTimeToNextSprint;
    public float CurrentForwardSpeed { get; private set; }
    public bool IsAbleToSprint => currentTimeToNextSprint == 0 && currentSprintNumber < horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintNumber;
    
    public float CurrentSprintNormalizeTime => currentSprintTime / horseRaceThirdPersonData.HorseRaceThirdPersonStats.SprintTime;
    public bool IsPlayer => horseRaceThirdPersonData.IsPlayer;
    public float CurrentRaceProgressWeight => GetCurrentProgress();
    public int InitialLane => horseRaceThirdPersonData.InitialLane;
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
        playerRelatedGameObjects.ForEach(x => x.SetActive(horseRaceThirdPersonData.IsPlayer));
        notPlayerRelatedGameObjects.ForEach(x => x.SetActive(horseRaceThirdPersonData.IsPlayer == false));
        playerRelatedGameComponents.enabled = horseRaceThirdPersonData.IsPlayer;
        notPlayerRelatedGameObjectsComponents.enabled = !horseRaceThirdPersonData.IsPlayer;
        horseTransform.position = TargetGenerator.FromTimeToPoint(
            horseRaceThirdPersonData.TargetGenerator.PredefinePath.StartTime,
            horseRaceThirdPersonData.TargetGenerator.GetOffsetFromLane(horseRaceThirdPersonData.InitialLane),
            horseRaceThirdPersonData.PredefinePath);

        horseTransform.rotation = horseRaceThirdPersonData.TargetGenerator.PredefinePath.StartRotation;
    }

    private float GetCurrentProgress()
    {
        return 0;
        // horseRaceThirdPersonData.TargetGenerator.Spline.FindNearestPointTo(horseTransform.position, out var normalizedT);
        // return normalizedT;
        // return Mathf.InverseLerp(horseRaceThirdPersonData.PredefinePath.StartTime, 
        //     horseRaceThirdPersonData.PredefinePath.EndTime, 
        //     horseRaceThirdPersonData.PredefinePath.SimplyPath.path.GetClosestTimeOnPath(horseTransform.position));
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

    public void EnableCamera(bool isBackCamera)
    {
        cameraFront.SetActive(!isBackCamera);
        cameraBack.SetActive(!isBackCamera);
    }
}