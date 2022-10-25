using System;
using System.Linq;
using JetBrains.Annotations;
using PathCreation;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrainingMapBlock : MonoBehaviour
{
    private enum Lane : int
    {
        Left = 0,
        Mid = 1,
        Right = 2
    }
    [SerializeField] private Transform boundingBoxReference;
    [SerializeField] private GameObject container;
    [SerializeField] private TrainingBlockLane[] lanes;
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private MeshPathContainer.PathType pathType;

    public float Size => boundingBoxReference.localScale.z;

    public PathCreator PathCreator
    {
        get => pathCreator;
        set => pathCreator = value;
    }

    public MeshPathContainer.PathType PathType
    {
        get => pathType;
        set => pathType = value;
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     StartGenerate();
    // }

    private void Start()
    {
        StartGenerate();
    }

    private void StartGenerate()
    {
        var numberObstacle = Random.Range(0, 3);
        var obstacleLanes = (lanes.Shuffle()).Take(numberObstacle).ToArray();
        obstacleLanes.ForEach(x => x.GenObstacle());
        var coinLane = lanes.Where(x => !obstacleLanes.Contains(x)).Shuffle().First();
        if (Random.Range(0, 2) == 1 || true)
        {
            coinLane.GenCoin();
        }
        container.SetActive(true);
    }
}