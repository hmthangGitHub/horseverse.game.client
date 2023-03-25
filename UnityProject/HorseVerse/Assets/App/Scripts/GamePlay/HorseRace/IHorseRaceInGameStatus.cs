public interface IHorseRaceInGameStatus
{
    bool IsPlayer { get; }
    float CurrentRaceProgressWeight { get; }
    int InitialLane { get; }
    string Name { get; }
}