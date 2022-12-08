using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHolder : MonoBehaviour
{
    [SerializeField]
    private Transform holder;

    public static Transform Holder { get; private set; }

    private void Awake()
    {
        Holder = holder;
    }
}
