using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;

public class HorseTrainingControllerV2 : MonoBehaviour, IDisposable
{
    private const string Obstacle = "Obstacle";
    private const string Coin = "Coin";
    private const string Bridge = "Bridge";
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Jumping = Animator.StringToHash("IsJumping");
    
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private GameObject landingVFX;
    [SerializeField] private GameObject trailVFX;
    [SerializeField] private GameObject takeCoinVFX;
    [SerializeField] private GameObject jumpVFX;
    [SerializeField] private LeanFingerUp touchUp;
    [SerializeField] private LeanFingerDown touchDown;
    [SerializeField] private LeanFingerUpdate touchUpdate;
    [SerializeField] private LeanFingerSwipe touchDrap;

    [SerializeField] private float currentForwardVelocity;
    [SerializeField] private float currentHorizontalVelocity;
    [SerializeField] private float defaultGravity;
    
    [SerializeField] private GameObject cam1;
    [SerializeField] private GameObject cam2;
    [SerializeField] private GameObject cam3;
    [SerializeField] private Transform horsePosition;
    [SerializeField] private Vector3 groundVelocity;
    [SerializeField] private Transform pivotPoint;

    [Space, Header("INPUT SETTINGS")]
    [SerializeField, Range(0, 1)] private float delayTimeForTouch = 0.05f;
    [SerializeField] private float clampOffsetY = 25;

    private bool isStart;
    private bool isDead;
    private bool isGrounded = true;
    private bool isJumping;
    
    private Animator animator;
    private Animator Animator => animator ??= GetComponentInChildren<Animator>();
    private float currentAirTime;
    public CryptoField<float> TotalRunTimeEncrypt { get; } = new CryptoField<float>();
    public CryptoField<int> TotalCoinEncrypt { get; } = new CryptoField<int>();
    private float MaxAirTime => Mathf.Abs(JumpVelocity / DefaultGravity) + masterHorseTrainingProperty.FallAirTimeMax;
    public Vector2 AirTime => new Vector2(masterHorseTrainingProperty.FallAirTimeMin, masterHorseTrainingProperty.FallAirTimeMax);
    public event Action OnTakeCoin = ActionUtility.EmptyAction.Instance;
    public event Action OnUpdateRunTime = ActionUtility.EmptyAction.Instance;
    public event Action OnDeadEvent = ActionUtility.EmptyAction.Instance;

    public float JumpVelocity => masterHorseTrainingProperty.JumpVelocity;
    public float HorizontalVelocity => masterHorseTrainingProperty.HorizontalVelocity;

    [SerializeField]
    public float ForwardVelocity { get; private set; }

    private float GetForwardVelocityFromCurrentDifficulty()
    {
        return masterTrainingDifficultyContainer.MasterTrainingDifficultyIndexer.First(x =>
            x.Value.Difficulty == CurrentDifficulty).Value.ForwardVelocity;
    }

    public float LowJumpMultiplier => masterHorseTrainingProperty.FallGravityMultiplier;
    public float DefaultGravity => defaultGravity;
    public float lastTapTimeStamp = 0;
    public int index = 0;
    private MasterHorseTrainingProperty masterHorseTrainingProperty;
    private MasterTrainingDifficultyContainer masterTrainingDifficultyContainer;
    private string horseModelPath;
    private enum LastTap
    {
        None,
        Left,
        Right,
    }
    private LastTap lastTap;
    private CinemachineOrbitalTransposer cinemachineOrbitalTransposer;
    private float animationHorizontal;
    private static readonly int VerticalVelocity = Animator.StringToHash("VerticalVelocity");
    private int horizontalDirection = 0;
    private int currentDifficulty = 0;

    public bool IsGrounded
    {
        get => isGrounded;
        set
        {
            if (isGrounded != value)
            {
                isGrounded = value;
                OnGrounded(isGrounded);
            }
        }
    }

    public bool IsStart
    {
        get => isStart;
        set
        {
            if (isStart != value)
            {
                isStart = value;
                OnStart(value);
            }
        }
    }

