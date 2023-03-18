using UnityEngine;

public partial class HorseRaceFirstPersonAIDriver
{
    private void OnDrawGizmos()
    {
        DrawAllTargets();
    }
    
    private void DrawAllTargets()
    {
        if (horseRaceThirdPersonBehaviour.HorseRaceThirdPersonMasterData?.PredefineWayPoints == default) return;

        foreach (var x in horseRaceThirdPersonBehaviour.HorseRaceThirdPersonMasterData.PredefineWayPoints)
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