using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [Header("BlockProperties")]
    [SerializeField] private PredefinePath predefinePathPrefab;
    [SerializeField] private TrainingMapBlock trainingBlockPrefab;
    [SerializeField] private HorseTrainingController horseTrainingController;
    [SerializeField] private int preGenerateBlocks;
    [SerializeField] private int offsetBlocks;
    [SerializeField] private float spacingPerBlockWorldSpace;
    private int currentBlockIndex = -1;
    private int cachedPlayerBlockMoved = 0;
    private readonly Queue<TrainingMapBlock> generatedTrainingMapBlocksQueue = new Queue<TrainingMapBlock>();
    
    [Header("Obsolete")]
    [SerializeField] private TrainingCoin coinPrefab;
    [SerializeField] private TrainingObstacle trainingObstaclePrefab;

    public float groundOffset = 1.0f;
    public float jumpHeight = 2.0f;
    public AnimationCurve jumpAnimationCurve;
    
    public float spacingBetweenCoins = 0.1f;
    public float horizontalOffset = 2.0f;

    public Vector2 spacingBetweenBlocks;
    public Vector2Int numberOfBlockRange;
    public int totalNumberOfCoin = 500;

    [Header("Generated")]
    public int numberOfBlock;
    public float timeOffset;
    public float direction;
    
    private PredefinePath predefinePath;
    public PredefinePath PredefinePath => predefinePath ??= Object.Instantiate(predefinePathPrefab, this.transform);


    private void LateUpdate()
    { 
        var playerBlockMove = (int)(horseTrainingController.totalDistance / (trainingBlockPrefab.Size + spacingPerBlockWorldSpace));
        if (playerBlockMove != cachedPlayerBlockMoved)
        {
            cachedPlayerBlockMoved = playerBlockMove;
            GenerateBlocks();
            if (cachedPlayerBlockMoved > offsetBlocks + 3)
            {
                RemoveBlock();
            }
        }
    }

    private void RemoveBlock()
    {
        var block = generatedTrainingMapBlocksQueue.Dequeue();
        Object.Destroy(block.gameObject);
    }

    private void Awake()
    {
        //GenerateCoin(totalNumberOfCoin);
        GeneratePredefineBlock();
    }

    private void GeneratePredefineBlock()
    {
        for (int i = 0; i < preGenerateBlocks; i++)
        {
            GenerateBlocks();    
        }
    }

    private void GenerateBlocks()
    {
        currentBlockIndex++;
        var (position, rotation) = GetWorldSpaceTransformFromIndex(currentBlockIndex + offsetBlocks);
        var block = Object.Instantiate(trainingBlockPrefab, position, rotation, this.transform);
        block.PredefinePath = PredefinePath;
        generatedTrainingMapBlocksQueue.Enqueue(block);
    }

    private float GetTimeFromWorldSpaceDistance(float distance)
    {
        float t = distance / PredefinePath.SimplyPath.path.length;
        return PredefinePath.StartTime + t * PredefinePath.Direction;
    }

    private float GetTimeFromMapBlockIndex(int index)
    {
        return GetTimeFromWorldSpaceDistance(index * trainingBlockPrefab.Size + index * spacingPerBlockWorldSpace);
    }

    private (Vector3 position, Quaternion rotation) GetWorldSpaceTransformFromTime(float t)
    {
        var rotationAtDistance = predefinePath.SimplyPath.path.GetRotation(t);

        return (PredefinePath.SimplyPath.path.GetPointAtTime(t),
            Quaternion.Euler(0, rotationAtDistance.eulerAngles.y + 180 * direction, 0));
    }

    private (Vector3, Quaternion) GetWorldSpaceTransformFromIndex(int index)
    {
        return GetWorldSpaceTransformFromTime(GetTimeFromMapBlockIndex(index));
    }

    #region Obsolete

    public void GenerateCoin(int totalNumberOfCoin)
    {
        var startTime = PredefinePath.StartTime;
        timeOffset = spacingBetweenCoins / PredefinePath.SimplyPath.path.length;
        direction = PredefinePath.Direction;
        numberOfBlock = Random.Range(numberOfBlockRange.x, numberOfBlockRange.y + 1);
        var blockOfCoinWeights = CalculateBlockOfCoinWeights(numberOfBlock);

        var startingTime = startTime;
        blockOfCoinWeights.ForEach(x =>
        {
            var numberOfCoin = Mathf.RoundToInt(totalNumberOfCoin * x);
            GenerateCoinBlock(numberOfCoin ,startingTime);

            var delta = (numberOfCoin * timeOffset + Random.Range(spacingBetweenBlocks.x, spacingBetweenBlocks.y) /
                PredefinePath.SimplyPath.path.length) * direction;
            startingTime += delta;
        });
    }

    private void GenerateCoinBlock(int numberOfCoin, float startingTime)
    {
        var horizontalOffsetRandom = Random.Range(-1, 2) * horizontalOffset;
        var height = groundOffset;
        var isRequiredJumping = Random.Range(1, 5) == 1; //25%
        
        for (int i = 0; i < numberOfCoin; i++)
        {
            if (isRequiredJumping)
            {
                height = groundOffset + jumpAnimationCurve.Evaluate(Map(i, 0, numberOfCoin - 1, 0.15f , 0.85f)) * jumpHeight;
            }
            var t = startingTime + timeOffset * i * direction;
            var rotationAtDistance = predefinePath.SimplyPath.path.GetRotation(t);
            var right = Quaternion.Euler(0, rotationAtDistance.eulerAngles.y + 180 * direction, 0) * Vector3.right;
            var pos = PredefinePath.SimplyPath.path.GetPointAtTime(t) + Vector3.up * height + right * horizontalOffsetRandom;
            var trainingCoin = Instantiate(coinPrefab, this.transform);
            trainingCoin.transform.position = pos;
            trainingCoin.Set(0.1f * i);
        }
    }
    
    float Map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }

    private static float[] CalculateBlockOfCoinWeights(int numberOfBlock)
    {
        var blockOfCoinWeights = new float[numberOfBlock];
        for (int i = 0; i < blockOfCoinWeights.Length; i++)
        {
            blockOfCoinWeights[i] = Random.Range(1, 100);
        }

        var sum = blockOfCoinWeights.Sum();
        for (int i = 0; i < blockOfCoinWeights.Length; i++)
        {
            blockOfCoinWeights[i] /= sum;
        }

        return blockOfCoinWeights;
    }

    #endregion
}