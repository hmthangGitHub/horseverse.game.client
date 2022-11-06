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

    public void SetBoxCollider(BoxCollider boxCollider)
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
        Vector3 v3Center = bounds.center;
        Vector3 v3Extents = bounds.extents;
        var v3BackTopRight =
            new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y,
                v3Center.z + v3Extents.z); // Back top right corner
        var v3FrontTopRight =
            new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y,
                v3Center.z - v3Extents.z); // Front top right corner
        var point = (v3BackTopRight + v3FrontTopRight) * 0.5f;
        point = boxCollider.transform.TransformPoint(point);
        this.transform.position = point;
    }
}
