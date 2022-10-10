using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathCreation;
using PathCreation.Examples;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [Header("BlockProperties")]
    [SerializeField] private PathCreator pathCreator;

    [SerializeField] private MeshPathContainer[] meshPathContainers;
    [SerializeField] private  TrainingPathBridge bridgePrefab;
    private int currentPath = 1;

    [SerializeField] private TrainingMapBlock trainingBlockPrefab;
    [SerializeField] private HorseTrainingController horseTrainingController;
    [SerializeField] private int preGenerateBlocks;
    [SerializeField] private int offsetBlocks;
    [SerializeField] private float spacingPerBlockWorldSpace;

    private bool isWaitingForPlayerToReachNewPath;
    private int numberOfBlockToChangeToNewPath;

    private readonly Queue<PathBlockGenerator> pathBlockGenerators = new Queue<PathBlockGenerator>();


    private void LateUpdate()
    {
        if (!isWaitingForPlayerToReachNewPath)
        {
            var playerBlockMove = (int)(horseTrainingController.HorseTrainingControllerData.TotalDistance / (trainingBlockPrefab.Size + spacingPerBlockWorldSpace));
            if (playerBlockMove <= numberOfBlockToChangeToNewPath)
            {
                pathBlockGenerators.First().GeneratePathAndBlock(playerBlockMove);
            }
            else
            {
                GenerateNewPath();
            }
        }
    }

    private void GenerateNewPath()
    {
        isWaitingForPlayerToReachNewPath = true;
        
        currentPath++;
        currentPath %= meshPathContainers.Length;
        var pathContainer = pathCreator.transform.parent.parent;
        var newPathContainer = Object.Instantiate(meshPathContainers[currentPath], pathContainer);
        newPathContainer.pathCreator.transform.position = pathCreator.bezierPath.GetPoint(pathCreator.bezierPath.NumPoints - 1) +
                                              pathCreator.transform.position + new Vector3(0, 0, 100);
        newPathContainer.pathCreator.InitializeEditorData(false);
        newPathContainer.pathCreator.GetComponent<MapMeshGenerator>()?.TriggerUpdate();
        trainingBlockPrefab.PathCreator = newPathContainer.pathCreator;
        trainingBlockPrefab.PathType = newPathContainer.pathType;
        CreateBridge(pathContainer, newPathContainer);
    }

    private void CreateBridge(Transform pathContainer, MeshPathContainer newPathContainer)
    {
        CreatePathBlockGenerator(newPathContainer.pathCreator);

        var bridge = Object.Instantiate(bridgePrefab, pathContainer);
        bridge.SourcePath = pathCreator;
        bridge.DestinationPath = newPathContainer.pathCreator;
        bridge.DestinationPathType = newPathContainer.pathType;
        bridge.CreateBridge();

        horseTrainingController.HorseTrainingControllerData.Bridge = bridge;

        void OnFinishLandingBridge()
        {
            Destroy(bridge.gameObject);
            pathCreator = newPathContainer.pathCreator;
            horseTrainingController.HorseTrainingControllerData.Bridge = default;
            isWaitingForPlayerToReachNewPath = false;
            RemoveOldPathBlockGenerator();
            horseTrainingController.HorseTrainingControllerData.OnFinishLandingBridge -= OnFinishLandingBridge;
        }

        horseTrainingController.HorseTrainingControllerData.OnFinishLandingBridge += OnFinishLandingBridge;
    }

    public void StartGame()
    {
        pathCreator.InitializeEditorData (false);
        CreatePathBlockGenerator(pathCreator);
        gameObject.SetActive(true);
        numberOfBlockToChangeToNewPath = preGenerateBlocks + Random.Range(5, 5);
    }

    private void CreatePathBlockGenerator(PathCreator pathCreator)
    {
        pathBlockGenerators.Enqueue(new PathBlockGenerator(pathCreator, 
            preGenerateBlocks, 
            trainingBlockPrefab, 
            offsetBlocks, 
            spacingPerBlockWorldSpace, 
            transform));
        pathBlockGenerators.Last().PreGenerate();
    }

    private void RemoveOldPathBlockGenerator()
    {
        var generator = pathBlockGenerators.Dequeue();
        generator.Dispose();
    }
}