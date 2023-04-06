using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IHorseRaceManager : IDisposable
{
    Transform WarmUpTarget { get; }
    IHorseRaceInGameStatus[] HorseControllers { get; }
    float NormalizedRaceTime { get;}
    int PlayerHorseIndex { get; }
    event Action OnHorseFinishTrackEvent;
    event Action OnShowResult;
    UniTask WaitToStart();
    void PrepareToRace();
    void StartRace();
    void UpdateRaceTime();
}