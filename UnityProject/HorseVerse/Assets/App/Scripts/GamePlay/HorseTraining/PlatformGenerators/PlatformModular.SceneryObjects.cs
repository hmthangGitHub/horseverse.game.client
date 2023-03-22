using System.Linq;
using UnityEngine;
using System.Collections;

public partial class PlatformModular
{
    [SerializeField]
    private Vector3 sceneryConflictRegionScale = new Vector3(2.5f, 20.0f, 1);
    [SerializeField]
    private Vector3 sceneryContainerScale = new Vector3(14.0f, 20.0f, 1.0f);

    private void CreateSceneryRegions(int type = 0)
    {
        var bounds = CreatePlatformBound();
        if (type == 0)
        {
            sceneryConflictRegion = CreateSceneryContainerBoxCollider(bounds.center, bounds, "SceneryConflictRegion", sceneryConflictRegionScale);
            sceneryBoxContainer = CreateSceneryContainerBoxCollider(bounds.center, bounds, "SceneryContainer", sceneryContainerScale);
        }
        else
        {
            sceneryConflictRegion = CreateSceneryContainerBoxCollider(bounds.center, bounds, "SceneryConflictRegion", Vector3.one * 3.0f);
            sceneryBoxContainer = CreateSceneryContainerBoxCollider(bounds.center, bounds, "SceneryContainer", Vector3.one * 5.0f);
        }
    }
    
    private void GenerateSceneryObjects(GameObject[] sceneryObjectPrefabs,
                                        GameObjectPoolList gameObjectPoolList, int type = 0)
    {
        CreateSceneryRegions(type);
        StartCoroutine(GenerateSceneryObjectsAsync(sceneryObjectPrefabs, gameObjectPoolList));
        //Enumerable.Range(0, Random.Range(0, 20))
        //          .ForEach(x =>
        //          {
        //              var attempt = 100;
        //              var randomPoint = sceneryBoxContainer.RandomPointInBounds();
        //              while (sceneryConflictRegion.bounds.Contains(randomPoint) && attempt > 0)
        //              {
        //                  randomPoint = sceneryBoxContainer.RandomPointInBounds();
        //                  attempt--;
        //              }

        //              if (attempt > 0)
        //              {
        //                  var sceneryGameObject = InstantiateGameObject(gameObjectPoolList, sceneryObjectPrefabs.RandomElement());
        //                  sceneryGameObject.transform.position = randomPoint;
        //                  sceneryGameObject.transform.localScale = UnityEngine.Random.Range(0.2f, 3.5f) * Vector3.one;
        //              }
        //          });
    }

    private IEnumerator GenerateSceneryObjectsAsync(GameObject[] sceneryObjectPrefabs,
                                        GameObjectPoolList gameObjectPoolList)
    {
        int rand = Random.Range(0, 20);
        for(int i = 0; i < rand; i++)
        {
            var attempt = 100;
            var randomPoint = sceneryBoxContainer.RandomPointInBounds();
            while (sceneryConflictRegion.bounds.Contains(randomPoint) && attempt > 0)
            {
                randomPoint = sceneryBoxContainer.RandomPointInBounds();
                attempt--;
            }

            if (attempt > 0)
            {
                var sceneryGameObject = InstantiateGameObject(gameObjectPoolList, sceneryObjectPrefabs.RandomElement());
                sceneryGameObject.transform.position = randomPoint;
                sceneryGameObject.transform.localScale = UnityEngine.Random.Range(0.2f, 3.5f) * Vector3.one;
            }

            if (i % 5 == 0) yield return null;
        }
    }

    private Bounds CreatePlatformBound()
    {
        var min = Vector3.positiveInfinity;
        var max = Vector3.negativeInfinity;

        foreach (var boxCollider in allPlatformColliders)
        {
            var size = Vector3.Scale(boxCollider.size , boxCollider.transform.lossyScale);
            min = Vector3.Min(boxCollider.transform.position + boxCollider.center - size / 2, min);
            max = Vector3.Max(boxCollider.transform.position + boxCollider.center + size / 2, max);
        }

        var bound = new Bounds();
        bound.SetMinMax(min, max);
        return bound;
    }
    
    private BoxCollider CreateSceneryContainerBoxCollider(Vector3 position,
                                                          Bounds bounds,
                                                          string objectName,
                                                          Vector3 scale)
    {
        var go = new GameObject(objectName)
        {
            transform =
            {
                localPosition = position,
                parent = this.sceneryContainer
            },
            layer = default
        };
        var collider = go.AddComponent<BoxCollider>();
        collider.center = Vector3.zero;
        collider.size = Vector3.Scale(bounds.size, scale);
        collider.isTrigger = true;
        return collider;
    }
}