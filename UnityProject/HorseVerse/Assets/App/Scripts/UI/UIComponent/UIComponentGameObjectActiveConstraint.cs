using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponentGameObjectActiveConstraint : MonoBehaviour
{
    public GameObject[] constraintGameObjects;

    private void OnEnable()
    {
        constraintGameObjects.ForEach(x => x.SetActive(true));
    }

    private void OnDisable()
    {
        constraintGameObjects.ForEach(x => x.SetActive(false));
    }
}
