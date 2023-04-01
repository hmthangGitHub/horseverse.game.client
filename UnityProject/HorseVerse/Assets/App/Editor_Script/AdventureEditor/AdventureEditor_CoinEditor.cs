using BezierSolution;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdventureEditor_CoinEditor : MonoBehaviour
{
    public BezierSpline spline;
    [SerializeField] int coinNumber;
    public List<GameObject> listCoin = new List<GameObject>();
    public int CoinNumber => listCoin.Count;
    private GameObject coinHolder = default;

    public Vector3[] BenzierPointPositions => GetComponentsInChildren<BezierPoint>(true)
                                              .Select(x => x.localPosition)
                                              .ToArray();

    public void Init(int _coinNumber, Vector3[] positions)
    {
        spline ??= this.gameObject.AddComponent<BezierSpline>();
        if (coinHolder == default)
        {
            coinHolder = new GameObject();
            coinHolder.transform.parent = this.transform;
            coinHolder.transform.localPosition = Vector3.zero;
        }
        coinNumber = _coinNumber;
        OnChangeNumberOfCoin(coinNumber);
        InitBenzierPoints(positions);
        StartCoroutine(UpdateCoinPosition());
    }

    private IEnumerator UpdateCoinPosition()
    {
        yield return null;
        listCoin.ForEach((x, i) => { 
            x.transform.position = spline.GetPoint(Mathf.InverseLerp(0, listCoin.Count - 1, i));
        });
    }

    public void OnChangeNumberOfCoin(int number)
    {
        if (number < 0) return;
        if (listCoin.Count > number)
        {
            var destroyCoins = listCoin.Skip(number).Take(listCoin.Count - number).ToArray();
            foreach (var x in destroyCoins) Object.Destroy(x);
            listCoin = listCoin.Except(destroyCoins)
                               .ToList();
        }
        else if (listCoin.Count < number)
        {
            listCoin.AddRange(Enumerable.Range(0, number - listCoin.Count)
                                        .Select(x =>
                                        {
                                            var coin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                            coin.transform.parent = this.coinHolder.transform;
                                            return coin;
                                        }));
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
    }

    public void UpdatePosition()
    {
        if(coinNumber != CoinNumber)
        {
            OnChangeNumberOfCoin(coinNumber);
        }
        StartCoroutine(UpdateCoinPosition());
    }
}
