using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathCreation;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinGenerator : MonoBehaviour
{
    [SerializeField] private TrainingCoin coinPrefab;
    [SerializeField] private PathCreator pathCreator;
    private PathCreator PathCreator => pathCreator ??= GetComponentsInParent<TrainingMapBlock>(true).First().PathCreator;
    private MeshPathContainer.PathType PathType => GetComponentsInParent<TrainingMapBlock>(true).First().PathType;
    [SerializeField] private float groundOffset;
    [SerializeField] private AnimationCurve jumpAnimationCurve;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float spacingBetweenCoins = 1.5f;
    
    private void OnEnable()
    {
        GenerateCoinBlock(Random.Range(3, 8));
    }

    private void GenerateCoinBlock(int numberOfCoin)
    {
        var height = groundOffset;
        var isRequiredJumping = PathType == MeshPathContainer.PathType.Cloud && Random.Range(1, 5) == 1; //25%
        var timeOffset = spacingBetweenCoins / PathCreator.path.length;
        
        for (int i = 0; i < numberOfCoin; i++)
        {
            if (isRequiredJumping)
            {
                height = groundOffset + jumpAnimationCurve.Evaluate(Map(i, 0, numberOfCoin - 1, 0.15f, 0.85f)) * jumpHeight;
            }
            
            var parent = this.transform.parent;
            var midLaneWorldPos = parent.parent.position;
            var t = PathCreator.path.GetClosestTimeOnPath(midLaneWorldPos) + i * timeOffset;
            var midLaneCoinPos = PathCreator.path.GetPointAtTime(t) + Vector3.up * height;
            
            var trainingCoin = Instantiate(coinPrefab, this.transform);
            trainingCoin.transform.position = midLaneCoinPos;
            trainingCoin.transform.localPosition += parent.localPosition;
            trainingCoin.Set(0.1f * i);
        }
    }
    
    float Map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }
}
