using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class UniTaskExtension
{
    public static async UniTask ThrowWhenTimeOut(this UniTask task, float seconds = 3.0f)
    {
        await UniTask.WhenAny(task, UniTask.Delay(TimeSpan.FromSeconds(seconds)));
        if (task.Status != UniTaskStatus.Succeeded)
        {
            throw new TimeoutException($"Timeout when execute task");
        }
    }
    
    public static async UniTask<T> ThrowWhenTimeOut<T>(this UniTask<T> task, float seconds = 10.0f, CancellationToken token = default)
    {
        UniTaskScheduler.UnobservedExceptionWriteLogType = LogType.Exception;
        await UniTask.WhenAny(task, UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: token, ignoreTimeScale: true));
        if (task.Status == UniTaskStatus.Pending)
        {
            throw new TimeoutException($"Timeout when execute task");
        }
        else
        {
            return task.GetAwaiter().GetResult();
        }
    }
}
