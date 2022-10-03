using System;
using System.Linq;
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
    
    public PredefinePath PredefinePath { get; set; } 
    public float Size => boundingBoxReference.localScale.z;
    private void OnTriggerEnter(Collider other)
    {
        StartGenerate();
        container.SetActive(true);
    }

    private void StartGenerate()
    {
        var numberObstacle = Random.Range(0, 3);
        var obstacleLanes = lanes.Shuffle().Take(numberObstacle).ToArray();
        obstacleLanes.ForEach(x => x.GenObstacle());
        var coinLane = lanes.Where(x => !obstacleLanes.Contains(x)).Shuffle().First();
        if (Random.Range(0, 2) == 1 || true)
        {
            coinLane.GenCoin();
        }

        if (obstacleLanes.Contains(coinLane))
        {
            int x = 10;
        }
    }
}