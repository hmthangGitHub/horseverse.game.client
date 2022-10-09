using DG.Tweening;
using UnityEngine;

public class TrainingObstacle : MonoBehaviour
{
    public GameObject[] gameObjects;
    private void OnEnable()
    {
        gameObjects.RandomElement().SetActive(true);
        DOTween.Sequence()
            .SetDelay(UnityEngine.Random.Range(0.0f, 0.2f))
            .Join(transform.DOMoveFrom(transform.position + Vector3.up * (-1.0f), transform.position, 1.0f).SetEase(Ease.OutElastic))
            .Join(transform.DOScaleFrom(transform.localScale * 0.0f, transform.localScale, 1.0f).SetEase(Ease.OutElastic));
    }
}