    public int CurrentDifficulty
    {
        get => currentDifficulty;
        set
        {
            if (currentDifficulty == value) return;
            currentDifficulty = value;
            ForwardVelocity = GetForwardVelocityFromCurrentDifficulty();
        }
    }

    private void AddInputEvents()
    {
        touchDown.OnFinger.AddListener(finger =>
        {
            if (!IsStart) return;

            //if (finger.Down && finger.StartScreenPosition.x < Screen.width / 2)
            //{
            //    horizontalDirection += 1;
            //}
            //else if (finger.Down && finger.StartScreenPosition.x > Screen.width / 2)
            //{
            //    horizontalDirection -= 1;
            //}

            //DetectDoubleTap(finger);
            HandleFirstTouch(finger);
        });

        touchUpdate.OnFinger.AddListener(finger =>
        {
            if (!IsStart) return;
            HandleUpdateTouch(finger);
        });

        touchUp.OnFinger.AddListener(finger =>
        {
            if (!IsStart) return;

            if (finger.Up && finger.StartScreenPosition.x < Screen.width / 2)
            {
                if(horizontalDirection == 1)
                    horizontalDirection -= 1;
            }
            else if (finger.Up && finger.StartScreenPosition.x > Screen.width / 2)
            {
                if (horizontalDirection == -1)
                    horizontalDirection += 1;
            }
        });

        touchDrap.OnDelta.AddListener(f =>
        {
            if (!IsStart) return;
            DetectSwideJump(f);
        });
    }

    private void DetectDoubleTap(LeanFinger finger)
    {
        var currentTouch = LastTap.None;
        if ( finger.StartScreenPosition.x < Screen.width / 2)
        {
            currentTouch = LastTap.Left;
        }
        else if ( finger.StartScreenPosition.x > Screen.width / 2)
        {
            currentTouch = LastTap.Right;
        }

        if (lastTap != LastTap.None && lastTap != currentTouch && (Time.realtimeSinceStartup - lastTapTimeStamp) < 0.2f)
        {
            ManualJump();
        }

        lastTapTimeStamp = Time.realtimeSinceStartup;
        lastTap = currentTouch;
    }

    private void HandleFirstTouch(LeanFinger finger)
    {
        var currentTouch = LastTap.None;
        if (finger.StartScreenPosition.x < Screen.width / 2)
        {
            currentTouch = LastTap.Left;
        }
        else if (finger.StartScreenPosition.x > Screen.width / 2)
        {
            currentTouch = LastTap.Right;
        }

        lastTapTimeStamp = Time.realtimeSinceStartup;
        lastTap = currentTouch;
    }

    private void HandleUpdateTouch(LeanFinger finger)
    {
        if (!finger.Up && lastTap == LastTap.Left && finger.StartScreenPosition.x < Screen.width / 2 
            && Mathf.Abs(finger.ScreenPosition.y - finger.StartScreenPosition.y) < clampOffsetY && (Time.realtimeSinceStartup - lastTapTimeStamp) > delayTimeForTouch)
        {
            horizontalDirection = 1;
        }
        else if (!finger.Up && lastTap == LastTap.Right && finger.StartScreenPosition.x > Screen.width / 2
           && Mathf.Abs(finger.ScreenPosition.y - finger.StartScreenPosition.y) < clampOffsetY && (Time.realtimeSinceStartup - lastTapTimeStamp) > delayTimeForTouch)
        {
            horizontalDirection = -1;
        }
        //lastTapTimeStamp = Time.realtimeSinceStartup;
    }

    private void DetectSwideJump(Vector2 f)
    {
        if (lastTap != LastTap.None)
        {
            ManualJump();
        }

        lastTapTimeStamp = Time.realtimeSinceStartup;
    }

