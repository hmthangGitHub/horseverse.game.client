using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlatformModular
{

    private void GenerateTraps(Trap[] trapList,
                                  GameObject[] trapPrefabs)
    {
        Debug.Log("TRAP LIST " + trapList.Length);
        trapList.ForEach(x =>
        {
            var trapPrefab = trapPrefabs.FirstOrDefault(trapP => trapP.name == $"{x.type}_{x.id}");
            _cacheTrap.Add(CreateTrap(x.type, x.extraData, trapPrefab, x.localPosition));
        });
    }

    private IEnumerator GenerateTrapAsync(Trap[] trapList,
                                  GameObject[] trapPrefabs)
    {
        int len = trapList.Length;
        Debug.Log("TRAP LIST " + len);
        for (int i = 0; i < len; i++)
        {
            var x = trapList[i];
            var trapPrefab = trapPrefabs.FirstOrDefault(trapP => trapP.name == $"{x.type}_{x.id}");
            _cacheTrap.Add(CreateTrap(x.type ,x.extraData, trapPrefab, x.localPosition));
            
            if (i % 5 == 0) yield return null;
        }
    }

    private GameObject CreateTrap(string type, string data, GameObject trapPrefab,
                                     Position localPosition)
    {
        var prefab = trapPrefab
                     .transform.Cast<Transform>()
                     .RandomElement();
        var trap = Instantiate(prefab.gameObject, transform);//(GameObject)pool.GetOrInstante(prefab.gameObject, transform);
        trap.name = prefab.name;
        trap.transform.localPosition = localPosition.ToVector3();
        if (string.Equals(type, TrapEditor.TYPE.ROLLING_ROCK.ToString()))
        {
            var comp = trap.GetComponent<TrainingTrapBall>();
            if (comp != default)
            {
                comp.SetEntity(comp.ParseData(data));
            }
        }
        else if (string.Equals(type, TrapEditor.TYPE.WOODEN_SPIKE.ToString()))
        {

        }
        return trap;
    }



}
