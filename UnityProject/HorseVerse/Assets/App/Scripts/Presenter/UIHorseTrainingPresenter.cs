using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIHorseTrainingPresenter : IDisposable
{
    public UIHorseTraining uiHorseTraining = default;
    private CancellationTokenSource cts;
    public async UniTask ShowUIHorseTraningAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        uiHorseTraining ??= await UILoader.Load<UIHorseTraining>(token : cts.Token);
        uiHorseTraining.SetEntity(new UIHorseTraining.Entity()
        {
            horseDetail = 
        });
    }
}
