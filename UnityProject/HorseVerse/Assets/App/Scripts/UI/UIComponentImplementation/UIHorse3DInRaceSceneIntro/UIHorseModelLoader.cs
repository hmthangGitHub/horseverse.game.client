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
        if (oldHorse != this.entity.horse)
        {
            cts.SafeCancelAndDispose();
            cts = new CancellationTokenSource();
            horseIntro3DContainer.transform.parent = default;

            if (horsePosition.transform.childCount > 0)
            {
                Destroy(horsePosition.transform.GetChild(0).gameObject);
                PrimitiveAssetLoader.UnloadAssetAtPath(oldHorse);
            }
            var horsePrefab = await PrimitiveAssetLoader.LoadAssetAsync<GameObject>(this.entity.horse, cts.Token);
            oldHorse = this.entity.horse;
            horse = Instantiate<GameObject>(horsePrefab, Vector3.zero, Quaternion.identity);
            horse.transform.parent = horsePosition.transform;
            horse.transform.localPosition = Vector3.zero;
            horse.transform.localRotation = Quaternion.Euler(0, 90, 0);

            if (this.gameObject.activeInHierarchy)
            {
                AnimatateHorse();
            }
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