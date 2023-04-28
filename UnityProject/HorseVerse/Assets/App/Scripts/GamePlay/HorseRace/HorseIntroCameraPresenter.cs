using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HorseIntroCameraPresenter : IDisposable
{
    private FreeCamera freeCamera;
    private WarmUpCamera warmUpCamera;
    private CancellationTokenSource cts;

    internal static async UniTask<HorseIntroCameraPresenter> InstantiateAsync(FreeCamera freeCameraPrefab, WarmUpCamera warmUpCamera, CancellationToken token)
    {
        var presenter = new HorseIntroCameraPresenter(freeCameraPrefab, warmUpCamera);
        await presenter.InitializeAsync(token);
        return presenter;
    }

    private async UniTask InitializeAsync(CancellationToken cancellationToken)
    {
        await UniTask.CompletedTask;
    }

    private HorseIntroCameraPresenter(FreeCamera freeCameraPrefab,
                                WarmUpCamera warmUpCameraPrefab)
    {
        freeCamera = UnityEngine.Object.Instantiate(freeCameraPrefab);
        warmUpCamera = UnityEngine.Object.Instantiate(warmUpCameraPrefab);
        freeCamera.gameObject.SetActive(false);
        warmUpCamera.gameObject.SetActive(false);
        cts = new CancellationTokenSource();
    }

    public async UniTask ShowFreeCamera()
    {
        freeCamera.gameObject.SetActive(true);
        var ucs = new UniTaskCompletionSource();

        void OnSkipFreeCamera()
        {
            freeCamera.OnSkipFreeCamera -= OnSkipFreeCamera;
            ucs.TrySetResult();
        }

        freeCamera.OnSkipFreeCamera += OnSkipFreeCamera;
        await ucs.Task.AttachExternalCancellation(cancellationToken: cts.Token);
    }

    public void HideFreeCamera()
    {
        freeCamera.gameObject.SetActive(false);
    }
    
    public void HideWarmUpCamera()
    {
        warmUpCamera.gameObject.SetActive(false);
    }
    
    public async UniTask ShowWarmUpCamera(Transform target)
    {
        warmUpCamera.SetTargetGroup(target);
        warmUpCamera.gameObject.SetActive(true);
        var ucs = new UniTaskCompletionSource();

        void OnFinishWarmingUp()
        {
            warmUpCamera.OnFinishWarmingUp -= OnFinishWarmingUp;
            ucs.TrySetResult();
        }

        warmUpCamera.OnFinishWarmingUp += OnFinishWarmingUp;
        await ucs.Task.AttachExternalCancellation(cts.Token);
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref cts);
        DisposeUtility.SafeDisposeComponent(ref freeCamera);
        DisposeUtility.SafeDisposeComponent(ref warmUpCamera);
    }
}