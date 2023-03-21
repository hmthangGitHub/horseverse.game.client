using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class HorseRaceThirdPersonListContainer : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup horseGroup;
    [SerializeField] private Transform horsesContainer;
    
    public Transform WarmUpTarget => horseGroup.Transform;
    public Transform HorsesContainer => horsesContainer;
    public CinemachineTargetGroup HorseGroup => horseGroup;
}
