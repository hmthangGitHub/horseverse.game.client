using UnityEngine;

public partial class HorseRaceFirstPersonAIDriver
{
    private void OnDrawGizmos()
    {
        DrawAllTargets();
    }
    
    private void DrawAllTargets()
    {
        if (horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData?.PredefineWayPoints == default) return;

        foreach (var x in horseRaceThirdPersonBehaviour.HorseRaceThirdPersonData.PredefineWayPoints)
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