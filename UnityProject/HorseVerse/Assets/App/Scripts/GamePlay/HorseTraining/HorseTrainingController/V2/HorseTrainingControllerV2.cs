using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using BezierSolution;
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
    private const string TrapTrigger = "TrainingTrapTrigger";
    private const string TrapObject = "TrainingTrap";

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
    [SerializeField] private GameObject cam4; //Use for scene change
    [SerializeField] private Transform horsePosition;
    [SerializeField] private Vector3 groundVelocity;
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private Transform coinBenzierEndPoint;
    [SerializeField] private BezierSpline benzierSpline;
    [SerializeField] private LayerMask platformLayer;

    [Space, Header("INPUT SETTINGS")]
    [SerializeField, Range(0, 1)] private float delayTimeForTouch = 0.05f;
    [SerializeField] private float clampOffsetY = 25;

    private bool isStart;
    private bool isDead;
    private bool isGrounded = true;
    private bool isJumping;
    private bool isTurning = false;
    private bool isChangeScene = false;
    private bool isPerformingChangeScene = false;
    private bool isChangeScened = false;

    public bool IsLanding => isGrounded;
    public bool IsJumping => isJumping;
    public bool IsDead => isDead;
    public bool IsPerformingChangeScene => isPerformingChangeScene;

    private Animator animator;
    private Animator Animator => animator ??= GetComponentInChildren<Animator>();
    private float currentAirTime;
    public ScrambleField<float> TotalRunTimeEncrypt { get; } = new ScrambleField<float>();
    public ScrambleField<int> TotalCoinEncrypt { get; } = new ScrambleField<int>();
    private float MaxAirTime => Mathf.Abs(JumpVelocity / DefaultGravity) + masterHorseTrainingProperty.FallAirTimeMax;
    public Vector2 AirTime => new Vector2(masterHorseTrainingProperty.FallAirTimeMin, masterHorseTrainingProperty.FallAirTimeMax);
    public event Action OnTakeCoin = ActionUtility.EmptyAction.Instance;
    public event Action OnUpdateRunTime = ActionUtility.EmptyAction.Instance;
    public event Action OnDeadEvent = ActionUtility.EmptyAction.Instance;

    public float JumpVelocity => masterHorseTrainingProperty.JumpVelocity;
    public float HorizontalVelocity => masterHorseTrainingProperty.HorizontalVelocity;

    [SerializeField]
    public float ForwardVelocity { get; private set; }

    private BlockObjectData currentBlock;
    private readonly Dictionary<CameraType, GameObject> cameraTypes = new Dictionary<CameraType, GameObject>();

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

    private enum CameraType
    {
        Run,
        Jump,
        Start,
        ChangeScene
    }
    
    private LastTap lastTap;
    private CinemachineOrbitalTransposer startCameraOrbitalTransposer;
    private CinemachineOrbitalTransposer changeSceneCameraOrbitalTransposer;
    private float animationHorizontal;
    private static readonly int VerticalVelocity = Animator.StringToHash("VerticalVelocity");
    private int horizontalDirection = 0;
    private int currentDifficulty = 0;

    private Vector3 v_direction = Vector3.forward;
    public Vector3 Direction => v_direction;
    private Vector3 v_side = Vector3.right;


    private bool IsGrounded
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
            if (isStart == value) return;
            isStart = value;
            OnStart(value);
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

    private void Start()
    {
        cameraTypes.Add(CameraType.Run, cam1);
        cameraTypes.Add(CameraType.Jump, cam2);
        cameraTypes.Add(CameraType.Start, cam3);
        cameraTypes.Add(CameraType.ChangeScene, cam4);
        
        startCameraOrbitalTransposer ??= GetCamera(CameraType.Start).GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineOrbitalTransposer>();
        changeSceneCameraOrbitalTransposer ??= GetCamera(CameraType.ChangeScene).GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineOrbitalTransposer>();
        
        SetActiveCamera(CameraType.Start);
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
        SetCameraYaw(masterHorseTrainingProperty.RunCameraRotation, GetCamera(CameraType.Run).transform);
        SetCameraYaw(masterHorseTrainingProperty.FallCameraRotation, GetCamera(CameraType.Jump).transform);
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
            SetActiveCamera(CameraType.Run);
            v_direction = this.rigidbody.transform.forward;
            DOTween.To(val =>
            {
                Animator.SetFloat(Speed, val);
                currentHorizontalVelocity = Mathf.Lerp(0.0f, HorizontalVelocity, val);
                UpdateCoinPoint();
            }, 0.0f, 1.0f, 2.0f).SetEase(Ease.Linear);
            AddInputEvents();
        }
    }

    private void Update()
    {
        if (IsStart && !IsDead )
        {
            if (!IsPerformingChangeScene)
            {
                CheckIfGrounded();
                CheckIfFall();
                ControlHorse();
                UpdateDirection();
                UpdateHorizontalAnimation();
                UpdateDifficulty();
                UpdateForwardVelocity();
            }
            UpdateJumpAnimation();
        }
        
        if(!IsStart)
        {
            startCameraOrbitalTransposer ??= GetCamera(CameraType.Start).GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineOrbitalTransposer>();
            startCameraOrbitalTransposer.m_XAxis.m_InputAxisValue = 0.03f;
        }

        if(IsPerformingChangeScene)
        {
            changeSceneCameraOrbitalTransposer.m_XAxis.m_InputAxisValue = 0.5f;
        }
    }

    private void UpdateCoinPoint()
    {
        coinBenzierEndPoint.position = GetRelativePoint();
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
        return hitPoint;
    }

    public Vector3 GetCoinAnimationPoint(float t)
    {
        return benzierSpline.GetPoint(t);
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

    private void UpdateDirection()
    {
        CheckIfCurve();

        isTurning = false;
        if (currentBlock != default && currentBlock.Curve != default)
        {
            if (currentBlock.BlockType == TYPE_OF_BLOCK.TURN_LEFT || currentBlock.BlockType == TYPE_OF_BLOCK.TURN_RIGHT)
            {
                if ((currentBlock.BlockType == TYPE_OF_BLOCK.TURN_LEFT && horizontalDirection > 0)
                    || (currentBlock.BlockType == TYPE_OF_BLOCK.TURN_RIGHT && horizontalDirection < 0))
                {
                    bool isEnd = false;
                    if (!isEnd)
                    {
                        Vector3 dir;
                        Vector3 localPoint = currentBlock.Curve.transform.InverseTransformPoint(this.rigidbody.transform.position);
                        var tangenPoint = currentBlock.Curve.findTheClosedPoint(localPoint, out dir, out isEnd);
                        bool isNeedTurn = !CheckIfIsRunForward(dir);
                        if (!isEnd && isNeedTurn)
                        {
                            v_direction = currentBlock.Curve.transform.TransformVector(dir).normalized;
                            isTurning = true;
                        }
                        else
                            v_direction = FixDirectionAfterEndCurve(v_direction);
                    }
                }
            }
        }
        this.rigidbody.transform.forward = Vector3.Lerp(this.rigidbody.transform.forward, v_direction, Time.deltaTime * 15.0f);
    }

    private Vector3 FixDirectionAfterEndCurve (Vector3 vec)
    {
        Vector3 v = vec;
        v.x = Mathf.Round(vec.x);
        v.y = Mathf.Round(vec.y);
        v.z = Mathf.Round(vec.z);
        return v;
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
        if (isGrounded) return;
        if (isChangeScened) return;
        
        currentAirTime += Time.deltaTime;
        if (currentAirTime > MaxAirTime)
        {
            OnDead();
            foreach (var camera in cameraTypes)
            {
                camera.Value.transform.parent = default;
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
        if (isPerformingChangeScene) return;

        rigidbody.velocity = GetSurfaceVelocity();
        if (rigidbody.velocity.y < 0)
        {
            
            Physics.gravity = Vector3.up * DefaultGravity * LowJumpMultiplier;
        }
        else
        {
            Physics.gravity = Vector3.up * DefaultGravity;
        }
    }

    private Vector3 GetSurfaceVelocity()
    {
        Vector3 fwd = v_direction * currentForwardVelocity;
        v_side = -Vector3.Cross(v_direction, Vector3.up);
        Vector3 right = isTurning ? Vector3.zero : v_side * (-horizontalDirection * currentHorizontalVelocity);
        fwd.y = 0;
        right.y = 0;
        Vector3 dir = fwd + right;
        dir.y = rigidbody.velocity.y;
        //return new Vector3(-horizontalDirection * currentHorizontalVelocity, rigidbody.velocity.y, currentForwardVelocity);
        return dir;
    }

    private void CheckIfGrounded()
    {
        Debug.DrawLine(pivotPoint.position, pivotPoint.position + -Vector3.up * 0.15f, Color.red);
        IsGrounded = Physics.RaycastAll(pivotPoint.position, -Vector3.up, 0.15f)
            .Any(x => x.collider.CompareTag("Platform"));
    }

    private void CheckIfCurve()
    {
        var target = Physics.RaycastAll(pivotPoint.position, -Vector3.up, 3.15f, platformLayer.value)
            .Where(x => x.collider.CompareTag("Platform"));
        if (target != null)
        {
            var i = target.FirstOrDefault();
            try
            {
                var data = i.collider.gameObject.GetComponentInParent<BlockObjectData>();
                currentBlock = data;
            }
            catch
            {
                if (currentBlock != default)
                {
                    currentBlock = default;
                    v_direction = FixDirectionAfterEndCurve(v_direction);
                }
            }
        }
        else
        {
            if (currentBlock != default)
            {
                currentBlock = default;
                v_direction = FixDirectionAfterEndCurve(v_direction);
            }
        }
    }

    private void OnGrounded(bool isGrounded)
    {
        if (isGrounded && IsStart)
        {
            rigidbody.velocity = Vector3.Scale(new Vector3(0.0f, 0.0f, 1.0f), rigidbody.velocity);
            var minAirTimeToShake = Mathf.Abs(JumpVelocity / DefaultGravity) * 2 + 0.1f;
            
            if (currentAirTime > minAirTimeToShake)
            {
                SetActiveCamera(CameraType.Run);
                var strength = Map(currentAirTime, minAirTimeToShake, MaxAirTime, 2.0f, 5.0f);
                var time = Map(currentAirTime, minAirTimeToShake, MaxAirTime, 0.1f, 0.35f);
                GetCamera(CameraType.Run).transform.DOShakePosition(time, strength, 20);
                var vfx = Instantiate(landingVFX);
                vfx.transform.position = pivotPoint.position + Vector3.up * 0.1f;
                trailVFX.SetActive(false);
                AudioManager.Instance.PlaySound(AudioManager.HorseLand);
                AudioManager.Instance.PlaySoundHasLoop(AudioManager.HorseRunTraining);
                isChangeScened = false;
            }
            currentAirTime = 0.0f;
        }

        if (isGrounded && isJumping)
        {
            v_direction = FixDirectionAfterEndCurve(v_direction);
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

        if (other.CompareTag(Bridge) && other.TryGetComponent<PlatformTrigger>(out var trigger))
        {
            SetActiveCamera(CameraType.Jump);
        }
        
        if (other.CompareTag(Obstacle) && !isDead)
        {
            OnDead();
            GetCamera(CameraType.Run).transform.DOShakePosition(0.35f, 2.5f);
            GetCamera(CameraType.Jump).transform.DOShakePosition(0.35f, 2.5f);
        }

        if (other.CompareTag(Coin))
        {
            OnTakeCoin.Invoke();
            TotalCoinEncrypt.Value++;
            takeCoinVFX.gameObject.SetActive(false);
            takeCoinVFX.gameObject.SetActive(true);
            SoundController.PlayHitCoin();
        }

        if (other.CompareTag(TrapObject) && !isDead)
        {
            OnDead();
            GetCamera(CameraType.Run).transform.DOShakePosition(0.35f, 2.5f);
            GetCamera(CameraType.Jump).transform.DOShakePosition(0.35f, 2.5f);
        }

        if (other.CompareTag(TrapTrigger))
        {
            var comp = other.GetComponentInParent<TrainingTrap>();
            if(comp)
            {
                comp.Active();
            }
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

    public void OnJumpOutPlatform()
    {
        if(currentBlock != default)
        {
            currentBlock = default;
            v_direction = FixDirectionAfterEndCurve(v_direction);
        }
    }

    public async UniTask PerformHighJumpToChangeSceneAsync()
    {
        isPerformingChangeScene = true;
        await AutoJumpAsync();
        await PreviewHorseInAirAsync();
    }

    private async Task PreviewHorseInAirAsync()
    {
        SetActiveCamera(CameraType.ChangeScene);
        var changeSceneInAirDuration = 1.5f;
        var t = 0f;

        var velocity = rigidbody.velocity;
        while (t <= changeSceneInAirDuration)
        {
            velocity.y = Mathf.Lerp(JumpVelocity, 0.0f, t / changeSceneInAirDuration);
            rigidbody.velocity = velocity;
            t += Time.deltaTime;
            await UniTask.Yield(cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }

    private async UniTask AutoJumpAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: this.GetCancellationTokenOnDestroy());
        ManualJump();
        horizontalDirection = 0;
        rigidbody.velocity = Vector3.up * JumpVelocity + GetSurfaceVelocity();
        IsGrounded = false;
        currentAirTime = 0;
        rigidbody.useGravity = false;
        isJumping = true;
        animator.SetBool(Jumping, isJumping);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: this.GetCancellationTokenOnDestroy());
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void LandToNewScene()
    {
        isPerformingChangeScene = false;
        rigidbody.useGravity = true;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        SetActiveCamera(CameraType.Jump);
    }

    private void SetActiveCamera(CameraType cameraType)
    {
        foreach (var camera in cameraTypes)
        {
            camera.Value.SetActive(camera.Key == cameraType);
        }
    }

    private GameObject GetCamera(CameraType cameraType)
    {
        return cameraTypes.First(x => x.Key == cameraType).Value;
    }

    private bool CheckIfIsRunForward(Vector3 direction)
    {
        var dir = direction.normalized;
        float A1 = Vector3.Dot(dir, Vector3.forward);
        float A2 = Vector3.Dot(dir, Vector3.right);
        if (Mathf.Abs(A1) >= 0.985f || Mathf.Abs(A2) >= 0.985f) return true;
        return false;
    }

    public void SetDirection(Vector3 dir)
    {
        v_direction = dir;
    }

    public void EnableRotateToWard()
    {
        StartCoroutine(doRotateToward());
    }

    IEnumerator doRotateToward()
    {
        yield return this.transform.DORotate(v_direction, 1.0f);
    }
}
