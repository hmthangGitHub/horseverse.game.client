#if UNITY_EDITOR
using System;
using UnityEngine;
public class HorseRaceFSTest : MonoBehaviour
{
    public TargetGenerator targetGenerator;
    public HorseRaceThirdPersonMasterData[] dataTests;
    private void OnGUI()
    {
        if (GUILayout.Button("Set",  GUILayout.Width(300), GUILayout.Height(300)))
        {
            dataTests[1].PredefineWayPoints = targetGenerator.GenerateRandomTargetsWithNoise();
            dataTests.ForEach(x => x.InitialLane = UnityEngine.Random.Range(0, 8));
            var horseRaceFirstPersonControllers = this.GetComponentsInChildren<HorseRaceThirdPersonBehaviour>();
            horseRaceFirstPersonControllers[0].HorseRaceThirdPersonMasterData = dataTests[0];
            horseRaceFirstPersonControllers[1].HorseRaceThirdPersonMasterData =  dataTests[1];
        }

        if (GUILayout.Button("Start", GUILayout.Width(300), GUILayout.Height(300)))
        {
            var horseRaceFirstPersonControllers = this.GetComponentsInChildren<HorseRaceThirdPersonBehaviour>();
            horseRaceFirstPersonControllers.ForEach(x => x.StartRace(UnityEngine.Random.Range(0.0f, 1.0f)));
        }
    }
}
#endif

