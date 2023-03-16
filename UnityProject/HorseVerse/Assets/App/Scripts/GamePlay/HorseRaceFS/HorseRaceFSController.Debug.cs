#if UNITY_EDITOR
using UnityEngine;

public partial class HorseRaceFirstPersonController
{
    [ContextMenu("Start")]
    public void StartGameDebug()
    {
        IsStart = true;
    }
}
#endif