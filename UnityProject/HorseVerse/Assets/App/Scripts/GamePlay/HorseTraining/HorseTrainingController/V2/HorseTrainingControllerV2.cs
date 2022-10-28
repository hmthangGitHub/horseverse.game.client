using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;

public class HorseTrainingControllerV2 : MonoBehaviour
{
    private const string Obstacle = "Obstacle";
    private const string Coin = "Coin";
    private const string Bridge = "Bridge";
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private GameObject landingVFX;
    [SerializeField] private GameObject trailVFX;
    [SerializeField] private LeanFingerUp touchUp;
    [SerializeField] private LeanFingerDown touchDown;
    [SerializeField] private LeanFingerTap doubleTap;
    
    [SerializeField] private float currentForwardVelocity;
    [SerializeField] private float currentHorizontalVelocity;
    [SerializeField] private float defaultGravity;
    [SerializeField] Vector2 airTime;
    
    [SerializeField] private GameObject cam1;
    [SerializeField] private GameObject cam2;
    [SerializeField] private GameObject cam3;
    
    [SerializeField] private Vector3 groundVelocity;

    private bool isStart;
    private bool isDead;
    private bool isGrounded;
    private bool isJustManualJump = true;
    public Transform pivotPoint;
    private Animator animator;
    private float currentAirTime;
    private float MaxAirTime => JumpVelocity / DefaultGravity + AirTime.y + 1.0f;

    public event Action OnTakeCoin = ActionUtility.EmptyAction.Instance;
    public event Action OnDeadEvent = ActionUtility.EmptyAction.Instance;

    public float JumpVelocity => masterHorseTrainingProperty.JumpVelocity;
    public float HorizontalVelocity => masterHorseTrainingProperty.HorizontalVelocity;
    public float ForwardVelocity => masterHorseTrainingProperty.ForwardVelocity;
    public float LowJumpMultiplier => masterHorseTrainingProperty.FallGravityMultiplier;
    public float DefaultGravity => defaultGravity;
    public float lastTap = 0;
    public int index = 0;
    private MasterHorseTrainingProperty masterHorseTrainingProperty;

    private void AddInputEvents()
    {
        touchDown.OnFinger.AddListener(finger =>
        {
            if (!IsStart) return;

            if (finger.Down && finger.StartScreenPosition.x < Screen.width / 2)
            {
                groundVelocity += Vector3.right * HorizontalVelocity;
            }
            else if (finger.Down && finger.StartScreenPosition.x > Screen.width / 2)
            {
                groundVelocity -= Vector3.right * HorizontalVelocity;
            }
        });

        touchUp.OnFinger.AddListener(finger =>
        {
            if (!IsStart) return;

            if (finger.Up && finger.StartScreenPosition.x < Screen.width / 2)
            {
                groundVelocity -= Vector3.right * HorizontalVelocity;
            }
            else if (finger.Up && finger.StartScreenPosition.x > Screen.width / 2)
            {
                groundVelocity += Vector3.right * HorizontalVelocity;
            }
        });

        doubleTap.OnFinger.AddListener(finger =>
        {
            if (!IsStart) return;
            
            if (finger.Index != index)
            {
                if (Time.realtimeSinceStartup - lastTap < 0.2f)
                {
                    ManualJump();
                }
            }

            lastTap = Time.realtimeSinceStartup;
            index = finger.Index;
        });
    }

