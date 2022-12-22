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
    [SerializeField] private LeanFingerUp touchUp;
    [SerializeField] private LeanFingerDown touchDown;
    
    [SerializeField] private float currentForwardVelocity;
    [SerializeField] private float currentHorizontalVelocity;
    [SerializeField] private float defaultGravity;
    
    [SerializeField] private GameObject cam1;
    [SerializeField] private GameObject cam2;
    [SerializeField] private GameObject cam3;
    [SerializeField] private Transform horsePosition;
    [SerializeField] private Vector3 groundVelocity;
    [SerializeField]
    private Transform pivotPoint;

    private bool isStart;
    private bool isDead;
    private bool isGrounded = true;
    private bool isJumping;
    
    private Animator animator;
    private Animator Animator => animator ??= GetComponentInChildren<Animator>();
    private float currentAirTime;
    private float MaxAirTime => Mathf.Abs(JumpVelocity / DefaultGravity) + masterHorseTrainingProperty.FallAirTimeMax;

    public event Action OnTakeCoin = ActionUtility.EmptyAction.Instance;
    public event Action OnDeadEvent = ActionUtility.EmptyAction.Instance;

    public float JumpVelocity => masterHorseTrainingProperty.JumpVelocity;
    public float HorizontalVelocity => masterHorseTrainingProperty.HorizontalVelocity;
    public float ForwardVelocity => masterHorseTrainingProperty.ForwardVelocity;
    public float LowJumpMultiplier => masterHorseTrainingProperty.FallGravityMultiplier;
    public float DefaultGravity => defaultGravity;
    public float lastTapTimeStamp = 0;
    public int index = 0;
    private MasterHorseTrainingProperty masterHorseTrainingProperty;
    private string horseModelPath;
    private enum LastTap
    {
        None,
        Left,
        Right
    }
    private LastTap lastTap;
    private CinemachineOrbitalTransposer cinemachineOrbitalTransposer;
    private float animationHorizontal;
    private static readonly int VerticalVelocity = Animator.StringToHash("VerticalVelocity");
    private int horizontalDirection = 0;

    private void AddInputEvents()
    {
        touchDown.OnFinger.AddListener(finger =>
        {
            if (!IsStart) return;

            if (finger.Down && finger.StartScreenPosition.x < Screen.width / 2)
            {
                horizontalDirection += 1;
            }
            else if (finger.Down && finger.StartScreenPosition.x > Screen.width / 2)
            {
                horizontalDirection -= 1;
            }
            DetectDoubleTap(finger);
        });

        touchUp.OnFinger.AddListener(finger =>
        {
            if (!IsStart) return;

            if (finger.Up && finger.StartScreenPosition.x < Screen.width / 2)
            {
                horizontalDirection -= 1;
            }
            else if (finger.Up && finger.StartScreenPosition.x > Screen.width / 2)
            {
                horizontalDirection += 1;
            }
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

    public async UniTask Initialize(MasterHorseTrainingProperty masterHorseTrainingProperty,
                                    HorseMeshInformation horseMeshInformation)
    {
        this.masterHorseTrainingProperty = masterHorseTrainingProperty;
        SetCameraYaw(masterHorseTrainingProperty.RunCameraRotation, cam1.transform);
        SetCameraYaw(masterHorseTrainingProperty.FallCameraRotation, cam2.transform);
        horseModelPath = horseMeshInformation.horseModelPath;
        var horse = await HorseMeshAssetLoader.InstantiateHorse(horseMeshInformation);
        horse.transform.parent = horsePosition;
        horse.transform.localPosition = Vector3.zero;
        horse.transform.localScale = Vector3.one;
    }

    private void SetCameraYaw(float rotation, Transform cameraTransform)
    {
        var localRotation =  cameraTransform.localEulerAngles;
        localRotation.x = rotation;
        cameraTransform.localEulerAngles = localRotation;
    }

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

    public Vector2 AirTime => new Vector2(masterHorseTrainingProperty.FallAirTimeMin, masterHorseTrainingProperty.FallAirTimeMax);

    private void OnStart(bool isStart)
    {
        if (isStart)
        {
            cam3.SetActive(false);
            cam1.SetActive(true);
            
            DOTween.To(val =>
            {
                //Debug.Log($"Speed {val}");
                Animator.SetFloat(Speed, val);
                currentForwardVelocity = Mathf.Lerp(0.0f, ForwardVelocity, val);
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
        }
        
        if(!IsStart)
        {
            cinemachineOrbitalTransposer ??= cam3.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineOrbitalTransposer>();
            cinemachineOrbitalTransposer.m_XAxis.m_InputAxisValue = 0.03f;
        }
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
            AudioManager.Instance.StopSound();
        }
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
    }
}
