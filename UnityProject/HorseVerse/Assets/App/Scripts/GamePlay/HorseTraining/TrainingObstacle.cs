using DG.Tweening;
using UnityEngine;

public class TrainingObstacle : MonoBehaviour
{
    public GameObject[] gameObjects;

    public void GeneObstacle(int index)
    {
        gameObjects.ForEach((x, i) => x.SetActive(i == index));
    }
}