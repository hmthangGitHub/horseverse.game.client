using System;
using Lean.Touch;
using UnityEngine;

public partial class HorseRaceFirstPersonController : MonoBehaviour
{
    [SerializeField] private GameObject[] playerRelatedGameObjects;
    [SerializeField] private GameObject[] notPlayerRelatedGameObjects;
    [SerializeField] private PredefinePath predefinePredefinePath;
    [SerializeField] private float horizontalSpeed = 5.0f;
    [SerializeField] private float offsetRange = 5.0f;
    [SerializeField] private bool isPlayer;
    [SerializeField] public float offset;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float forwardSpeed = 0;
    [SerializeField] private float maxSpeed = 10;
    private HorseRaceThirdPersonData horseRaceThirdPersonData;

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
        set => horseRaceThirdPersonData = value;
    }

    public PredefinePath PredefinePath => predefinePredefinePath;

    public float Offset
    {
        get => offset;
        set => offset = value;
    }

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

    private void Start()
    {
        playerRelatedGameObjects.ForEach(x => x.SetActive(this.isPlayer));
        notPlayerRelatedGameObjects.ForEach(x => x.SetActive(this.isPlayer == false));
    }

    private void OnStart(bool isStart)
    {
        if (isStart)
            ForwardSpeed = MaxSpeed;
    }
}