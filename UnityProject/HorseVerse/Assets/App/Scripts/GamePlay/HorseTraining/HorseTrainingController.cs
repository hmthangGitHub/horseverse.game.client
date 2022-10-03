using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HorseTrainingController : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public PredefinePath predefinePath;
    public HorseTrainingTrigger horseTrainingTrigger;
    public bool isStart = false;
    public float speed = 0;
    public float originalSpeed = 12;
    public float offset = 5;
    public float moveTime = 0.5f;
    public float regularJumpTime = 1.0f;
    public float jumpHeight = 1.5f;
    
    private new Transform transform;
    private Animator animator;
    private Transform horseMesh;
    
    public float totalDistance;
    public float startTime;
    public float direction;
    public float currentOffset;
    public float currentHeight;
    public int currentOffSetInt;

    private Tween currentMovingTween;
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Speed = Animator.StringToHash("Speed");

    public event Action OnTakeCoin = ActionUtility.EmptyAction.Instance;
    public event Action OnTouchObstacle = ActionUtility.EmptyAction.Instance;

    public enum Move
    {
        NONE,
        LEFT,
        RIGHT,
        JUMP,
    }

    public Move lastStrafeMove = Move.NONE;
    
    private void Awake()
    {
        SetData(this.mapGenerator.PredefinePath); // TODO remove
        animator = this.GetComponentInChildren<Animator>(true);
        horseMesh = animator.transform;
        horseTrainingTrigger.OnTouchObstacle += () =>
        {
            isStart = false;
            OnTouchObstacle.Invoke();
        };
        horseTrainingTrigger.OnTakeCoin += () => OnTakeCoin.Invoke();
    }

    public void SetData(PredefinePath predefinePath)
    {
        this.predefinePath = predefinePath;
        transform = base.transform;
        transform.position = predefinePath.StartPosition;
        transform.rotation = predefinePath.StartRotation;
        startTime = predefinePath.StartTime;
        direction = predefinePath.Direction;
    }

    public void StartGame()
    {
        speed = originalSpeed;
        isStart = true;
        animator.SetFloat(Speed, 1.0f);
    }

    public void JumpRight()
    {
        if (!ValidateBeforeMove(1)) return;
        UpdateOffSet();
        lastStrafeMove = Move.RIGHT;
    }
    
    public void Jump()
    {
        if (!ValidateBeforeMove(0)) return;
        UpdateHeight();
        UpdateJumpAnimation(true);
    }

    private void UpdateJumpAnimation(bool isJumping)
    {
        if (isJumping)
        {
            animator.SetBool(Jumping, true);
            animator.CrossFade(Jumping , 0.1f, 0);    
        }
        else
        {
            animator.SetBool(Jumping, false);
            animator.SetBool(Jumping, false);
        }
    }

    private void UpdateHeight()
    {
        currentMovingTween?.Kill();

        currentMovingTween = DOTween.To(val =>
        {
            currentHeight = Mathf.Lerp(0, jumpHeight, val);
        }, 0.0f, 1.0f, regularJumpTime)
        .SetEase(Ease.OutCubic)
        .SetLoops(2, loopType: LoopType.Yoyo)
        .OnComplete(() => UpdateJumpAnimation(false));
    }

    private bool ValidateBeforeMove(int move)
    {
        if (currentMovingTween == default || currentMovingTween.IsPlaying() == false)
        {
            currentOffSetInt += move;
            if (currentOffSetInt < -1 || currentOffSetInt > 1)
            {
                currentOffSetInt = Mathf.Clamp(currentOffSetInt, -1, 1);
                return false;
            }
            return true;
        }
        return false;
    }

    public void JumpLeft()
    {
        if (!ValidateBeforeMove(-1)) return;
        UpdateOffSet();
        lastStrafeMove = Move.LEFT;
    }

    private void UpdateOffSet()
    {
        var sourceOffset = currentOffset;
        var targetOffSet = currentOffSetInt * offset;
    
        currentMovingTween?.Kill();
        currentMovingTween = DOTween.To(val =>
        {
            currentOffset = Mathf.Lerp(sourceOffset, targetOffSet, val);
        }, 0.0f, 1.0f, moveTime).SetEase(Ease.InOutQuart);

        horseMesh.DOLocalRotate(new Vector3(0, 40.0f * Mathf.Sign(targetOffSet - sourceOffset), 0), moveTime * 0.5f)
            .SetEase(Ease.InBack)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                horseMesh.localRotation = Quaternion.identity;
            });
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            JumpLeft();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            JumpRight();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    public void FixedUpdate()
    {
        if (!isStart) return;
        var t = (totalDistance / predefinePath.SimplyPath.path.length) * direction + startTime;

        speed = GetDesiredSpeed(t);
        totalDistance += speed * Time.deltaTime;
        
        t = (totalDistance / predefinePath.SimplyPath.path.length) * direction + startTime;
        var rotationAtDistance = predefinePath.SimplyPath.path.GetRotation(t);
        transform.rotation = Quaternion.Euler(0, rotationAtDistance.eulerAngles.y + 180 * direction, 0);
        
        var right = transform.right;
        var pos = predefinePath.SimplyPath.path.GetPointAtTime(t);
        transform.position = pos + right * currentOffset + transform.up * currentHeight;
    }

    private float GetDesiredSpeed(float t)
    {
        return originalSpeed + (int)((Mathf.Abs(t - startTime)) / 1.0f) * originalSpeed * 0.15f;
    }
}
