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
    public enum Mode
    {
        EditMode,
        Runtime
    }
    public CoinEditor.Mode mode;
    public UIComponentSplineEditorMode.Status status;
    public BezierSpline spline;
    public UIDebugLevelEditorSplineEditor uiDebugLevelEditorSplineEditor;
    public GameObject coinPrefab;
    public GameObject container;
    public List<GameObject> listCoin = new List<GameObject>();
    public int CoinNumber => listCoin.Count;
    public Vector3[] BenzierPointPositions => GetComponentsInChildren<BezierPoint>(true)
                                            .Select(x => x.localPosition)
                                            .ToArray();
    
    private void OnToggleStatus()
    {
        status = status == UIComponentSplineEditorMode.Status.Edit ? UIComponentSplineEditorMode.Status.Normal : UIComponentSplineEditorMode.Status.Edit;
        GetComponentsInChildren<BezierPoint>().ForEach(AddHandlerToBenzierPoint);
        gameObject.GetOrAddComponent<RuntimeTransformHandle>();
        gameObject.GetOrAddComponent<HandleEnableController>().enabled = status == UIComponentSplineEditorMode.Status.Edit;
        
        uiDebugLevelEditorSplineEditor.mode.SetEntity(status);
    }

    private void AddHandlerToBenzierPoint(BezierPoint x)
    {
        var runtimeTransformHandle = x.gameObject.GetOrAddComponent<RuntimeTransformHandle>();
        runtimeTransformHandle.enabled = status == UIComponentSplineEditorMode.Status.Edit;
    }

    public void Init(int coinNumber,
                     Vector3[] positions)
    {
        InitUIDebugIfNeed(coinNumber);
        InitBenzierPoints(positions);
        OnChangeNumberOfCoin(coinNumber);
    }

    private void InitUIDebugIfNeed(int coinNumber)
    {
        if (mode == Mode.EditMode)
        {
            uiDebugLevelEditorSplineEditor.SetEntity(new UIDebugLevelEditorSplineEditor.Entity()
            {
                mode = UIComponentSplineEditorMode.Status.Normal,
                addBtn = new ButtonComponent.Entity(AddNewBenzierPoint),
                cancelBtn = new ButtonComponent.Entity(OnToggleStatus),
                editBtn = new ButtonComponent.Entity(OnToggleStatus),
                removeBtn = new ButtonComponent.Entity(() =>
                {
                    if (spline.Count > 2)
                    {
                        spline.RemovePointAt(spline.Count - 1);
                    }
                }),
                coinNumber = new UIComponentInputField.Entity()
                {
                    defaultValue = coinNumber.ToString(),
                    onValueChange = val => OnChangeNumberOfCoin(Int32.Parse(val))
                }
            });

            uiDebugLevelEditorSplineEditor.In()
                                          .Forget();
            OnToggleStatus();
            OnToggleStatus();
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

    private void AddNewBenzierPoint()
    {
        var lastPoint = spline.GetComponentsInChildren<BezierPoint>()[spline.Count - 1];
        var pos = lastPoint.localPosition + lastPoint.transform.forward;
        var benzierPoint = spline.DuplicatePointAt(spline.Count - 1);
        benzierPoint.localPosition = pos;
        AddHandlerToBenzierPoint(benzierPoint);
    }
    
    private void OnChangeNumberOfCoin(int number)
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