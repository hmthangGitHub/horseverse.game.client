using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDesignPin : MonoBehaviour
{
    public static LevelDesignPin Instantiate(Collider collider)
    {
        var pin = new GameObject("Pin").AddComponent<LevelDesignPin>();
        pin.SetBoxCollider(collider);
        pin.transform.parent = collider.transform;
        return pin;
    }
    
    private Collider boxCollider;

    private void SetBoxCollider(Collider boxCollider)
    {
        this.boxCollider = boxCollider;
    }

    private void Update()
    {
        UpdatePositionToLeft();
    }

    private void UpdatePositionToLeft()
    {
        var bounds = boxCollider.bounds;
        this.transform.localPosition = new Vector3(0, bounds.extents.y, 0);
    }
}
