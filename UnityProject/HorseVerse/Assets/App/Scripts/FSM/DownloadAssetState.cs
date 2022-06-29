using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DownloadAssetState : InjectedBState
{
    private UIDownloadProgressPresenter uIDownloadProgressPresenter;

    public override void Enter()
    {
        base.Enter();
        uIDownloadProgressPresenter = new UIDownloadProgressPresenter();
        uIDownloadProgressPresenter.OnDownLoadDone += OnDownLoadDone;
        uIDownloadProgressPresenter.PerformDownLoadAsync().Forget();
    }

    private void OnDownLoadDone()
    {
        this.Machine.ChangeState<InitialState>();
    }

    public override void Exit()
    {
        base.Exit();
        uIDownloadProgressPresenter.Dispose();
    }
}
