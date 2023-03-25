using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    [SerializeField] private LayerMask layer;
    [SerializeField] private Transform origin;
    [SerializeField] private float length;
    [SerializeField] private Vector3 direction;
    public bool IsInterSect { get; private set; }

    private void FixedUpdate()
    {
        IsInterSect = Physics.Raycast(origin.position, direction, length, layer.value);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(origin.position, direction * length, IsInterSect ? Color.red : Color.green);
    }
}
