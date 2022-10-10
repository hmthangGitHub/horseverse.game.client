using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class MeshPathContainer : MonoBehaviour
{
    public enum PathType
    {
        Cloud,
        Air
    }

    public PathType pathType;
    public PathCreator pathCreator;
}
