using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    private void Awake()
    {
        targetTemplate.gameObject.SetActive(false);
    }

    [SerializeField]
    private int numberOfTargets;
    public GameObject targetTemplate;
    public GameObject[] Targets { get; private set; }
    
    public int NumberOfTargets { get => numberOfTargets; set => numberOfTargets = value; }

    public void Spawn()
    {
        var list = new List<GameObject>();
        for (int i = 0; i < numberOfTargets; i++)
        {
            var item = GameObject.Instantiate(targetTemplate, this.gameObject.transform);
            item.SetActive(true);
            list.Add(item);
        }
        Targets = list.ToArray();
    }
}
