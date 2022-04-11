using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorseRaceManager : MonoBehaviour
{
    public Transform[] transforms;
    public int[] top;
    public float[] timeToFinish;
    public uint laneNumber = 10;
    public float offSet = -5;
    public float offsetPerLane = -1.0f;
    public PathCreation.PathCreator path;
    public int totalLap = 1;
    public float timeToFinishAround = 0.0f;

    public float averageSpeed = 24.0f;
    public float averageTimeToFinish = 0.0f;
    public Vector2 timeOffSet = new Vector2(-1.0f, 1.0f);

    public float raceLength = 0.0f;
    public int playerHorseIndex = 0;
    private void Start()
    {
        path.GetComponent<PathCreation.Examples.RoadMeshCreator>().TriggerUpdate();
        playerHorseIndex = UnityEngine.Random.Range(0, transforms.Length);
        raceLength = path.path.length * path.transform.lossyScale.x;
        averageTimeToFinish = raceLength / averageSpeed * totalLap;
        timeToFinish = new float[top.Length];
        top = top.OrderBy(x => UnityEngine.Random.Range(-1.0f, 1.0f)).ToArray();
        for (int i = 0; i < timeToFinish.Length; i++)
        {
            timeToFinish[i] = averageTimeToFinish + UnityEngine.Random.Range(timeOffSet.x, timeOffSet.y);
        }
        timeToFinish = timeToFinish.OrderBy(x => x).ToArray();

        for (int i = 0; i < transforms.Length; i++)
        {
            HorseController horseController = transforms[i].GetComponent<HorseController>();
            horseController.currentOffset = offSet + i * offsetPerLane;
            horseController.top = top[i];
            horseController.timeToFinish = timeToFinish[top[i] - 1];
            horseController.averageTimeToFinish = averageTimeToFinish;
            horseController.lap = totalLap;
            horseController.IsPlayer = playerHorseIndex == i;
        }
    }
}
