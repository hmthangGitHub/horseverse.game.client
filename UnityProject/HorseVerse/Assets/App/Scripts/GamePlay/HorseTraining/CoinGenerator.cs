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
    // private PathCreator PathCreator => pathCreator ??= GetComponentsInParent<TrainingMapBlock>(true).First().PathCreator;
    private MeshPathContainer.PathType PathType => GetComponentsInParent<TrainingMapBlock>(true).First().PathType;
    [SerializeField] private float groundOffset;
    [SerializeField] private AnimationCurve jumpAnimationCurve;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float spacingBetweenCoins = 1.5f;
    
    public void GenerateCoinBlock(int numberOfCoin, bool isRequiredJumping)
    {
        var height = groundOffset;
        
        for (int i = 0; i < numberOfCoin; i++)
        {
            if (isRequiredJumping)
            {
                height = groundOffset + jumpAnimationCurve.Evaluate(Map(i, 0, numberOfCoin - 1, 0.15f, 0.85f)) * jumpHeight;
            }
            var trainingCoin = Instantiate(coinPrefab, this.transform);
            trainingCoin.transform.position = this.transform.position + Vector3.forward * ((i - numberOfCoin / 2) * spacingBetweenCoins) + Vector3.up * height;
            trainingCoin.Set(0.1f * i);
        }
    }
    
    float Map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
