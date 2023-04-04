using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class PlatformModular
{

    private void GenerateSubObjects(SubObject[] subObjectList,
                                  GameObject[] subObjectPrefabs)
    {
        if (subObjectList == default || subObjectPrefabs == default) return;
        subObjectList.ForEach(x =>
        {
            var prefab = subObjectPrefabs.FirstOrDefault(xx => xx.name == $"{x.type}");
            CreateSubObject(prefab, x.localPosition);
        });
    }
    private IEnumerator GenerateSubObjectsAsync(SubObject[] subObjectList,
                                  GameObject[] subObjectPrefabs)
    {
        if (subObjectList == default || subObjectPrefabs == default) yield break;
        int len = subObjectList.Length;
        for (int i = 0; i < len; i++)
        {
            var x = subObjectList[i];
            var prefab = subObjectPrefabs.FirstOrDefault(xx => xx.name == $"{x.type}");
            CreateSubObject(prefab, x.localPosition);

            if (i % 5 == 0) yield return null;
        }
    }

    private GameObject CreateSubObject(GameObject trapPrefab,
                                     Position localPosition)
    {
        var prefab = trapPrefab
                     .transform.Cast<Transform>()
                     .RandomElement();
        var obj = Instantiate(prefab.gameObject, transform);//(GameObject)pool.GetOrInstante(prefab.gameObject, transform);
        obj.name = prefab.name;
        obj.transform.localPosition = localPosition.ToVector3();
        return obj;
    }
}
