using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingState : InjectedBHState
{
    public static long MasterMapId = 10001004;
    private HorseRaceContext horseRaceContext;
    private HorseRaceContext HorseRaceContext => horseRaceContext ??= Container.Inject<HorseRaceContext>();
    
    public override void AddStates()
    {
        base.AddStates();
        AddState<RacingMenuState>();
        AddState<RaceModeChoosingState>();
        AddState<HorseRaceActionState>();
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