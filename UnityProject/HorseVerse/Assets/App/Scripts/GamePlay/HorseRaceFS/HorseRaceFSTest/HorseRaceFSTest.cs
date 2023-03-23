#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
public class HorseRaceFSTest : MonoBehaviour
{
    public TargetGenerator targetGenerator;
    public HorseRaceThirdPersonData[] dataTests;
    private void OnGUI()
    {
        if (GUILayout.Button("Set",  GUILayout.Width(300), GUILayout.Height(300)))
        {
            dataTests.Skip(1).ForEach(x => x.PredefineWayPoints = targetGenerator.GenerateRandomTargetsWithNoise());
            dataTests.ForEach(x => x.InitialLane = UnityEngine.Random.Range(0, 8));
            var horseRaceFirstPersonControllers = this.GetComponentsInChildren<HorseRaceThirdPersonBehaviour>();
            horseRaceFirstPersonControllers.ForEach((x, i) => horseRaceFirstPersonControllers[i].HorseRaceThirdPersonData = dataTests[i]);
        }

        if (GUILayout.Button("Start", GUILayout.Width(300), GUILayout.Height(300)))
        {
            var horseRaceFirstPersonControllers = this.GetComponentsInChildren<HorseRaceThirdPersonBehaviour>();
            horseRaceFirstPersonControllers.ForEach(x => x.StartRace(UnityEngine.Random.Range(0.0f, 1.0f)));
        }
    }
}
#endif

