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
    public Vector3[] BenzierPointPositions => GetComponentsInChildren<BezierPoint>(true)
                                            .Select(x => x.localPosition)
                                            .ToArray();
    
    public void OnToggleStatus()
    {
        status = status == UIComponentSplineEditorMode.Status.Edit ? UIComponentSplineEditorMode.Status.Normal : UIComponentSplineEditorMode.Status.Edit;
        GetComponentsInChildren<BezierPoint>().ForEach(AddHandlerToBenzierPoint);
        gameObject.GetOrAddComponent<RuntimeTransformHandle>();
        gameObject.GetOrAddComponent<HandleEnableController>().enabled = status == UIComponentSplineEditorMode.Status.Edit;
    }

    private void AddHandlerToBenzierPoint(BezierPoint x)
    {
        var runtimeTransformHandle = x.gameObject.GetOrAddComponent<RuntimeTransformHandle>();
        runtimeTransformHandle.enabled = status == UIComponentSplineEditorMode.Status.Edit;
    }

    public void Init(int coinNumber,
                     Vector3[] positions)
    {
        OnChangeNumberOfCoin(coinNumber);
        InitBenzierPoints(positions);
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
            destroyCoins.ForEach(x => Object.Destroy(x));
            listCoin = listCoin.Except(destroyCoins)
                               .ToList();
        }
        else if (listCoin.Count < number)
        {
            listCoin.AddRange(Enumerable.Range(0, number - listCoin.Count)  
                      .Select(x => Instantiate(coinPrefab, container.transform)));
        }
    }

    private void Update()
    {
        UpdateCoinPosition();
    }
    
    private void UpdateCoinPosition()
    {
        listCoin.ForEach((x,
                          i) => x.transform.position = spline.GetPoint(Mathf.InverseLerp(0, listCoin.Count - 1, i)));
    }

#if UNITY_EDITOR
    [SerializeField]
    private int numberOfCoin = 0;
    [SerializeField]
    private Transform[] initPoints;
    
    [ContextMenu("Init Test")]
    private void InitTest()
    {
        Init(numberOfCoin, initPoints.Select(x => x.position).ToArray());
    }
#endif
}