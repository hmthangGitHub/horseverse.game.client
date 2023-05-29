using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class HorseTrainingManager : MonoBehaviour, IDisposable
{
    [SerializeField] private HorseTrainingControllerV2 horseTrainingController;
    [SerializeField] private PlatformGeneratorBase platformGenerator;
    [SerializeField] private Animator changeSceneVFX;
    public PlatformGeneratorBase PlatformGenerator => platformGenerator;

    public HorseTrainingControllerV2 HorseTrainingController => horseTrainingController;

    private GameObject follower = default;

    public async UniTask Initialize(string mapPath,
                                    string mapId,
                                    int NumberOfBlock,
                                    Action onTakeCoin,
                                    Action onUpdateRunTime,
                                    Action onTouchObstacle,
                                    Action onFinishOnePlatform,
                                    Action onFinishOneScene,
                                    MasterHorseTrainingProperty masterHorseTrainingProperty,
                                    MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer,
                                    MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer,
                                    MasterTrainingBlockDistributeContainer masterTrainingBlockDistributeContainer,
                                    MasterTrainingDifficultyContainer masterTrainingDifficultyContainer,
                                    HorseMeshInformation horseMeshInformation)
    {
        await HorseTrainingController.Initialize(masterHorseTrainingProperty, masterTrainingDifficultyContainer, horseMeshInformation);
        HorseTrainingController.OnTakeCoin += onTakeCoin;
        HorseTrainingController.OnUpdateRunTime += onUpdateRunTime;
        HorseTrainingController.OnDeadEvent += onTouchObstacle;
        PlatformGenerator.OnFinishOnePlatform += onFinishOnePlatform;
        PlatformGenerator.OnFinishOneScene += onFinishOneScene;
        await PlatformGenerator.InitializeAsync(masterHorseTrainingProperty, 
            masterHorseTrainingBlockContainer, 
            masterHorseTrainingBlockComboContainer, 
            masterTrainingDifficultyContainer, 
            masterTrainingBlockDistributeContainer,
            mapId,
            NumberOfBlock,
            Vector3.forward);

        await UniTask.DelayFrame(5);

        if (SceneEntityComponent.Instance != default)
        {
            //SceneEntityComponent.Instance.SetCameraTarget(HorseTrainingController.transform);
            
            if (SceneEntityComponent.Instance.Skybox != default)
                RenderSettings.skybox = SceneEntityComponent.Instance.Skybox;
            var ss = SceneEntityComponent.Instance.InstanceFollow(this.transform);
            ss.target = HorseTrainingController.transform;
            if (follower != default) Destroy(follower);
            follower = ss.gameObject;
        }
        else Debug.LogError("Cant find Entity");
    }

    public void StartGame()
    {
        HorseTrainingController.IsStart = true;
    }

    public void Dispose()
    {
        DisposeUtility.SafeDispose(ref horseTrainingController);
    }

    public async UniTask UpdateMap( string mapPath,
                                    string mapId,
                                    int NumberOfBlock,
                                    MasterHorseTrainingBlockContainer masterHorseTrainingBlockContainer,
                                    MasterHorseTrainingBlockComboContainer masterHorseTrainingBlockComboContainer,
                                    MasterTrainingBlockDistributeContainer masterTrainingBlockDistributeContainer)
    {
        var dir = Vector3.forward;//PlatformGenerator.NextDirection;
        PlatformGenerator.SetLastPlatform(null);
        await PlatformGenerator.UpdateMapAsync(masterHorseTrainingBlockContainer,
            masterHorseTrainingBlockComboContainer,
            masterTrainingBlockDistributeContainer,
            mapId,
            NumberOfBlock,
            dir);

        await UniTask.DelayFrame(5);
        HorseTrainingController.SetDirection(dir);
        HorseTrainingController.EnableRotateToWard();
        if (SceneEntityComponent.Instance != default)
        {
            //SceneEntityComponent.Instance.SetCameraTarget(HorseTrainingController.transform);
            if(SceneEntityComponent.Instance.Skybox != default)
                RenderSettings.skybox = SceneEntityComponent.Instance.Skybox;
            HorseTrainingController.transform.position = SceneEntityComponent.Instance.ChangingPoint.transform.position;
            PlatformGenerator.SetLastPlatform(SceneEntityComponent.Instance.StartPlatform);

            var ss = SceneEntityComponent.Instance.InstanceFollow(this.transform);
            ss.target = HorseTrainingController.transform;
            if (follower != default) Destroy(follower);
            follower = ss.gameObject;
        }
        else Debug.LogError("Cant find Entity");
    }

    public async UniTask PerformHighJumpToChangeSceneAsync()
    {
        await (horseTrainingController.PerformHighJumpToChangeSceneAsync(), PerformChangeSceneEffect());
    }

    public void LandToNewScene()
    {
        horseTrainingController.LandToNewScene();
    }

    public async UniTask GenerateMultiBlockAsyncWhenChangeScene(int numberOfBlock)
    {
        await PlatformGenerator.GenerateMultiBlockAsyncWhenChangeScene(numberOfBlock);

        await UniTask.Yield();

        if (SceneEntityComponent.Instance != default)
        {
            //SceneEntityComponent.Instance.SetCameraTarget(HorseTrainingController.transform);
            var ss = SceneEntityComponent.Instance.InstanceFollow(this.transform);
            ss.target = HorseTrainingController.transform;
            if (follower != default) Destroy(follower);
            follower = ss.gameObject;
        }
    }

    public async UniTask PerformChangeSceneEffect()
    {
        await UniTask.Delay(1500);
        await PerformChangeSceneEffect(true);
    }

    public async UniTask StopChangeSceneEffect()
    {
        await PerformChangeSceneEffect(false);
    }

    private IEnumerator PerformChangeSceneEffect(bool active)
    {
        changeSceneVFX.gameObject.SetActive(true);
        if (active)
        {
            changeSceneVFX.SetTrigger("In");
            yield return new WaitForSeconds(4.0f);
        }
        else
        {
            changeSceneVFX.SetTrigger("Out");
            yield return new WaitForSeconds(3.5f);
            changeSceneVFX.gameObject.SetActive(false);
        }
    }
}
