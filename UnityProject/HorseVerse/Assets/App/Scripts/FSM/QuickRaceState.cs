using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickRaceState : InjectedBHState
{
    public static long MasterMapId = 10001004;
    private HorseRaceContext horseRaceContext;
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    
    public override void AddStates()
    {
        base.AddStates();
        AddState<QuickRaceMenuState>();
        AddState<RaceModeChoosingState>();
        AddState<HorseRaceState>();
        AddState<RaceState>();
        SetInitialState<RaceModeChoosingState>();
    }

    public override void Enter()
    {
        base.Enter();
        HorseRaceContext.GameMode = HorseGameMode.Race;
    }

    public override void Exit()
    {
        base.Exit();
        HorseRaceContext.Reset();
        horseRaceContext = default;
    }
}