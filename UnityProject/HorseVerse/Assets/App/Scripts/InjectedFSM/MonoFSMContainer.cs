using Assets.RobustFSM.Mono;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoFSMContainer : MonoFSM, IFSMContainer
{
    public IDIContainer container = default;
    public IDIContainer Container => container ??= new DIContainer();

    public override void AddStates()
    {
        base.AddStates();
    }
}
