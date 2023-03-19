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
    }

    public GameObject horseIntro3DContainer;
    public GameObject horsePosition;
    private CancellationTokenSource cts;
    private GameObject horse;
    private string oldHorse = string.Empty;
    private static readonly int Speed = Animator.StringToHash("Speed");

    protected override void OnSetEntity()
    {
        
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
        horse.transform.localScale = Vector3.one;

        if (this.gameObject.activeInHierarchy)
        {
            AnimatateHorse();
        }
    }

    public void SetTransform(Vector3 position, Quaternion rotation)
    {
        horseIntro3DContainer.transform.parent = default;
        horseIntro3DContainer.transform.position = position;
        horseIntro3DContainer.transform.rotation = rotation;
        horseIntro3DContainer.transform.localScale = Vector3.one;
    }

    private void SetHorseAnimation()
    {
        if (horse != null)
        {
            var animator = horse.GetComponentInChildren<Animator>();
                animator.applyRootMotion = false;
                animator.Play("Running", 0, 0.0f);
                animator.SetFloat(Speed, UnityEngine.Random.Range(0.2f, 0.27f));
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