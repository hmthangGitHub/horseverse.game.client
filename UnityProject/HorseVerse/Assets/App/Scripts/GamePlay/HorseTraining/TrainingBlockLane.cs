using UnityEngine;

public class TrainingBlockLane : MonoBehaviour
{
    public GameObject obstacle;
    public GameObject coin;

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
}
