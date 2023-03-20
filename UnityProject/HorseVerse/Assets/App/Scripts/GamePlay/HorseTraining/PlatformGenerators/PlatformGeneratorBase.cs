using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class PlatformGeneratorBase : MonoBehaviour, IDisposable
{
    protected MasterHorseTrainingProperty masterHorseTrainingProperty;
    protected MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer;
    protected MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer;
    protected MasterTrainingDifficultyContainer masterTrainingDifficultyContainer;
    protected MasterTrainingBlockDistributeContainer masterTrainingBlockDistributeContainer;
    
    public PlatformBase platformPrefab;
    public GameObject lastPlatform;
    public HorseTrainingControllerV2 horseTrainingControllerV2;
    public PlatformGeneratorPool pool;

    private readonly Queue<GameObject> platformQueue = new Queue<GameObject>();
    private bool isFirstJump = true;

    protected Vector3 nextDirection = Vector3.forward;
    protected Vector3 nextSideDirection = Vector3.right;


    public async UniTask InitializeAsync(MasterHorseTrainingProperty masterHorseTrainingProperty,
                                         MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer,
                                         MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer,
                                         MasterTrainingDifficultyContainer masterTrainingDifficultyContainer,
                                         MasterTrainingBlockDistributeContainer masterTrainingBlockDistributeContainer,
                                         string Scene_Key)
    {
        this.masterHorseTrainingProperty = masterHorseTrainingProperty;
        this.masterHorseTrainingBlockContainer = masterHorseTrainingBlockContainer;
        this.masterHorseTrainingBlockComboContainer = masterHorseTrainingBlockComboContainer;
        this.masterTrainingDifficultyContainer = masterTrainingDifficultyContainer;
        this.masterTrainingBlockDistributeContainer = masterTrainingBlockDistributeContainer;
        await InitializeInternal(Scene_Key);
        GenerateMulti(2);
    }

    protected abstract UniTask InitializeInternal();
    protected abstract UniTask InitializeInternal(string path);

    public abstract void Dispose();

    private Vector3 PredictRelativePointToPlayer()
    {
        var highestPoint = PredictHighestPoint();

        var timeToReach = Random.Range(horseTrainingControllerV2.AirTime.x, horseTrainingControllerV2.AirTime.y);
        var relativePointToPlayer = PredictDownPoint(timeToReach) 
                                    + highestPoint + nextSideDirection * (horseTrainingControllerV2.HorizontalVelocity * Random.Range(-1f, 1f) * timeToReach);
        return relativePointToPlayer;
    }

    private Vector3 PredictDownPoint(float time)
    {
        var z = horseTrainingControllerV2.ForwardVelocity * time;
        var y = horseTrainingControllerV2.LowJumpMultiplier * horseTrainingControllerV2.DefaultGravity * 0.5f * time * time;
        return new Vector3(0, y, 0) + nextDirection * z;
    }

    private Vector3 PredictHighestPoint()
    {
        var jumpVel = new Vector3(0, horseTrainingControllerV2.JumpVelocity, 0) + nextDirection * horseTrainingControllerV2.ForwardVelocity; 
        var v0 = jumpVel.magnitude;

        var angle = Mathf.Deg2Rad * Vector3.Angle(jumpVel, nextDirection);
        var maxZ = v0 * v0 * Mathf.Sin(2 * angle) / (-horseTrainingControllerV2.DefaultGravity * 2); 
        var maxY = (horseTrainingControllerV2.JumpVelocity * horseTrainingControllerV2.JumpVelocity) / (2 * -horseTrainingControllerV2.DefaultGravity);
        return new Vector3(0, maxY, 0) + nextDirection * maxZ;
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

    private void Generate()
    {
        var relativePointToPlayer = PredictRelativePointToPlayer();
        var platformTest = lastPlatform.GetComponent<PlatformBase>();
        var lastEndPosition = platformTest.end.position;
        var platform = CreateNewPlatform(relativePointToPlayer, lastEndPosition);
        lastPlatform = platform;
        platformQueue.Enqueue(platform);
    }

    private async UniTask GenerateAsync()
    {
        var relativePointToPlayer = PredictRelativePointToPlayer();
        var platformTest = lastPlatform.GetComponent<PlatformBase>();
        var lastEndPosition = platformTest.end.position;

        int random = Random.Range(0, 4);

        if (random == 2 || random == 3)
        {
            var platform = await CreateNewTurnPlatformAsync(relativePointToPlayer, lastEndPosition, nextDirection);
            lastPlatform = platform;
            platformQueue.Enqueue(platform);
            TurnLeft();
        }
        else
        {
            var platform = await CreateNewPlatformAsync(relativePointToPlayer, lastEndPosition, nextDirection);
            lastPlatform = platform;
            platformQueue.Enqueue(platform);

        }

        //if (random == 3) TurnRight();
    }

    private void GenerateMulti(int number)
    {
        for(int i = 0; i < number; i++)
        {
            var relativePointToPlayer = PredictRelativePointToPlayer();
            var platformTest = lastPlatform.GetComponent<PlatformBase>();
            var lastEndPosition = platformTest.end.position;
            var platform = CreateNewPlatform(relativePointToPlayer, lastEndPosition);
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
                                         Vector3 lastEndPosition)
    {
        CreateDebugSphere(relativePointToPlayer + lastEndPosition);
        var platform = CreatePlatform(relativePointToPlayer, lastEndPosition);
        platform.OnFinishPlatform += OnCreateNewPlatform;
        return platform.gameObject;
    }

    private async UniTask<GameObject> CreateNewPlatformAsync(Vector3 relativePointToPlayer,
                                         Vector3 lastEndPosition, Vector3 direction)
    {
        CreateDebugSphere(relativePointToPlayer + lastEndPosition);
        var platform = await CreatePlatformAsync(relativePointToPlayer, lastEndPosition);
        platform.OnFinishPlatform += OnCreateNewPlatform;
        platform.transform.forward = direction;
        return platform.gameObject;
    }

    private async UniTask<GameObject> CreateNewTurnPlatformAsync(Vector3 relativePointToPlayer,
                                         Vector3 lastEndPosition, Vector3 direction)
    {
        CreateDebugSphere(relativePointToPlayer + lastEndPosition);
        var platform = await CreateTurnPlatformAsync(relativePointToPlayer, lastEndPosition);
        platform.OnFinishPlatform += OnCreateNewPlatform;
        platform.transform.forward = direction;
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
        GenerateAsync().AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
    }

    protected abstract PlatformBase CreatePlatform(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition);

    protected virtual async UniTask<PlatformBase> CreatePlatformAsync(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition)
    {
        return null;
    }

    protected virtual async UniTask<PlatformBase> CreateTurnPlatformAsync(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition)
    {
        return null;
    }
}