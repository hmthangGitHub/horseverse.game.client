using System;
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
    
    public static async UniTask<T> ThrowWhenTimeOut<T>(this UniTask<T> task, float seconds = 3.0f)
    {
        UniTaskScheduler.UnobservedExceptionWriteLogType = LogType.Exception;
        await UniTask.WhenAny(task, UniTask.Delay(TimeSpan.FromSeconds(seconds)));
        if (task.Status != UniTaskStatus.Succeeded)
        {
            throw new SystemException($"Timeout when execute task");
            throw new TimeoutException($"Timeout when execute task");
        }
        else
        {
            return task.GetAwaiter().GetResult();
        }
    }
}
