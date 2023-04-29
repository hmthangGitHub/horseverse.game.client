using System;
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

    private GameObject startingPlatform;

    private enum EndOfBlockBehaviour
    {
        DestroyPreviousAndCreateNew,
        DestroyPrevious,
    }

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
        GenerateMultiInitialize(2);
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

    public void SetLastPlatform(GameObject _lastPlatform)
    {
        lastPlatform = _lastPlatform;
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

    private void Turn(TYPE_OF_BLOCK typeOfBlock)
    {
        switch (typeOfBlock)
        {
            case TYPE_OF_BLOCK.TURN_LEFT:
                TurnLeft();
                break;
            case TYPE_OF_BLOCK.TURN_RIGHT:
                TurnRight();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(typeOfBlock), typeOfBlock, null);
        }
    }

    private void TurnLeft()
    {
        nextDirection = Quaternion.AngleAxis(-90, Vector3.up) * nextDirection;
        nextSideDirection = Quaternion.AngleAxis(-90, Vector3.up) * nextSideDirection;
    }

    private void TurnRight()
    {
        nextDirection = Quaternion.AngleAxis(90, Vector3.up) * nextDirection;
        nextSideDirection = Quaternion.AngleAxis(90, Vector3.up) * nextSideDirection;
    }

    private async UniTask GenerateAsync()
    {
        var lastEndPosition = lastPlatform.GetComponent<PlatformBase>().end.position;

        int random = Random.Range(0, 4);

        switch (random)
        {
            case 0:
            case 1:
                await CreateNewPlatformAlongWithPrevious(nextDirection, 
                    TYPE_OF_BLOCK.NORMAL,
                    EndOfBlockBehaviour.DestroyPreviousAndCreateNew, 
                    true);
                break;
            case 2:
                await CreateComboThreeBlockToTurn(TYPE_OF_BLOCK.TURN_LEFT);
                break;
            case 3:
                await CreateComboThreeBlockToTurn(TYPE_OF_BLOCK.TURN_RIGHT);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(random));
        }
    }

    private async UniTask CreateComboThreeBlockToTurn(TYPE_OF_BLOCK blockTurnType)
    {
        await CreateNewPlatformAlongWithPrevious(nextDirection, 
            TYPE_OF_BLOCK.NORMAL, 
            EndOfBlockBehaviour.DestroyPrevious, 
            false);

        await CreateNewPlatformAlongWithPrevious(nextDirection, 
            blockTurnType, 
            EndOfBlockBehaviour.DestroyPrevious,
            false);
        
        Turn(blockTurnType);

        await CreateNewPlatformAlongWithPrevious(nextDirection, 
            TYPE_OF_BLOCK.NORMAL,
            EndOfBlockBehaviour.DestroyPreviousAndCreateNew, 
            true);
    }

    private void SetEndOfBlockBehaviour(PlatformBase platform, EndOfBlockBehaviour endOfBlockBehaviour)
    {
        Action action;
#if ENABLE_ADVENTURE_CHANGE_SCENE
        if (currentBlock >= numberOfBlock)
        {
            action = OnEndOfScene;
        }
        else
#endif
        {
            action = endOfBlockBehaviour switch
            {
                EndOfBlockBehaviour.DestroyPrevious => OnDestroyPlatform,
                EndOfBlockBehaviour.DestroyPreviousAndCreateNew => OnCreateNewPlatform,
                _ => throw new ArgumentOutOfRangeException(nameof(endOfBlockBehaviour), endOfBlockBehaviour, null)
            };
        }

        platform.OnFinishPlatform += action;
    }

    private void GenerateMultiInitialize(int number)
    {
        for(int i = 0; i < number; i++)
        {
            var relativePointToPlayer = PredictRelativePointToPlayer(nextDirection);
            var platformTest = lastPlatform.GetComponent<PlatformBase>();
            var lastEndPosition = platformTest.end.position;
            var platform = CreateNewPlatformInitialize(relativePointToPlayer, lastEndPosition, nextDirection);
            lastPlatform = platform;
            platformQueue.Enqueue(platform);
        }
    }

    public async UniTask GenerateMultiBlockAsyncWhenChangeScene(int number)
    {
        for (var i = 0; i < number; i++)
        {
            var relativePointToPlayer = PredictRelativePointToPlayer(nextDirection, i != 0); 
            var platformTest = lastPlatform.GetComponent<PlatformBase>();
            var lastEndPosition = platformTest.end.position; //i != 0 ? platformTest.end.position : horseTrainingControllerV2.transform.position;
            await CreateNewPlatformAsync(relativePointToPlayer, lastEndPosition, nextDirection, TYPE_OF_BLOCK.NORMAL, EndOfBlockBehaviour.DestroyPreviousAndCreateNew, true);
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
    
    private GameObject CreateNewPlatformInitialize(Vector3 relativePointToPlayer,
                                         Vector3 lastEndPosition,
                                         Vector3 direction)
    {
        CreateDebugSphere(relativePointToPlayer + lastEndPosition);
        var platform = CreatePlatform(relativePointToPlayer, lastEndPosition);
#if ENABLE_ADVENTURE_CHANGE_SCENE
        currentBlock++;
#endif
        SetEndOfBlockBehaviour(platform, EndOfBlockBehaviour.DestroyPreviousAndCreateNew);
        RotatePlatform(platform, direction);
        return platform.gameObject;
    }

    private static void RotatePlatform(PlatformBase platform, Vector3 direction)
    {
        platform.transform.RotateAround(platform.start.position, Vector3.up, Quaternion.LookRotation(direction).eulerAngles.y);
    }

    private async UniTask CreateNewPlatformAlongWithPrevious(Vector3 direction,
                                                             TYPE_OF_BLOCK platformType,
                                                             EndOfBlockBehaviour endOfBlockBehaviour,
                                                             bool isSinglePlatform)
    {
        var lastEndPosition = lastPlatform.GetComponent<PlatformBase>().end.position;
        var relativePointToPlayer = PredictRelativePointToPlayer(nextDirection);
        await CreateNewPlatformAsync(relativePointToPlayer,
            lastEndPosition, 
            direction, 
            platformType,
            endOfBlockBehaviour, 
            isSinglePlatform);
    }
    
    private async UniTask<GameObject> CreateNewPlatformAsync(Vector3 relativePointToPlayer,
                                                             Vector3 lastEndPosition, 
                                                             Vector3 direction,
                                                             TYPE_OF_BLOCK platformType,
                                                             EndOfBlockBehaviour endOfBlockBehaviour,
                                                             bool isSinglePlatform)
    {
        CreateDebugSphere(relativePointToPlayer + lastEndPosition);
        var platform = platformType switch
        {
            TYPE_OF_BLOCK.NORMAL => await CreatePlatformAsync(relativePointToPlayer, lastEndPosition),
            TYPE_OF_BLOCK.TURN_LEFT => await CreateTurnPlatformAsync(TYPE_OF_BLOCK.TURN_LEFT, relativePointToPlayer, lastEndPosition),
            TYPE_OF_BLOCK.TURN_RIGHT => await CreateTurnPlatformAsync(TYPE_OF_BLOCK.TURN_RIGHT, relativePointToPlayer, lastEndPosition),
            TYPE_OF_BLOCK.NORMAL_WITHOUT_SCENERY_OBJECT => await CreatePlatformWithoutSceneryObjectAsync(relativePointToPlayer, lastEndPosition),
            _ => throw new ArgumentOutOfRangeException(nameof(platformType), platformType, null)
        };
#if ENABLE_ADVENTURE_CHANGE_SCENE
        if (isSinglePlatform)
        {
            currentBlock++;
        }
#endif
        SetEndOfBlockBehaviour(platform, endOfBlockBehaviour);
        RotatePlatform(platform, direction);
        lastPlatform = platform.gameObject;
        platformQueue.Enqueue(lastPlatform);
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

        if (currentBlock < numberOfBlock)
        {
            GenerateAsync().AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }
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

    protected virtual UniTask CreateSceneryObjectAsync(PlatformBase platform, BoxCollider[] boxColliders)
    {
        return default;
    }
}