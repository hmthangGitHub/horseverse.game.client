#if UNITY_EDITOR
using UnityEngine;

public partial class HorseRaceThirdPersonBehaviour
{
    [ContextMenu("Start")]
    public void StartGameDebug()
    {
        StartRace(0.0f);
    }
}
#endif