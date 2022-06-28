using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIDownloadProgressPresenter : IDisposable, IProgress<float>
{
    public event Action OnDownLoadDone = ActionUtility.EmptyAction.Instance;
    private UIDownloadAssetProgress uiDownloadAssetProgress;
    private CancellationTokenSource cts;
    public async UniTaskVoid PerformDownLoadAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiDownloadAssetProgress = await UILoader.Instantiate<UIDownloadAssetProgress>(token: cts.Token);
        var resourceLocators = await Addressables.InitializeAsync().ToUniTask();
        uiDownloadAssetProgress.SetEntity(new UIDownloadAssetProgress.Entity()
        {
            totalFiles = resourceLocators.Keys.Count(),
            currentFiles = 0,
            progressBar = new UIComponentProgressBar.Entity()
            {
                progress = 0
            }
        });
        await uiDownloadAssetProgress.In();
        var keys = resourceLocators.Keys.ToArray();
        for (int i = 0; i < keys.Length; i++)
        {
            await Addressables.DownloadDependenciesAsync(keys[i]).ToUniTask(this);
            uiDownloadAssetProgress.detailProgress.SetEntity(i + 1, keys.Length);
        }
        await UniTask.Delay(1000);
        OnDownLoadDone.Invoke();
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
        cts = default;
        UILoader.SafeRelease(ref uiDownloadAssetProgress);
    }

    public void Report(float value)
    {
        uiDownloadAssetProgress.entity.progressBar.progress = value == 0 ? 1 : value;
        uiDownloadAssetProgress.progressBar.SetEntity(uiDownloadAssetProgress.entity.progressBar);
    }
}
