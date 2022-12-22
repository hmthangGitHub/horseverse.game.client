using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDesignPin : MonoBehaviour
{
    public static LevelDesignPin Instantiate(BoxCollider collider)
    {
        var pin = new GameObject("Pin").AddComponent<LevelDesignPin>();
        pin.SetBoxCollider(collider);
        pin.transform.parent = collider.transform.parent;
        return pin;
    }
    
    private BoxCollider boxCollider;

    private void SetBoxCollider(BoxCollider boxCollider)
    {
        this.boxCollider = boxCollider;
    }

    private void Update()
    {
        UpdatePositionToLeft();
    }

    private void UpdatePositionToLeft()
    {
        var bounds = new Bounds(boxCollider.center, boxCollider.size);
        Vector3 v3CenterTop = bounds.center + new Vector3(0, bounds.extents.y, 0) ;
        var point = boxCollider.transform.TransformPoint(v3CenterTop);
        this.transform.position = point;
    }
}