    public async UniTask Initialize(MasterHorseTrainingProperty masterHorseTrainingProperty,
                                    MasterTrainingDifficultyContainer masterTrainingDifficultyContainer,
                                    HorseMeshInformation horseMeshInformation)
    {
        this.masterHorseTrainingProperty = masterHorseTrainingProperty;
        this.masterTrainingDifficultyContainer = masterTrainingDifficultyContainer;
        SetCameraYaw(masterHorseTrainingProperty.RunCameraRotation, cam1.transform);
        SetCameraYaw(masterHorseTrainingProperty.FallCameraRotation, cam2.transform);
        horseModelPath = horseMeshInformation.horseModelPath;
        var horse = await HorseMeshAssetLoader.InstantiateHorse(horseMeshInformation);
        horse.transform.parent = horsePosition;
        horse.transform.localPosition = Vector3.zero;
        horse.transform.localScale = Vector3.one;
        CurrentDifficulty = 1;
        TotalCoinEncrypt.Value = 0;
        TotalRunTimeEncrypt.Value = 0;
    }

    private void SetCameraYaw(float rotation, Transform cameraTransform)
    {
        var localRotation =  cameraTransform.localEulerAngles;
        localRotation.x = rotation;
        cameraTransform.localEulerAngles = localRotation;
    }

    private void OnStart(bool isStart)
    {
        if (isStart)
        {
            cam3.SetActive(false);
            cam1.SetActive(true);
            
            DOTween.To(val =>
            {
                Animator.SetFloat(Speed, val);
                currentHorizontalVelocity = Mathf.Lerp(0.0f, HorizontalVelocity, val);
            }, 0.0f, 1.0f, 2.0f).SetEase(Ease.Linear);
            AddInputEvents();
        }
    }

    private void Update()
    {
        if (IsStart && !isDead)
        {
            CheckIfGrounded();
            CheckIfFall();
            ControlHorse();
            UpdateHorizontalAnimation();
            UpdateJumpAnimation();
            UpdateForwardVelocity();
            UpdateDifficulty();
        }
        
        if(!IsStart)
        {
            cinemachineOrbitalTransposer ??= cam3.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineOrbitalTransposer>();
            cinemachineOrbitalTransposer.m_XAxis.m_InputAxisValue = 0.03f;
        }
    }

    private void UpdateForwardVelocity()
    {
        currentForwardVelocity = Mathf.Lerp(currentForwardVelocity, ForwardVelocity, Time.deltaTime);
    }

    private void UpdateDifficulty()
    {
        var oldRunTime = TotalRunTimeEncrypt.Value;
        TotalRunTimeEncrypt.Value += Time.deltaTime;
        OnUpdateRunTime();
        if (Mathf.FloorToInt(TotalRunTimeEncrypt.Value) == Mathf.FloorToInt(oldRunTime)) return;
        
        var totalScore = (int)(TotalRunTimeEncrypt.Value * 2) + TotalCoinEncrypt.Value;
        CurrentDifficulty = masterTrainingDifficultyContainer.MasterTrainingDifficultyIndexer
                                                             .Last(x => x.Value.RequiredScore <= totalScore)
                                                             .Value.Difficulty;
    }

    public Vector3 GetRelativePoint()
    {
        var outputCamera = CinemachineCore.Instance.GetActiveBrain(0)
                                          .OutputCamera;
        var ray = outputCamera.ScreenPointToRay(new Vector3(-10, Screen.height + 10));

        var plane = new Plane(outputCamera.transform.forward, transform.position);

        plane.Raycast(ray, out var enter);
        var hitPoint = ray.GetPoint(enter);
        var relative = hitPoint - transform.position;
        Debug.DrawLine(transform.position, hitPoint);
        return relative;
    }

    private void UpdateJumpAnimation()
    {
        animator.SetFloat(VerticalVelocity, rigidbody.velocity.y);
        animator.SetBool(Jumping, isJumping);
    }

    private void UpdateHorizontalAnimation()
    {
        animationHorizontal = Mathf.Lerp(animationHorizontal, Math.Sign(-horizontalDirection), Time.deltaTime * 10.0f);
        Animator.SetFloat(Horizontal, animationHorizontal);
    }

