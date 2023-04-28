using System.Linq;
using UnityEngine;
using System.Collections;

public partial class PlatformModular
{
    [SerializeField]
    private Vector3 sceneryConflictRegionScale = new Vector3(2.5f, 50.0f, 1.5f);
    [SerializeField]
    private Vector3 sceneryContainerScale = new Vector3(14.0f, 20.0f, 1.0f);

    private Vector3 sceneryTurnConflictScale = new Vector3(1, 20, 1);
    private Vector3 sceneryTurnContainerScale = new Vector3(2, 20, 2);

    public void CreateSceneryRegions(int type = 0)
    {
        var bounds = CreatePlatformBound();
        if (type == 0)
        {
            sceneryConflictRegion = CreateSceneryContainerBoxCollider(bounds.center, bounds, "SceneryConflictRegion", sceneryConflictRegionScale);
            sceneryBoxContainer = CreateSceneryContainerBoxCollider(bounds.center, bounds, "SceneryContainer", sceneryContainerScale);
            CreatePositionContainer(-1, "PositionContainerLeft");
            CreatePositionContainer(1, "PositionContainerRight");
        }
        else
        {
            sceneryConflictRegion = CreateSceneryContainerBoxCollider(bounds.center, bounds, "SceneryConflictRegion", sceneryTurnConflictScale);
            sceneryBoxContainer = CreateSceneryContainerBoxCollider(bounds.center, bounds, "SceneryContainer", sceneryTurnContainerScale);
        }
    }

    private void CreatePositionContainer(int direction,
                                         string name)
    {
        var min = sceneryBoxContainer.transform.position + direction * (sceneryBoxContainer.size / 2);
        var minConflict = sceneryConflictRegion.transform.position + direction * (sceneryConflictRegion.size / 2);
        var center = new Vector3(((min + minConflict) / 2).x, sceneryBoxContainer.transform.position.y, sceneryBoxContainer.transform.position.z);
        var extend = new Vector3((min.x - minConflict.x) / 2, 
            sceneryBoxContainer.bounds.extents.y,
            sceneryBoxContainer.bounds.extents.z);
        
        var bounds = new Bounds()
        {
            center = center,
            extents = extend
        };
        
        var positionContainerLeft = new GameObject(name)
        {
            transform =
            {
                localPosition = bounds.center,
                parent = this.sceneryContainer
            },
            layer = default
        };

        var boxCollider = positionContainerLeft.AddComponent<BoxCollider>();
        boxCollider.size = extend * 2;
        boxCollider.center = Vector3.zero;
        boxCollider.size = bounds.size;
        boxCollider.isTrigger = true;
        
        sceneryPositionContainers.Add(boxCollider);
    }

    private void GenerateSceneryObjects(GameObject[] sceneryObjectPrefabs,
                                        GameObjectPoolList gameObjectPoolList, int type = 0)
    {
        CreateSceneryRegions(type);
        StartCoroutine(GenerateSceneryObjectsAsync(sceneryObjectPrefabs, gameObjectPoolList));
    }

    private IEnumerator GenerateSceneryObjectsAsync(GameObject[] sceneryObjectPrefabs,
                                        GameObjectPoolList gameObjectPoolList)
    {
        if (sceneryObjectPrefabs.Length == 0) yield break;
        int rand = Random.Range(0, 20);
        yield return null;
        yield return null;
        for(int i = 0; i < rand; i++)
        {
            var attempt = 100;
            var randomPoint = GetRandomSceneryPosition();
            
            var sceneryGameObject = GetRandomSceneryObject(sceneryObjectPrefabs, gameObjectPoolList);
            sceneryGameObject.transform.localPosition = randomPoint;

            if (sceneryGameObject.TryGetComponent<BoxCollider>(out var boxCollider))
            {
                var center = boxCollider.transform.position + boxCollider.center;
                var bound = new Bounds
                {
                    center = center,
                    extents = Vector3.Scale(boxCollider.bounds.extents, sceneryGameObject.transform.localScale)
                };
                while (bound.Intersects(sceneryConflictRegion.bounds) && attempt > 0)
                {
                    randomPoint =  GetRandomSceneryPosition();
                    sceneryGameObject.transform.localPosition = randomPoint;
                    attempt--;
                }
            }
            else
            {
                while (sceneryConflictRegion.bounds.Contains(randomPoint) && attempt > 0)
                {
                    randomPoint = sceneryPositionContainers.RandomElement().RandomPointInBounds();
                    attempt--;
                }
            }
            
            if (attempt > 0)
            {
                sceneryGameObject.transform.position = randomPoint;
            }
            else
            {
                sceneryGameObject.SetActive(false);
            }

            if (i % 5 == 0) yield return null;
        }
    }

    private Vector3 GetRandomSceneryPosition()
    {
        return sceneryPositionContainers.RandomElement().RandomPointInBounds();
    }

    private GameObject GetRandomSceneryObject(GameObject[] sceneryObjectPrefabs,
                                              GameObjectPoolList gameObjectPoolList)
    {
        var sceneryGameObject = InstantiateGameObject(gameObjectPoolList, sceneryObjectPrefabs.RandomElement());
        if (sceneryGameObject.TryGetComponent<SceneryObjectReference>(out var sceneryObjectReference))
        {
            sceneryGameObject.transform.localScale = Random.Range(sceneryObjectReference.scaleRange.x, sceneryObjectReference.scaleRange.y) * Vector3.one;
        }
        else
        {
            sceneryGameObject.transform.localScale = Random.Range(0.2f, 3.5f) * Vector3.one;
        }
        return sceneryGameObject;
    }

    private IEnumerator GenerateSceneryObjectsAsync(GameObject[] sceneryObjectPrefabs,
                                                    GameObjectPoolList gameObjectPoolList, BoxCollider[] boxColliders)
    {
        int rand = Random.Range(0, 20);
        for (int i = 0; i < rand; i++)
        {
            var attempt = 100;
            var randomPoint = sceneryBoxContainer.RandomPointInBounds();

            while (attempt > 0)
            {
                bool isConflict = false;
                for(int j = 0; j < boxColliders.Length; j++)
                {
                    if (boxColliders[j].bounds.Contains(randomPoint))
                    {
                        randomPoint = sceneryBoxContainer.RandomPointInBounds();
                        isConflict = true;
                        break;
                    }
                }
                attempt--;
                if (!isConflict) break;
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

    public IEnumerator GenerateSceneryObjects(BoxCollider[] boxColliders, GameObject[] sceneryObjectPrefabs,
                                        GameObjectPoolList gameObjectPoolList)
    {
        yield return GenerateSceneryObjectsAsync(sceneryObjectPrefabs, gameObjectPoolList, boxColliders);
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
        go.transform.localRotation = Quaternion.identity;
        float currentAngle = Vector3.SignedAngle(go.transform.forward, Vector3.forward, Vector3.up);
        Vector3 _scale = Quaternion.Euler(0, currentAngle, 0) * scale;
        var collider = go.AddComponent<BoxCollider>();
        collider.center = Vector3.zero;
        collider.size = Vector3.Scale(bounds.size, scale);
        collider.isTrigger = true;
        return collider;
    }
}