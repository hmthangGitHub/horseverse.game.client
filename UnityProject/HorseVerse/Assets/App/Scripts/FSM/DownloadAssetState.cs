using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DownloadAssetState : InjectedBState
{
    public override void Enter()
    {
        base.Enter();
        PerformDownLoadAsync().Forget();
    }

    public async UniTaskVoid PerformDownLoadAsync()
    {
        var resourceLocators = await Addressables.InitializeAsync().ToUniTask();
        await resourceLocators.Keys.Select(x => Addressables.DownloadDependenciesAsync(x).ToUniTask());

        this.Machine.ChangeState<InitialState>();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
