using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIHorseModelLoader : UIComponent<UIHorseModelLoader.Entity>
{
    [Serializable]
    public class Entity
    {
        public string horse;
        public Color color1;
        public Color color2;
        public Color color3;
        public Color color4;
        public Vector3 position;
        public Quaternion rotation;
    }

    public GameObject horseIntro3DContainer;
    public GameObject horsePosition;
    private CancellationTokenSource cts;
    private GameObject horse;
    private string oldHorse = string.Empty;

    protected override void OnSetEntity()
    {
        this.transform.position = this.entity.position;
        this.transform.rotation = this.entity.rotation;
        LoadHorseAsync().Forget();
    }

    private void OnEnable()
    {
        AnimatateHorse() ;
    }

    private void AnimatateHorse()
    {
        SetHorseAnimation();
    }

    private async UniTask LoadHorseAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();
        horseIntro3DContainer.transform.parent = default;

        if (horsePosition.transform.childCount > 0)
        {
            Destroy(horsePosition.transform.GetChild(0)
                                 .gameObject);
            PrimitiveAssetLoader.UnloadAssetAtPath(oldHorse);
        }

        oldHorse = this.entity.horse;

        horse = await HorseMeshAssetLoader.InstantiateHorse(this.entity.horse, this.entity.color1,
            this.entity.color2, this.entity.color3, this.entity.color4, cts.Token);
        horse.transform.parent = horsePosition.transform;
        horse.transform.localPosition = Vector3.zero;
        horse.transform.localRotation = Quaternion.Euler(0, 90, 0);

        if (this.gameObject.activeInHierarchy)
        {
            AnimatateHorse();
        }
    }

    private void SetHorseAnimation()
    {
        if (horse != null)
        {
            Animator animator = horse.GetComponent<Animator>();
            if (animator != null)
            {
                animator.applyRootMotion = false;
                animator.SetFloat("Speed", 0.5f);
            }
            
        }
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        if (horse != null)
        {
            PrimitiveAssetLoader.UnloadAssetAtPath(oldHorse);
        }
        GameObject.Destroy(horseIntro3DContainer);
    }
}	