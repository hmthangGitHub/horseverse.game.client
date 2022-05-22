using RobustFSM.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjectedBState : BState, IInjectedState
{
    public IDIContainer Container => (this.SuperMachine as IFSMContainer).Container;
}
