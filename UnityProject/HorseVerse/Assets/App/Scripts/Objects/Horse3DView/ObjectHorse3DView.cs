using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHorse3DView : ObjectComponent<ObjectHorse3DView.Entity>
{
    public class Entity
    {
        public HorseObjectLoader.Entity horseLoader;
    }

    public HorseObjectLoader horseLoader;

    protected override void OnSetEntity()
    {
        horseLoader.SetEntity(this.entity.horseLoader);
    }

    public async UniTask In(int type = 0)
    {
        gameObject.SetActive(true);
        horseLoader.horseContainer.gameObject.SetActive(true);
        horseLoader.SetBackgroundType(type);
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        await UniTask.Delay(200);
        await UniTask.CompletedTask;
    }

    public async UniTask Out()
    {
        gameObject.SetActive(false);
        horseLoader.horseContainer.gameObject.SetActive(false);
        await UniTask.Delay(200);
        await UniTask.CompletedTask;
    }

}
