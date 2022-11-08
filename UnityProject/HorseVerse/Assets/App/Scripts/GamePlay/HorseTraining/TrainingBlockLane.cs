using System;
using UnityEngine;

public class TrainingBlockLane : MonoBehaviour
{
    public GameObject obstacle;
    public GameObject coin;
    public TrainingObstacle trainingObstacle;
    public CoinGenerator coinGenerator;

    public void GenObstacle()
    {
        obstacle.SetActive(true);
        coin.SetActive(false);
    }
    
    public void GenCoin()
    {
        obstacle.SetActive(false);
        coin.SetActive(true);
    }

    public void GenBlockLane((MasterHorseTrainingLaneType laneType, int customValue) blockInfo)
    {
        
        trainingObstacle.gameObject.SetActive(false);
        coinGenerator.gameObject.SetActive(false);
        switch (blockInfo.laneType)
        {
            case MasterHorseTrainingLaneType.Empty:
                break;
            case MasterHorseTrainingLaneType.ShortObstacle:
                trainingObstacle.gameObject.SetActive(true);
                trainingObstacle.GeneObstacle(1);
                break;
            case MasterHorseTrainingLaneType.TallObstacle:
                trainingObstacle.gameObject.SetActive(true);
                trainingObstacle.GeneObstacle(0);
                break;
            case MasterHorseTrainingLaneType.Coin:
                coinGenerator.gameObject.SetActive(true);
                coinGenerator.GenerateCoinBlock(blockInfo.customValue, false);
                break;
            case MasterHorseTrainingLaneType.JumpCoin:
                coinGenerator.gameObject.SetActive(true);
                coinGenerator.GenerateCoinBlock(blockInfo.customValue, true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
