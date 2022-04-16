using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorseRaceManager : MonoBehaviour
{
    public Transform[] transforms;
    public List<HorseController> horseControllers = new List<HorseController>();
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
    public int playerHorseId = 0;
    public Action OnFinishTrackEvent;

    public void StartRace(int[] playerList, int horseId)
    {
        playerHorseId = horseId;
        path.GetComponent<PathCreation.Examples.RoadMeshCreator>().TriggerUpdate();
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
            HorseController horseController = transforms[playerList[i]].GetComponent<HorseController>();
            horseController.currentOffset = offSet + i * offsetPerLane;
            horseController.top = top[i];
            horseController.timeToFinish = timeToFinish[top[i] - 1];
            horseController.currentTimeToFinish = horseController.timeToFinish;
            horseController.averageTimeToFinish = averageTimeToFinish;
            horseController.lap = totalLap;
            horseController.IsPlayer = playerHorseId == playerList[i];
            horseController.Lane = i;
            horseControllers.Add(horseController);
        }

        horseControllers.FirstOrDefault(x => x.top == 1).OnFinishTrackEvent += OnFinishTrack;
    }

    private void OnFinishTrack()
    {
        OnFinishTrackEvent.Invoke();
    }

    public void Skip()
    {
        foreach (var item in horseControllers)
        {
            item.Skip();
        }
    }
}
