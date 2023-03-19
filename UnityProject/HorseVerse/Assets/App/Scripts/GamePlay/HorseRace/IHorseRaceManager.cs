using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IHorseRaceManager : IDisposable
{
    Transform WarmUpTarget { get; }
    HorseController[] HorseControllers { get; }
    float RaceTime { get;}
    int PlayerHorseIndex { get; }
    event Action OnFinishTrackEvent;
    UniTask WaitToStart();
    void PrepareToRace();
    void StartRace();
}