    public void SetMasterHorseTrainingProperty(MasterHorseTrainingProperty masterHorseTrainingProperty)
    {
        this.masterHorseTrainingProperty = masterHorseTrainingProperty;
        SetCameraYaw(masterHorseTrainingProperty.RunCameraRotation, cam1.transform);
        SetCameraYaw(masterHorseTrainingProperty.FallCameraRotation, cam2.transform);
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

    public Vector2 AirTime
    {
        get => airTime;
        set => airTime = value;
    }

    private void OnStart(bool b)
    {
        if (b)
        {
            currentForwardVelocity = ForwardVelocity;  
            currentHorizontalVelocity = HorizontalVelocity;
            cam3.SetActive(false);
            cam1.SetActive(true);
            animator = GetComponentInChildren<Animator>();
            animator.SetFloat("Speed", 1.0f);
            
            AddInputEvents();
        }
    }

    private void Update()
    {
        if (IsStart && !isDead)
        {
            CheckIfGrounded();
            CheckIfFall();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ManualJump();
            }
        
            if (Input.GetKeyDown(KeyCode.A))
            {
                groundVelocity += Vector3.right * HorizontalVelocity;
            }
            else if(Input.GetKeyUp(KeyCode.A))
            {
                groundVelocity -= Vector3.right * HorizontalVelocity;
            }
        
            if (Input.GetKeyDown(KeyCode.D))
            {
                groundVelocity -= Vector3.right * HorizontalVelocity;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                groundVelocity += Vector3.right * HorizontalVelocity;
            }
        }
        
        if(!IsStart)
        {
            cam3.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis
                .m_InputAxisValue = 0.03f;
        }
    }


    public void ManualJump()
    {
        if (isGrounded)
        {
            Jump(true);
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
        isJustManualJump = manual;
        rigidbody.velocity = Vector3.up * JumpVelocity;
    }

    private void FixedUpdate()
    {
        if (!IsStart) return;
        rigidbody.velocity = new Vector3(-groundVelocity.x, rigidbody.velocity.y, currentForwardVelocity);
        if (rigidbody.velocity.y < 0)
        {
            
            Physics.gravity = Vector3.up * DefaultGravity * LowJumpMultiplier;
        }
        else
        {
            Physics.gravity = Vector3.up * DefaultGravity;
        }
    }

    public Vector3 PredictHighestPoint()
    {
        var jumpVel = new Vector3(0, JumpVelocity, currentForwardVelocity); 
        var v0 = jumpVel.magnitude;

        var angle = Mathf.Deg2Rad * Vector3.Angle(jumpVel, Vector3.forward);
        var maxZ = v0 * v0 * Mathf.Sin(2 * angle) / (-DefaultGravity * 2); 
        var maxY = (JumpVelocity * JumpVelocity) / (2 * -DefaultGravity);
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(0, maxY, maxZ) + pivotPoint.position;
        sphere.transform.localScale = Vector3.one * 0.1f;
        sphere.GetComponent<Collider>().enabled = false;
        return new Vector3(0, maxY, maxZ) + pivotPoint.position;
    }

    private void CheckIfGrounded()
    {
        IsGrounded = Physics.RaycastAll(transform.position, -Vector3.up, pivotPoint.transform.localPosition.magnitude + 0.1f)
            .Any(x => x.collider.CompareTag("Platform"));
    }

    private void OnGrounded(bool isGrounded)
    {
        if (isGrounded && IsStart)
        {
            cam1.SetActive(true);
            cam2.SetActive(false);
            rigidbody.velocity = Vector3.Scale(new Vector3(0.0f, 0.0f, 1.0f), rigidbody.velocity);

            if (!isJustManualJump)
            {
                var strength = Map(currentAirTime, 0.0f, MaxAirTime, 2.0f, 10.0f);
                var time = Map(currentAirTime, 0.0f, MaxAirTime, 0.2f, 0.35f);
                cam1.transform.DOShakePosition(time, strength);
                var vfx = Instantiate(landingVFX);
                vfx.transform.position = pivotPoint.position + Vector3.up * 0.1f;
                trailVFX.SetActive(false);
            }

            currentAirTime = 0.0f;
        }
        else
        {
            animator.CrossFade("Jumping", 0.25f, 0);
        }
        animator.SetBool("Jumping", !isGrounded);
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
            
            trailVFX.SetActive(true);
            Jump(false);
        }
        
        if (other.CompareTag(Obstacle) && !isDead)
        {
            OnDead();
            cam1.transform.DOShakePosition(0.5f, 4.0f);
            cam2.transform.DOShakePosition(0.5f, 4.0f);
        }

        if (other.CompareTag(Coin))
        {
            OnTakeCoin.Invoke();
        }
    }

    private void OnDead()
    {
        isDead = true;
        currentForwardVelocity = 0.0f;
        currentHorizontalVelocity = 0.0f;
        OnDeadEvent.Invoke();
    }
}
