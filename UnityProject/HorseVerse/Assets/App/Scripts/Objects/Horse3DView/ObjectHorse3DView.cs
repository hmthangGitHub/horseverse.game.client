using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;

public class ObjectHorse3DView : ObjectComponent<ObjectHorse3DView.Entity>
{
    public class Entity
    {
        public HorseObjectLoader.Entity horseLoader;
        public int platformIndex = -1;
        public MainMenuCameraType.CameraType cameraType;
        public bool rotateEnable = true;
        public Action horseTouchAction;
    }

    public HorseObjectLoader horseLoader;
    public Transform platformContainer;
    public MainMenuCameraType mainMenuCameraType;
    public HorseRotator rotateTouch;
    private Vector3 originalHorsePos;
    private Tween horseMoveTween;
    private CancellationTokenSource cts;
    [SerializeField]
    private int animationOffset = 10;
    [SerializeField]
    private float duration = 0.25f;
    public LeanSelectableByFinger selectableByFinger;

    private void Start()
    {
        originalHorsePos = horseLoader.horsePosition.position;
    }

    protected override void OnSetEntity()
    {
        horseLoader.SetEntity(this.entity.horseLoader);
        SetPlatformIndex(this.entity.platformIndex);
        mainMenuCameraType.SetCameraType(this.entity.cameraType);
        SetRotateEnable(this.entity.rotateEnable);
        selectableByFinger.OnSelectedFingerUp.RemoveAllListeners();
        selectableByFinger.OnSelectedFingerUp.AddListener(_ => this.entity.horseTouchAction());
    }

    public void SetRotateEnable(bool enable)
    {
        rotateTouch.enabled = enable;
    }
    
    public void SetPlatformIndex(int platformIndex)
    {
        platformContainer.Cast<Transform>()
                         .ToList()
                         .ForEach((x, i) => x.gameObject.SetActive(i == platformIndex));
    }

    public async UniTask In(int type = 0)
    {
        gameObject.SetActive(true);
        horseLoader.horseContainer.gameObject.SetActive(true);
        horseLoader.SetBackgroundType(type);
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        await UniTask.Delay(200, cancellationToken: this.GetCancellationTokenOnDestroy());
    }

    public async UniTask Out()
    {
        gameObject.SetActive(false);
        horseLoader.horseContainer.gameObject.SetActive(false);
        await UniTask.Delay(200);
        await UniTask.CompletedTask;
    }

    public UniTask PlayHorizontalAnimation(float direction)
    {
        EndAnimation();
        horseMoveTween = DOTween.Sequence()
                                .Append(horseLoader.horsePosition.DOMoveFrom(originalHorsePos,
                                                       originalHorsePos + direction * Camera.main.transform.right * animationOffset, 
                                                       duration)
                                                   .SetEase(Ease.InBack));

        return horseMoveTween?.AwaitForComplete(TweenCancelBehaviour.CancelAwait, cancellationToken: cts.Token) ?? UniTask.CompletedTask;
    }
    
    public UniTask InFromOppositeDirectionAnimation(float direction)
    {
        EndAnimation();
        horseMoveTween = DOTween.Sequence()
                                .Append(horseLoader.horsePosition.DOMoveFrom(
                                                       originalHorsePos - direction * Camera.main.transform.right * animationOffset,
                                                       originalHorsePos,
                                                       0.5f)
                                .SetEase(Ease.OutBack));

        return horseMoveTween?.AwaitForComplete(TweenCancelBehaviour.CancelAwait, cancellationToken: cts.Token) ?? UniTask.CompletedTask;
    }

    private void EndAnimation()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        horseMoveTween?.Kill();
        horseMoveTween = default;
    }
    
    public void EnableHorseTouch(bool enable)
    {
        selectableByFinger.enabled = enable;
        selectableByFinger.OnSelectedFingerUp.RemoveAllListeners();
    }
}
