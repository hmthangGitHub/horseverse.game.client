using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingState : InjectedBHState
{
    public static long MasterMapId = 1003;
    private HorseRaceContext horseRaceContext;
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    
    public override void AddStates()
    {
        base.AddStates();
        AddState<RacingThirdPersonMenuState>();
        AddState<RaceModeChoosingState>();
        AddState<RacingHistoryState>();
        SetInitialState<RaceModeChoosingState>();
    }

    public override void Enter()
    {
        HorseRaceContext.GameMode = HorseGameMode.Race;
        HorseRaceContext.RaceMatchDataContext = new RaceMatchDataContext();
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        HorseRaceContext.Reset();
        horseRaceContext = default;
    }
}