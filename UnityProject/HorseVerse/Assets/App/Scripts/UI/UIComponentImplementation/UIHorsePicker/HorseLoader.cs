using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class HorseLoader : UIComponent<HorseLoader.Entity>
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
        else
        {
            UpdateColor();
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
        horse = await HorseMeshAssetLoader.InstantiateHorse(this.entity.horse,  cts.Token);

        horse.transform.parent = horsePosition;
        horse.transform.localScale = Vector3.one;
        horse.transform.localPosition = Vector3.zero;
        horse.transform.localRotation = Quaternion.identity;
        
        SetLayerRecursively(horse, LayerMask.NameToLayer("RenderTexture"));
    }

    private void RemoveOldHorse()
    {
        if (horse != default && !string.IsNullOrEmpty(oldHorse))
        {
            Destroy(horse);
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
            Animator animator = horse.GetComponentInChildren<Animator>();
            animator.applyRootMotion = false;
        }
    }

    private void UpdateColor()
    {
        if (horse != null)
        {
            horse.GetComponentInChildren<HorseObjectData>().SetColor(this.entity.color1,
            this.entity.color2,
            this.entity.color3,
            this.entity.color4);
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
        cts = default;
        if (horse != null)
        {
            PrimitiveAssetLoader.UnloadAssetAtPath(oldHorse);
        }

        if (this.horseContainer != default)
        {
            Object.Destroy(this.horseContainer.gameObject);    
        }
    }
}
