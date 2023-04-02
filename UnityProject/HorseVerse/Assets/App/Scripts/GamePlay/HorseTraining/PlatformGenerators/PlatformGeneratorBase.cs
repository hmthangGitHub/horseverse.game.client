﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class PlatformGeneratorBase : MonoBehaviour, IDisposable
{
    protected MasterHorseTrainingProperty masterHorseTrainingProperty;
    protected MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer;
    protected MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer;
    protected MasterTrainingBlockDistributeContainer masterTrainingBlockDistributeContainer;
    
    public PlatformBase platformPrefab;
    public GameObject lastPlatform;
    public HorseTrainingControllerV2 horseTrainingControllerV2;
    public PlatformGeneratorPool pool;

    private readonly Queue<GameObject> platformQueue = new Queue<GameObject>();
    private bool isFirstJump = true;
    private bool isEndScene = false;

    private Vector3 nextDirection = Vector3.forward;
    private Vector3 nextSideDirection = Vector3.right;

    public Vector3 NextDirection => nextDirection;

    public event Action OnFinishOnePlatform = ActionUtility.EmptyAction.Instance;
    public event Action OnFinishOneScene = ActionUtility.EmptyAction.Instance;

    protected bool isReleasing = false;
    protected int numberOfBlock = 0;
    protected int currentBlock = 0;

    public async UniTask InitializeAsync(MasterHorseTrainingProperty masterHorseTrainingProperty,
                                         MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer,
                                         MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer,
                                         MasterTrainingDifficultyContainer masterTrainingDifficultyContainer,
                                         MasterTrainingBlockDistributeContainer masterTrainingBlockDistributeContainer,
                                         string Scene_Key,
                                         int NumberOfBlocks,
                                         Vector3 direction)
    {
        this.masterHorseTrainingProperty = masterHorseTrainingProperty;
        this.masterHorseTrainingBlockContainer = masterHorseTrainingBlockContainer;
        this.masterHorseTrainingBlockComboContainer = masterHorseTrainingBlockComboContainer;
        this.masterTrainingBlockDistributeContainer = masterTrainingBlockDistributeContainer;
        this.numberOfBlock = NumberOfBlocks;
        this.currentBlock = 0;
        isEndScene = false;
        this.nextDirection = direction;
        this.nextSideDirection = Quaternion.AngleAxis(90, Vector3.up) * nextDirection;
        Debug.Log("DIRECTION " + this.nextDirection + "; SIDE  " + this.nextSideDirection);
        await InitializeInternal(Scene_Key);
        GenerateMulti(2);
    }

    public async UniTask UpdateMapAsync( MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer,
                                         MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer,
                                         string Scene_Key, 
                                         int NumberOfBlocks,
                                         Vector3 direction)
    {
        this.masterHorseTrainingBlockContainer = masterHorseTrainingBlockContainer;
        this.masterHorseTrainingBlockComboContainer = masterHorseTrainingBlockComboContainer;
        this.numberOfBlock = NumberOfBlocks;
        this.currentBlock = 0;
        isEndScene = false;
        this.nextDirection = direction;
        this.nextSideDirection = Quaternion.AngleAxis(90, Vector3.up) * nextDirection;
        Debug.Log("DIRECTION " + this.nextDirection + "; SIDE  " + this.nextSideDirection);
        await ReleaseInternal();
        await InitializeInternal(Scene_Key);
    }

    protected abstract UniTask InitializeInternal();
    protected abstract UniTask InitializeInternal(string path);
    protected abstract UniTask ReleaseInternal();


    public abstract void Dispose();

    private Vector3 PredictRelativePointToPlayer(Vector3 direction, bool isJumping = true)
    {
        var highestPoint = isJumping ? PredictHighestPoint() : Vector3.zero;

        var timeToReach = Random.Range(horseTrainingControllerV2.AirTime.x, horseTrainingControllerV2.AirTime.y);
        var horizontalOffset = isJumping ? horseTrainingControllerV2.HorizontalVelocity * Random.Range(-1f, 1f) : 0.0f;
        var relativePointToPlayer = PredictDownPoint(timeToReach) + highestPoint + Vector3.right * (horizontalOffset * timeToReach);
        return Quaternion.LookRotation(direction) * relativePointToPlayer;
    }

    private Vector3 PredictDownPoint(float time)
    {
        var z = horseTrainingControllerV2.ForwardVelocity * time;
        var y = horseTrainingControllerV2.LowJumpMultiplier * horseTrainingControllerV2.DefaultGravity * 0.5f * time * time;
        return new Vector3(0, y, z);
    }

    private Vector3 PredictHighestPoint()
    {
        var jumpVel = new Vector3(0, horseTrainingControllerV2.JumpVelocity, horseTrainingControllerV2.ForwardVelocity); 
        var v0 = jumpVel.magnitude;

        var angle = Mathf.Deg2Rad * Vector3.Angle(jumpVel, Vector3.forward);
        var maxZ = v0 * v0 * Mathf.Sin(2 * angle) / (-horseTrainingControllerV2.DefaultGravity * 2); 
        var maxY = (horseTrainingControllerV2.JumpVelocity * horseTrainingControllerV2.JumpVelocity) / (2 * -horseTrainingControllerV2.DefaultGravity);
        return new Vector3(0, maxY, maxZ);
    }

    protected void TurnLeft()
    {
        nextDirection = Quaternion.AngleAxis(-90, Vector3.up) * nextDirection;
        nextSideDirection = Quaternion.AngleAxis(-90, Vector3.up) * nextSideDirection;
    }

    protected void TurnRight()
    {
        nextDirection = Quaternion.AngleAxis(90, Vector3.up) * nextDirection;
        nextSideDirection = Quaternion.AngleAxis(90, Vector3.up) * nextSideDirection;
    }

    private async UniTask GenerateAsync()
    {
        var platformTest = lastPlatform.GetComponent<PlatformBase>();
        var lastEndPosition = platformTest.end.position;

        int random = Random.Range(0, 4);

        if (random == 2 || random == 3)
        {
            await CreateTurnPlatformsAsync(lastEndPosition, random);
        }
        else
        {
            var relativePointToPlayer = PredictRelativePointToPlayer(nextDirection);
            var platform = await CreateNewPlatformAsync(relativePointToPlayer, lastEndPosition, nextDirection);
            lastPlatform = platform;
            platformQueue.Enqueue(platform);
        }
    }

    private async UniTask CreateTurnPlatformsAsync(Vector3 lastEndPosition,
                                       int random)
    {
        var relativePointToPlayer = PredictRelativePointToPlayer(nextDirection);
        var platform1 = await CreatePlatformWithoutSceneryObjectAsync(relativePointToPlayer, lastEndPosition);
        RotatePlatform(platform1, nextDirection);
        platform1.OnFinishPlatform += OnDestroyPlatform;
        platformQueue.Enqueue(platform1.gameObject);
        
        List<BoxCollider> _ff = new List<BoxCollider>();
        lastEndPosition = platform1.end.position;
        var pp1 = platform1.GetComponent<PlatformModular>();
        pp1.CreateSceneryRegions();
        _ff.Add(pp1.sceneryConflictRegion);
        PlatformBase platform = default;
        
        if (random == 2)
        {
            platform = await CreateTurnPlatformAsync(TYPE_OF_BLOCK.TURN_LEFT, relativePointToPlayer, lastEndPosition);
            platform.OnFinishPlatform += OnDestroyPlatform;
            RotatePlatform(platform, nextDirection);
            platformQueue.Enqueue(platform.gameObject);
            TurnLeft();

            relativePointToPlayer = PredictRelativePointToPlayer(nextDirection);
            lastEndPosition = platform.end.position;

            var pp = platform.GetComponent<PlatformModular>();
            pp.CreateSceneryRegions(1);
            _ff.Add(pp.sceneryConflictRegion);
        }
        else if (random == 3)
        {
            platform = await CreateTurnPlatformAsync(TYPE_OF_BLOCK.TURN_RIGHT, relativePointToPlayer, lastEndPosition);
            platform.OnFinishPlatform += OnDestroyPlatform;
            RotatePlatform(platform, nextDirection);
            platformQueue.Enqueue(platform.gameObject);
            TurnRight();

            relativePointToPlayer = PredictRelativePointToPlayer(nextDirection);
            lastEndPosition = platform.end.position;

            var pp = platform.GetComponent<PlatformModular>();
            pp.CreateSceneryRegions(1);
            _ff.Add(pp.sceneryConflictRegion);
        }

        CreateDebugSphere(relativePointToPlayer + lastEndPosition);
        var platform2 = await CreatePlatformWithoutSceneryObjectAsync(relativePointToPlayer, lastEndPosition);
        RotatePlatform(platform2, nextDirection);

        currentBlock++;
        SetEndOfBlockBehaviour(platform2);

        platformQueue.Enqueue(platform2.gameObject);
        var pp2 = platform2.GetComponent<PlatformModular>();
        pp2.CreateSceneryRegions();
        _ff.Add(pp2.sceneryConflictRegion);
        lastPlatform = platform2.gameObject;
        var ff = _ff.ToArray();
        
        await CreateSceneryObectAsync(platform1, ff);
        await CreateSceneryObectAsync(platform, ff);
        await CreateSceneryObectAsync(platform2, ff);
    }

    private void SetEndOfBlockBehaviour(PlatformBase platform2)
    {
        if (currentBlock >= numberOfBlock)
        {
            platform2.OnFinishPlatform += OnEndOfScene;
        }
        else
        {
            platform2.OnFinishPlatform += OnCreateNewPlatform;
        }
    }

    private void GenerateMulti(int number)
    {
        for(int i = 0; i < number; i++)
        {
            var relativePointToPlayer = PredictRelativePointToPlayer(nextDirection);
            var platformTest = lastPlatform.GetComponent<PlatformBase>();
            var lastEndPosition = platformTest.end.position;
            var platform = CreateNewPlatform(relativePointToPlayer, lastEndPosition, nextDirection);
            lastPlatform = platform;
            platformQueue.Enqueue(platform);
        }
    }

    public async UniTask GenerateMultiAsync(int number)
    {
        for (var i = 0; i < number; i++)
        {
            var relativePointToPlayer = PredictRelativePointToPlayer(nextDirection, i != 0); 
            var platformTest = lastPlatform.GetComponent<PlatformBase>();
            var lastEndPosition = i != 0 ? platformTest.end.position : horseTrainingControllerV2.transform.position;
            var platform = await CreateNewPlatformAsync(relativePointToPlayer, lastEndPosition, nextDirection);
            lastPlatform = platform;
            platformQueue.Enqueue(platform);
        }
    }

    private void CreateDebugSphere(Vector3 position)
    {
#if UNITY_EDITOR
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * 0.1f;
        sphere.GetComponent<Collider>().enabled = false;
#endif
    }
    
    private GameObject CreateNewPlatform(Vector3 relativePointToPlayer,
                                         Vector3 lastEndPosition,
                                         Vector3 direction)
    {
        CreateDebugSphere(relativePointToPlayer + lastEndPosition);
        var platform = CreatePlatform(relativePointToPlayer, lastEndPosition);
        currentBlock++;
        SetEndOfBlockBehaviour(platform);
        Debug.LogError("currentBlock " + currentBlock + " vs " + numberOfBlock + direction);
        RotatePlatform(platform, direction);
        return platform.gameObject;
    }

    private static void RotatePlatform(PlatformBase platform, Vector3 direction)
    {
        platform.transform.RotateAround(platform.start.position, Vector3.up, Quaternion.LookRotation(direction).eulerAngles.y);
    }

    private async UniTask<GameObject> CreateNewPlatformAsync(Vector3 relativePointToPlayer,
                                                             Vector3 lastEndPosition, 
                                                             Vector3 direction)
    {
        CreateDebugSphere(relativePointToPlayer + lastEndPosition);
        var platform = await CreatePlatformAsync(relativePointToPlayer, lastEndPosition);
        currentBlock++;
        SetEndOfBlockBehaviour(platform);
        RotatePlatform(platform, direction);
        Debug.LogError("currentBlock " + currentBlock + " vs " + numberOfBlock + direction);
        return platform.gameObject;
    }

    private void OnCreateNewPlatform()
    {
        if (isFirstJump)
        {
            isFirstJump = false;
        }
        else
        {
            var pp = platformQueue.Dequeue();
            pp.GetComponent<PlatformModular>().Clear();
            Destroy(pp);
        }
        if(currentBlock < numberOfBlock)
            GenerateAsync().AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        OnFinishOnePlatform?.Invoke();
    }

    private void OnEndOfScene()
    {
        Debug.Log(" End Of Scene");
        var pp = platformQueue.Dequeue();
        pp.GetComponent<PlatformModular>().Clear();
        Destroy(pp);

        isEndScene = true;
        OnFinishOnePlatform?.Invoke();
        OnFinishOneScene?.Invoke();
    }

    private void OnDestroyPlatform()
    {
        var pp = platformQueue.Dequeue();
        pp.GetComponent<PlatformModular>().Clear();
        Destroy(pp);

        OnFinishOnePlatform?.Invoke();
    }

    protected abstract PlatformBase CreatePlatform(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition);

    protected virtual UniTask<PlatformBase> CreatePlatformAsync(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition)
    {
        return default;
    }

    protected virtual UniTask<PlatformBase> CreateTurnPlatformAsync(TYPE_OF_BLOCK type, Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition)
    {
        return default;
    }

    protected virtual UniTask<PlatformBase> CreatePlatformWithoutSceneryObjectAsync(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition)
    {
        return default;
    }

    protected virtual UniTask CreateSceneryObectAsync(PlatformBase platform, BoxCollider[] boxColliders)
    {
        return default;
    }
}