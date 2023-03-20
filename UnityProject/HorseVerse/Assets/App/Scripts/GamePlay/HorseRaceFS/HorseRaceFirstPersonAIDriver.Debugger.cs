using UnityEngine;

public partial class HorseRaceFirstPersonAIDriver
{
    private void OnDrawGizmos()
    {
        DrawAllTargets();
    }
    
    private void DrawAllTargets()
    {
        if (horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData?.HorseRaceThirdPersonStats?.PredefineWayPoints == default) return;

        foreach (var x in horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.HorseRaceThirdPersonStats.PredefineWayPoints)
        {
            Gizmos.DrawSphere(x, 0.5f);
        }
    }

    [ContextMenu("ChangeTarget")]
    private void ChangeTargetTest()
    {
        ChangeTarget(); 
    }
}