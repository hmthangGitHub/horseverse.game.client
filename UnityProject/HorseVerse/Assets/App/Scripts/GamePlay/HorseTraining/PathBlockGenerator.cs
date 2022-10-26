using System;
using System.Collections.Generic;
using System.Linq;
using PathCreation;
using PathCreation.Examples;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class PathBlockGenerator : IDisposable
{
    private readonly Queue<TrainingMapBlock> generatedTrainingMapBlocksQueue = new Queue<TrainingMapBlock>();
    private int currentBlockIndex = -1;
    private int cachedPlayerBlockMoved;
    
    private readonly TrainingMapBlock trainingBlockPrefab;
    private readonly PathCreator pathCreator;
    private readonly int preGenerateBlocks;
    private readonly int offsetBlocks;
    private readonly float spacingPerBlockWorldSpace;
    private readonly Transform blockContainer;
    
    public PathBlockGenerator(PathCreator pathCreator, int preGenerateBlocks, TrainingMapBlock trainingBlockPrefab, int offsetBlocks, float spacingPerBlockWorldSpace, Transform blockContainer)
    {
        this.pathCreator = pathCreator;
        this.preGenerateBlocks = preGenerateBlocks;
        this.trainingBlockPrefab = trainingBlockPrefab;
        this.offsetBlocks = offsetBlocks;
        this.spacingPerBlockWorldSpace = spacingPerBlockWorldSpace;
        this.blockContainer = blockContainer;
    }

    public void GeneratePathAndBlock(int playerBlockMove, bool lastBlock)
    {
        if (playerBlockMove != cachedPlayerBlockMoved)
        {
            GeneratePath(lastBlock);
            GenerateBlocks();
            cachedPlayerBlockMoved = playerBlockMove;
            if (cachedPlayerBlockMoved > offsetBlocks + 3)
            {
                RemoveBlock();
            }
        }
    }

    public void PreGenerate()
    {
        for (int i = 0; i < preGenerateBlocks; i++)
        {
            GeneratePath(false);
            GenerateBlocks();
        }
    }
    
    private void GenerateBlocks()
    {
        currentBlockIndex++;
        var (position, rotation) = GetWorldSpaceTransformFromIndex(currentBlockIndex + offsetBlocks);
        var block = Object.Instantiate(trainingBlockPrefab, position, rotation, this.blockContainer);
        block.gameObject.SetActive(true);
        generatedTrainingMapBlocksQueue.Enqueue(block);
    }
    
    private void GeneratePath(bool lastBlock)
    {
        var lastPoint = pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 1];
        var direction = lastBlock ? GetLastBlockDirection(lastPoint) : GetLastBlockDirection(lastPoint);
        pathCreator.bezierPath.AddSegmentToEnd(lastPoint + direction * (trainingBlockPrefab.Size + spacingPerBlockWorldSpace));
        pathCreator.GetComponent<MapMeshGenerator>()?.TriggerUpdate();
    }

    private Vector3 GetLastBlockDirection(Vector3 lastPoint)
    {
        return Vector3.forward;
    }

    private Vector3 GetDirection(Vector3 lastPoint)
    {
        var direction = lastPoint - pathCreator.bezierPath[pathCreator.bezierPath.NumPoints - 2];
        direction = new Vector3(direction.x, 0, direction.z);
        var angle = Vector3.Angle(direction.normalized, Vector3.right);
        angle = Random.Range(angle - 5.0f, angle + 5.0f);
        direction = Quaternion.Euler(0, -angle, 0) * Vector3.right;
        return direction;
    }

    private float GetTimeFromWorldSpaceDistance(float distance)
    {
        return distance / pathCreator.path.length;
    }

    private float GetTimeFromMapBlockIndex(int index)
    {
        return GetTimeFromWorldSpaceDistance(index * trainingBlockPrefab.Size + index * spacingPerBlockWorldSpace);
    }

    private (Vector3 position, Quaternion rotation) GetWorldSpaceTransformFromTime(float t)
    {
        var rotationAtDistance = pathCreator.path.GetRotation(t);

        return (pathCreator.path.GetPointAtTime(t),
            Quaternion.Euler(0, rotationAtDistance.eulerAngles.y, 0));
    }

    private (Vector3, Quaternion) GetWorldSpaceTransformFromIndex(int index)
    {
        return GetWorldSpaceTransformFromTime(GetTimeFromMapBlockIndex(index));
    }
    
    private void RemoveBlock()
    {
        if (generatedTrainingMapBlocksQueue.Any())
        {
            var block = generatedTrainingMapBlocksQueue.Dequeue();
            Object.Destroy(block.gameObject);    
        }
    }

    public void Dispose()
    {
        while (generatedTrainingMapBlocksQueue.Any())
        {
            RemoveBlock();
        }
        Object.Destroy(pathCreator.gameObject);
    }
}