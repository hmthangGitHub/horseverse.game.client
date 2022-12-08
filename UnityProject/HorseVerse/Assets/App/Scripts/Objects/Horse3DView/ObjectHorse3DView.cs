using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHorse3DView : MonoBehaviour
{
    public class Entity
    {
        public HorseObjectLoader.Entity horseLoader;
    }

    public HorseObjectLoader horseLoader;

    public Entity entity { get; protected set; }
    public void SetEntity(Entity entity)
    {
        this.entity = entity;
        this.gameObject.SetActive(false);
        OnSetEntity();
    }

    protected void OnSetEntity()
    {
        horseLoader.SetEntity(this.entity.horseLoader);
    }

    public async UniTask In()
    {
        gameObject.SetActive(true);
        horseLoader.horseContainer.gameObject.SetActive(true);
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
