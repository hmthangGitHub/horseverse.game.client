using System;
using Assets.RobustFSM.Mono;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoFSMContainer : MonoFSM, IFSMContainer
{
    private IDIContainer container = default;
    public IDIContainer Container => container ??= new DIContainer();

    public void Reset()
    {
        container = default;
    }
}
