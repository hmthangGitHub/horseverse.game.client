using Cysharp.Threading.Tasks;
using RobustFSM.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRaceState : BState
{
    HorseRaceManager horseRaceManager;
    public override async void Enter()
    {
     
        base.Enter();
        horseRaceManager ??= GameObject.Instantiate<HorseRaceManager>((await Resources.LoadAsync<HorseRaceManager>("HorseRaceManager") as HorseRaceManager));
        horseRaceManager.playerHorseIndex = this.SuperMachine.GetPreviousState<HorsePickingState>().HorseIndex;
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
