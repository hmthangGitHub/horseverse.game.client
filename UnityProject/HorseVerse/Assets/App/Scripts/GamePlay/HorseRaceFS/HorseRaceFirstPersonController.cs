using System;
using Lean.Touch;
using UnityEngine;
using UnityEngine.AI;

public partial class HorseRaceFirstPersonController : MonoBehaviour
{
    [SerializeField] private GameObject[] playerRelatedGameObjects;
    [SerializeField] private GameObject[] notPlayerRelatedGameObjects;
    [SerializeField] private Collider playerRelatedGameComponents;
    [SerializeField] private NavMeshAgent notPlayerRelatedGameObjectsComponents;
    [SerializeField] private Transform horseTransform;
    [SerializeField] private PredefinePath predefinePredefinePath;
    [SerializeField] private float horizontalSpeed = 5.0f;
    [SerializeField] private float offsetRange = 5.0f;
    [SerializeField] private bool isPlayer;
    [SerializeField] private float forwardSpeed = 0;
    [SerializeField] private float maxSpeed = 10;
    private HorseRaceThirdPersonData horseRaceThirdPersonData;
    public float CurrentRaceProgressWeight => GetCurrentWeight();

    [SerializeField] public float MaxSpeed => maxSpeed;
    private bool isStart;
    
    public int HorizontalDirection { get;  set; }

    public float ForwardSpeed
    {
        get => forwardSpeed;
        private set => forwardSpeed = value;
    }

    public bool IsStart
    {
        get => isStart;
        set
        {
            if (isStart == value) return;
            isStart = value;
            OnStart(value);
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

    public PredefinePath PredefinePath => predefinePredefinePath;

    public float OffsetRange
    {
        get => offsetRange;
        set => offsetRange = value;
    }

    public float HorizontalSpeed
    {
        get => horizontalSpeed;
        set => horizontalSpeed = value;
    }

    private void OnSetData()
    {
        playerRelatedGameObjects.ForEach(x => x.SetActive(this.isPlayer));
        notPlayerRelatedGameObjects.ForEach(x => x.SetActive(this.isPlayer == false));
        playerRelatedGameComponents.enabled = this.isPlayer;
        notPlayerRelatedGameObjectsComponents.enabled = !this.isPlayer;
    }

    private void OnStart(bool isStart)
    {
        if (isStart)
            ForwardSpeed = MaxSpeed;
    }
    
    private float GetCurrentWeight()
    {
        return Mathf.InverseLerp(PredefinePath.StartTime, PredefinePath.EndTime, PredefinePath.SimplyPath.path.GetClosestTimeOnPath(horseTransform.position));
    }
}