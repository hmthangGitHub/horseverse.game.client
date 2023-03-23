using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObjectData : MonoBehaviour
{
    [SerializeField] TYPE_OF_BLOCK blockType; 
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [Space]
    [SerializeField] BezierCurve curve;
    [Space, Header("Box Colliders")]
    [SerializeField] List<BoxCollider> boxColliders;


    public TYPE_OF_BLOCK BlockType => blockType;
    public Transform StartPoint => startPoint;
    public Transform EndPoint => endPoint;
    public BezierCurve Curve => curve;
    public List<BoxCollider> BoxColliders => boxColliders;

    public void Init()
    {
        if (curve != default) curve.init();
    }
}
