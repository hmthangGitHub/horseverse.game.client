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

    public async UniTask InitializeAsync(MasterHorseTrainingProperty masterHorseTrainingProperty,
                                         MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer,
                                         MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer,
                                         MasterTrainingDifficultyContainer masterTrainingDifficultyContainer,
                                         MasterTrainingBlockDistributeContainer masterTrainingBlockDistributeContainer)
    {
        this.masterHorseTrainingProperty = masterHorseTrainingProperty;
        this.masterHorseTrainingBlockContainer = masterHorseTrainingBlockContainer;
        this.masterHorseTrainingBlockComboContainer = masterHorseTrainingBlockComboContainer;
        this.masterTrainingDifficultyContainer = masterTrainingDifficultyContainer;
        this.masterTrainingBlockDistributeContainer = masterTrainingBlockDistributeContainer;
        await InitializeInternal();
        for (var i = 0; i < 4; i++)
        {
            Generate();
        }
    }

    protected abstract UniTask InitializeInternal();

    public abstract void Dispose();

    private Vector3 PredictRelativePointToPlayer()
    {
        var highestPoint = PredictHighestPoint();

        var timeToReach = Random.Range(horseTrainingControllerV2.AirTime.x, horseTrainingControllerV2.AirTime.y);
        var relativePointToPlayer = PredictDownPoint(timeToReach) 
                                    + highestPoint + Vector3.right * (horseTrainingControllerV2.HorizontalVelocity * Random.Range(-1f, 1f) * timeToReach);
        return relativePointToPlayer;
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

    private void Generate()
    {
        var relativePointToPlayer = PredictRelativePointToPlayer();
        var platformTest = lastPlatform.GetComponent<PlatformBase>();
        var lastEndPosition = platformTest.end.position;
        var platform = CreateNewPlatform(relativePointToPlayer, lastEndPosition);
        lastPlatform = platform;
        platformQueue.Enqueue(platform);
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
        Generate();
    }

    protected abstract PlatformBase CreatePlatform(Vector3 relativePointToPlayer,
                                                   Vector3 lastEndPosition);
}