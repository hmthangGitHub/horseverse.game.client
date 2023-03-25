using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRaceFSIgnoreCollider : MonoBehaviour
{
    [SerializeField]
    private Collider[] ignoreColliders;

    private void Start()
    {
        for (var i = 0; i < ignoreColliders.Length; i++)
        {
            for (var j = i + 1; j < ignoreColliders.Length; j++)
            {
                Physics.IgnoreCollision(ignoreColliders[i], ignoreColliders[j], true);
            }
        }
    }
}
