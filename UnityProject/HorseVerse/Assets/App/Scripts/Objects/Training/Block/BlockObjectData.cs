using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObjectData : MonoBehaviour
{
    public enum Type {
        NORMAL,
        START,
        END,
        TURN_LEFT,
        TURN_RIGHT,
        SPLIT_TWO_TURNS,
        BEGIN_SCENE,
        END_SCENE,
    }

    [SerializeField] Type blockType; 
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [Space]
    [SerializeField] BezierCurve curve;

    public Type BlockType => blockType;
    public Transform StartPoint => startPoint;
    public Transform EndPoint => endPoint;
    public BezierCurve Curve => curve;
}
