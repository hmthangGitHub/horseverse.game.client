using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "map_settings", menuName = "HVerse/Map Settings", order = 1)]
public class MapSettings : ScriptableObject
{
    public PathCreation.PathCreator path;
    public RaceModeCameras raceModeCamera;
    public FreeCamera freeCamera;
    public WarmUpCamera warmUpCamera;
}
