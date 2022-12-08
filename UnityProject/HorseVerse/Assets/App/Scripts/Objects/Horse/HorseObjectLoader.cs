using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Object = UnityEngine.Object;

public class HorseObjectLoader : MonoBehaviour
{
    [Serializable]
    public class Entity
    {
        public string horse;
        public Color color1;
        public Color color2;
        public Color color3;
        public Color color4;
    }

    public Transform horseContainer;
    public Transform horsePosition;

    private CancellationTokenSource cts;
    private GameObject horse;
    private string oldHorse = string.Empty;

    public Entity entity { get; protected set; }
    public void SetEntity(Entity entity)
    {
        this.entity = entity;
        this.gameObject.SetActive(false);
        OnSetEntity();
    }

    protected void OnSetEntity()
    {
        LoadHorseAsync().Forget();
    }

    private void OnEnable()
    {
        SetHorseAnimationIfNeed();
    }

    private async UniTask LoadHorseAsync()
    {
        if (oldHorse != this.entity.horse)
        {
            HorseObjectLoaderExtension.safeCancelAndDispose(cts);
            cts = new CancellationTokenSource();

            DetachHorseContainer();
            RemoveOldHorse();
            await LoadNewHorse();
            SetHorseAnimationIfNeed();
        }
    }

    private void SetHorseAnimationIfNeed()
    {
        if (this.gameObject.activeInHierarchy)
        {
            SetHorseAnimation();
        }
    }

    private async UniTask LoadNewHorse()
    {
        oldHorse = this.entity.horse;
        horse = await HorseMeshAssetLoader.InstantiateHorse(this.entity.horse, this.entity.color1,
            this.entity.color2,
            this.entity.color3,
            this.entity.color4, cts.Token);

        horse.transform.parent = horsePosition;
        horse.transform.localScale = Vector3.one;
        horse.transform.localPosition = Vector3.zero;
        horse.transform.localRotation = Quaternion.identity;

        HorseObjectLoaderExtension.setLayerRecursively(horse, LayerMask.NameToLayer("Default"));
    }

    private void RemoveOldHorse()
    {
        if (horsePosition.transform.childCount > 0)
        {
            Destroy(horsePosition.transform.GetChild(0).gameObject);
            PrimitiveAssetLoader.UnloadAssetAtPath(oldHorse);
        }
    }


    private void DetachHorseContainer()
    {
        horseContainer.transform.parent = null;
        horseContainer.transform.localScale = Vector3.one;
    }

    private void SetHorseAnimation()
    {
        if (horse != null)
        {
            Animator animator = horse.GetComponent<Animator>();
            animator.applyRootMotion = false;
        }
    }

    private void OnDestroy()
    {
        HorseObjectLoaderExtension.safeCancelAndDispose(cts);
        cts = default;
        if (horse != null)
        {
            PrimitiveAssetLoader.UnloadAssetAtPath(oldHorse);
        }

        if (this.horseContainer != default)
        {
            Object.Destroy(this.horseContainer.gameObject);
        }
        Debug.Log("Destroy horse obj loader");
    }

    
}

public static class HorseObjectLoaderExtension
{
    public static void setLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    public static void safeCancelAndDispose(CancellationTokenSource cancellationTokenSource)
    {
        if (cancellationTokenSource?.IsCancellationRequested == false)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = default;
        }
    }
}
