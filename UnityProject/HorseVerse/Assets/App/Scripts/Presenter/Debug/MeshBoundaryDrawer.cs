using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class MeshBoundaryDrawer : MonoBehaviour
{
    public static MeshBoundaryDrawer Instantiate(BoxCollider collider, Color color, Material material)
    {
        var drawer = new GameObject("MeshBoundaryDrawer").AddComponent<MeshBoundaryDrawer>();
        drawer.material = material;
        drawer.color = color;
        drawer.boxCollider = collider;
        drawer.transform.parent = collider.transform.parent;
        return drawer;
    }
    
    public Color color = Color.green;
    public Material material;
    private Vector3 v3FrontTopLeft;
    private Vector3 v3FrontTopRight;
    private Vector3 v3FrontBottomLeft;
    private Vector3 v3FrontBottomRight;
    private Vector3 v3BackTopLeft;
    private Vector3 v3BackTopRight;
    private Vector3 v3BackBottomLeft;
    private Vector3 v3BackBottomRight;

    private LineRenderer lineRenderer;
    private List<Vector3> positionList = new List<Vector3>();
    public BoxCollider boxCollider;

    private void Start()
    {
        lineRenderer = this.gameObject.AddComponent<LineRenderer>();
        lineRenderer.endWidth = 0.01f;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.material = Material.Instantiate(material);
        lineRenderer.material.color = color;
        lineRenderer.endColor = color;
        lineRenderer.startColor = color;
        lineRenderer.colorGradient.alphaKeys = new[]
        {
            new GradientAlphaKey()
            {
                alpha = 1.0f,
                time = 0.0f
            }
        };
        lineRenderer.textureMode = LineTextureMode.Tile;
    }

    void Update()
    {
        CalcPositons();
        DrawBox();
    }

    void CalcPositons()
    {
        if(boxCollider == default) return;
        var bounds = new Bounds(boxCollider.center,
            boxCollider.size);

        Vector3 v3Center = bounds.center;
        Vector3 v3Extents = bounds.extents;

        v3FrontTopLeft =
            new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y,
                v3Center.z - v3Extents.z); // Front top left corner
        v3FrontTopRight =
            new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y,
                v3Center.z - v3Extents.z); // Front top right corner
        v3FrontBottomLeft =
            new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y,
                v3Center.z - v3Extents.z); // Front bottom left corner
        v3FrontBottomRight =
            new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y,
                v3Center.z - v3Extents.z); // Front bottom right corner
        v3BackTopLeft =
            new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y,
                v3Center.z + v3Extents.z); // Back top left corner
        v3BackTopRight =
            new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y,
                v3Center.z + v3Extents.z); // Back top right corner
        v3BackBottomLeft =
            new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y,
                v3Center.z + v3Extents.z); // Back bottom left corner
        v3BackBottomRight =
            new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y,
                v3Center.z + v3Extents.z); // Back bottom right corner

        v3FrontTopLeft = boxCollider.transform.TransformPoint(v3FrontTopLeft);
        v3FrontTopRight = boxCollider.transform.TransformPoint(v3FrontTopRight);
        v3FrontBottomLeft = boxCollider.transform.TransformPoint(v3FrontBottomLeft);
        v3FrontBottomRight = boxCollider.transform.TransformPoint(v3FrontBottomRight);
        v3BackTopLeft = boxCollider.transform.TransformPoint(v3BackTopLeft);
        v3BackTopRight = boxCollider.transform.TransformPoint(v3BackTopRight);
        v3BackBottomLeft = boxCollider.transform.TransformPoint(v3BackBottomLeft);
        v3BackBottomRight = boxCollider.transform.TransformPoint(v3BackBottomRight);
    }

    void DrawBox()
    {
        //if (Input.GetKey (KeyCode.S)) {
        positionList.Clear();
        DrawLine(v3FrontTopLeft, v3FrontTopRight, color);
        DrawLine(v3FrontTopRight, v3FrontBottomRight, color);
        DrawLine(v3FrontBottomRight, v3FrontBottomLeft, color);
        DrawLine(v3FrontBottomLeft, v3FrontTopLeft, color);
        
        DrawLine(v3BackTopLeft, v3BackTopRight, color);
        DrawLine(v3BackTopRight, v3BackBottomRight, color);
        DrawLine(v3BackBottomRight, v3BackBottomLeft, color);
        DrawLine(v3BackBottomLeft, v3BackTopLeft, color);

        DrawLine(v3FrontTopLeft, v3BackTopLeft, color);
        DrawLine(v3FrontTopRight, v3BackTopRight, color);
        DrawLine(v3FrontBottomRight, v3BackBottomRight, color);
        DrawLine(v3FrontBottomLeft, v3BackBottomLeft, color);
        lineRenderer.positionCount = positionList.Count;
        lineRenderer?.SetPositions(positionList.ToArray());
        //}
    }

    private void DrawLine(Vector3 p0, Vector3 p1, Color p2)
    {
        positionList.Add(p0);
        positionList.Add(p1);
    }
}