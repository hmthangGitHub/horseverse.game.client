using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class HorseLoader : UIComponent<HorseLoader.Entity>
{
    [Serializable]
    public class Entity
    {
        public string horse;
    }

    public Transform horseContainer;
    public Transform horsePosition;

    private CancellationTokenSource cts;
    private GameObject horse;
    private string oldHorse = string.Empty;

    protected override void OnSetEntity()
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
            cts.SafeCancelAndDispose();
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
        var horsePrefab = await PrimitiveAssetLoader.LoadAssetAsync<GameObject>(this.entity.horse, cts.Token);
        oldHorse = this.entity.horse;

        horse = Instantiate<GameObject>(horsePrefab, Vector3.zero, Quaternion.identity, horsePosition.transform);
        horse.transform.localScale = Vector3.one;
        horse.transform.localPosition = Vector3.zero;

        SetLayerRecursively(horse, LayerMask.NameToLayer("RenderTexture"));
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
            animator.SetFloat("Speed", UnityEngine.Random.Range(0.0f, 1.0f));
        }
    }

    public static void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    private void OnDestroy()
    {
        cts.SafeCancelAndDispose();
        if (horse != null)
        {
            PrimitiveAssetLoader.UnloadAssetAtPath(oldHorse);
        }

        if (this.horseContainer != default)
        {
            GameObject.Destroy(this.horseContainer.gameObject);    
        }
    }
}
