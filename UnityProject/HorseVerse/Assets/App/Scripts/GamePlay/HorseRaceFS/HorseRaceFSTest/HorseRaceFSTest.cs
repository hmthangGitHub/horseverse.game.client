#if UNITY_EDITOR
using System;
using UnityEngine;
public class HorseRaceFSTest : MonoBehaviour
{
    public TargetGenerator targetGenerator;
    private void OnGUI()
    {
        if (GUILayout.Button("Start"))
        {
            var horseRaceFirstPersonControllers = this.GetComponentsInChildren<HorseRaceFirstPersonController>();
            horseRaceFirstPersonControllers[0].HorseRaceThirdPersonData = new HorseRaceThirdPersonData()
            {
                IsPlayer = true,
                PredefineWayPoints = default
            };
            
            horseRaceFirstPersonControllers[1].HorseRaceThirdPersonData = new HorseRaceThirdPersonData()
            {
                IsPlayer = false,
                PredefineWayPoints = targetGenerator.GenerateRandomTargets()
            };
            horseRaceFirstPersonControllers.ForEach(x => x.IsStart = true);
        }
    }
}
#endif

