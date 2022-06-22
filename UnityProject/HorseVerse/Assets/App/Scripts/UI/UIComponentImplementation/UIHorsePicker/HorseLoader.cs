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

    public GameObject horsePosition;
    private CancellationTokenSource cts;
    public new CinemachineVirtualCamera camera;
    private GameObject horse;

    protected override void OnSetEntity()
    {
        LoadHorseAsync().Forget();
    }

    private void OnEnable()
    {
        AnimatateHorse().Forget();
    }

    private async UniTask AnimatateHorse()
    {
        SetHorseAnimation();
        await UniTask.DelayFrame(2);
        AnimateCamera();
    }

    private async UniTask LoadHorseAsync()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        if (horsePosition.transform.childCount > 0)
        {
            Destroy(horsePosition.transform.GetChild(0).gameObject);
        }
        var horsePrefab = await Resources.LoadAsync<GameObject>(this.entity.horse) as GameObject;
        horse = Instantiate<GameObject>(horsePrefab, Vector3.zero, Quaternion.identity, horsePosition.transform);
        horse.transform.localScale = Vector3.one;
        horse.transform.localPosition = Vector3.zero;
        SetLayerRecursively(horse, LayerMask.NameToLayer("UI"));
        if (this.gameObject.activeInHierarchy)
        {
            AnimatateHorse().Forget();
        }
    }

    private void AnimateCamera()
    {
        camera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.m_InputAxisValue = 0.1f;
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
        cts?.Cancel();
    }
}