    private void ControlHorse()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ManualJump();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            horizontalDirection += 1;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            horizontalDirection -= 1;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            horizontalDirection -= 1;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            horizontalDirection += 1;
        }
    }

    public void ManualJump()
    {
        if (isGrounded)
        {
            Jump(true);
            isJumping = true;
            animator.CrossFade("JumpStart", 0.1f, 0);
            var vfx = Instantiate(jumpVFX);
            vfx.transform.position = pivotPoint.position + Vector3.up * 0.1f;
            AudioManager.Instance.StopSound();
        }
    }

    public void ManualTurn(float x, float y)
    {
        if(x != 0)
            horizontalDirection = -(int)x;
    }

    private void CheckIfFall()
    {
        if (!isGrounded)
        {
            currentAirTime += Time.deltaTime;
            
            if (currentAirTime > MaxAirTime)
            {
                OnDead();
                cam1.transform.parent = null;
                cam2.transform.parent = null;
            }    
        }
        
    }

    private void Jump(bool manual)
    {
        rigidbody.velocity = Vector3.up * JumpVelocity;
    }

    private void FixedUpdate()
    {
        if (!IsStart) return;
        rigidbody.velocity = new Vector3(-horizontalDirection * currentHorizontalVelocity, rigidbody.velocity.y, currentForwardVelocity);
        if (rigidbody.velocity.y < 0)
        {
            
            Physics.gravity = Vector3.up * DefaultGravity * LowJumpMultiplier;
        }
        else
        {
            Physics.gravity = Vector3.up * DefaultGravity;
        }
    }

    private void CheckIfGrounded()
    {
        Debug.DrawLine(pivotPoint.position, pivotPoint.position + -Vector3.up * 0.15f, Color.red);
        IsGrounded = Physics.RaycastAll(pivotPoint.position, -Vector3.up, 0.15f)
            .Any(x => x.collider.CompareTag("Platform"));
    }

    private void OnGrounded(bool isGrounded)
    {
        if (isGrounded && IsStart)
        {
            rigidbody.velocity = Vector3.Scale(new Vector3(0.0f, 0.0f, 1.0f), rigidbody.velocity);
            var minAirTimeToShake = Mathf.Abs(JumpVelocity / DefaultGravity) * 2 + 0.1f;
            if (currentAirTime > minAirTimeToShake)
            {
                cam1.SetActive(true);
                cam2.SetActive(false);
                var strength = Map(currentAirTime, minAirTimeToShake, MaxAirTime, 2.0f, 5.0f);
                var time = Map(currentAirTime, minAirTimeToShake, MaxAirTime, 0.1f, 0.35f);
                cam1.transform.DOShakePosition(time, strength, 20);
                var vfx = Instantiate(landingVFX);
                vfx.transform.position = pivotPoint.position + Vector3.up * 0.1f;
                trailVFX.SetActive(false);
                AudioManager.Instance.PlaySound(AudioManager.HorseLand);
                AudioManager.Instance.PlaySoundHasLoop(AudioManager.HorseRunTraining);
            }
            currentAirTime = 0.0f;
        }

        if (isGrounded && isJumping)
        {
            isJumping = false;
            AudioManager.Instance.PlaySoundHasLoop(AudioManager.HorseRunTraining);
        }
    }
    
    float Map(float v, float a1, float a2, float b1, float b2)
    {
        return b1 + (v-a1)*(b2-b1)/(a2-a1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsStart) return;

        if (other.CompareTag(Bridge))
        {
            cam1.SetActive(false);
            cam2.SetActive(true);
        }
        
        if (other.CompareTag(Obstacle) && !isDead)
        {
            OnDead();
            cam1.transform.DOShakePosition(0.35f, 2.5f);
            cam2.transform.DOShakePosition(0.35f, 2.5f);
        }

        if (other.CompareTag(Coin))
        {
            OnTakeCoin.Invoke();
            TotalCoinEncrypt.Value++;
            takeCoinVFX.gameObject.SetActive(false);
            takeCoinVFX.gameObject.SetActive(true);
            SoundController.PlayHitCoin();
        }
    }

    private void OnDead()
    {
        isDead = true;
        currentForwardVelocity = 0.0f;
        currentHorizontalVelocity = 0.0f;
        OnDeadEvent.Invoke();
    }

    public void Dispose()
    {
        PrimitiveAssetLoader.UnloadAssetAtPath(horseModelPath);
        masterHorseTrainingProperty = default;
        masterTrainingDifficultyContainer = default;
    }

}
