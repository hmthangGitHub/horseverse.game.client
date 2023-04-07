using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BezierSolution;
using Cysharp.Threading.Tasks;
using RuntimeHandle;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class CoinEditor : MonoBehaviour
{
    public UIComponentSplineEditorMode.Status status;
    public BezierSpline spline;
    public GameObject coinPrefab;
    public GameObject container;
    public List<GameObject> listCoin = new List<GameObject>();
    public int CoinNumber => listCoin.Count;
    public ParentPositionController parentPositionControllerPrefab;
    private float coinRadius;
    public Vector3[] BenzierPointPositions => GetComponentsInChildren<BezierPoint>(true)
                                              .Select(x => x.localPosition)
                                              .ToArray();
    private PlatformGeneratorPool pool;
    private bool isEditing;
    private int initDelayFrame = 3;

    public void OnToggleStatus()
    {
        status = status == UIComponentSplineEditorMode.Status.Edit ? UIComponentSplineEditorMode.Status.Normal : UIComponentSplineEditorMode.Status.Edit;
        GetComponentsInChildren<BezierPoint>().ForEach(AddHandlerToBenzierPoint);
        gameObject.GetOrAddComponent<RuntimeTransformHandle>();
        AddPositionControllerIfNeed(this.transform);
        gameObject.GetOrAddComponent<HandleEnableController>().enabled = status == UIComponentSplineEditorMode.Status.Edit;
    }

    private void AddPositionControllerIfNeed(Transform parent)
    {
        if (parent.Cast<Transform>()
                  .All(x => x.GetComponent<ParentPositionController>() == default))
        {
            Instantiate(parentPositionControllerPrefab, parent);
        }
    }

    private void AddHandlerToBenzierPoint(BezierPoint x)
    {
        var runtimeTransformHandle = x.gameObject.GetOrAddComponent<RuntimeTransformHandle>();
        runtimeTransformHandle.enabled = status == UIComponentSplineEditorMode.Status.Edit;

        if (status == UIComponentSplineEditorMode.Status.Edit)
        {
            AddPositionControllerIfNeed(x.transform);
        }
    }

    public void Init(int coinNumber,
                     Vector3[] positions,
                     float coinRadius,
                     PlatformGeneratorPool _pool,
                     bool isEditing = true)
    {
        this.coinRadius = coinRadius;
        this.pool = _pool;
        OnChangeNumberOfCoin(coinNumber);
        InitBenzierPoints(positions);
        this.isEditing = isEditing;
        if (!this.isEditing)
        {
            UpdateCoinPositionAsync().Forget();
        }
    }

    public void Clear()
    {
        if (listCoin.Count > 0)
        {
            var destroyCoins = listCoin.ToArray();
            destroyCoins.ForEach(x => { if (this.pool != default) this.pool.AddToPool(coinPrefab.name, x); else Object.Destroy(x); });
            listCoin.Clear();
        }
        this.pool = default;
    }

    public void RemoveLastBenzierPoint()
    {
        if (spline.Count > 2)
        {
            spline.RemovePointAt(spline.Count - 1);
        }
    }

    private void InitBenzierPoints(Vector3[] positions)
    {
        for (int i = 0; i < positions.Length - 2; i++)
        {
            AddNewBenzierPoint();
        }
        GetComponentsInChildren<BezierPoint>().ForEach((x, i) =>
        {
            if (positions.Length - 1 >= i)
            {
                x.localPosition = positions[i];
            }
        });
    }

    public void AddNewBenzierPoint()
    {
        var lastPoint = spline.GetComponentsInChildren<BezierPoint>()[spline.Count - 1];
        var pos = lastPoint.localPosition + lastPoint.transform.forward;
        var benzierPoint = spline.DuplicatePointAt(spline.Count - 1);
        benzierPoint.localPosition = pos;
        AddHandlerToBenzierPoint(benzierPoint);
    }
    
    public void OnChangeNumberOfCoin(int number)
    {
        if(number < 0) return;
        if (listCoin.Count > number)
        {
            var destroyCoins = listCoin.Skip(number).Take(listCoin.Count - number).ToArray();
            //destroyCoins.ForEach(x => { if (this.pool != default) this.pool.AddToPool(coinPrefab.name, x); else Object.Destroy(x); });
            foreach (var x in destroyCoins) Object.Destroy(x);
            listCoin = listCoin.Except(destroyCoins)
                               .ToList();
        }
        else if (listCoin.Count < number)
        {
            listCoin.AddRange(Enumerable.Range(0, number - listCoin.Count)
                                        .Select(x =>
                                        {
                                            var coin = Instantiate(coinPrefab, container.transform);
                                            //var coin = this.pool != default ? (GameObject)this.pool.GetOrInstante(coinPrefab, container.transform) : Instantiate(coinPrefab, container.transform);
                                            //var coinTraining = coin.GetComponentInChildren<TrainingCoin>();
                                            //if (coinTraining != default) coinTraining.onDestroy = () => { if (this.pool != default) this.pool.AddToPool(coinPrefab.name, coin); };
                                            coin.GetComponentInChildren<SphereCollider>(true).radius = coinRadius;
                                            return coin;
                                        }));
        }
    }

    private void Update()
    {
        if (!isEditing) return;
        UpdateCoinPosition();
    }

    private async UniTaskVoid UpdateCoinPositionAsync()
    {
        await UniTask.DelayFrame(2, cancellationToken: this.GetCancellationTokenOnDestroy());
        UpdateCoinPosition();
    }
    
    public void UpdateCoinPosition()
    {
        listCoin.ForEach((x,
                          i) => x.transform.position = spline.GetPoint(Mathf.InverseLerp(0, listCoin.Count - 1, i)));
    }

#if UNITY_EDITOR
    [SerializeField]
    private int numberOfCoinTest = 0;
    [SerializeField]
    private Transform[] initPointsTest;
    [SerializeField]
    private float coinRadiusTest;

    [ContextMenu("Init Test")]
    private void InitTest()
    {
        Init(numberOfCoinTest, initPointsTest.Select(x => x.position).ToArray(), coinRadiusTest, null);
    }
#endif
}