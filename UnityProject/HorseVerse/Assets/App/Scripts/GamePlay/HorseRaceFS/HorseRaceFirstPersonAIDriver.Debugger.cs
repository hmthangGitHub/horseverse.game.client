using UnityEngine;

public partial class HorseRaceFirstPersonAIDriver
{
    private void OnDrawGizmos()
    {
        if (!horseRaceFirstPersonController.IsStart) return;
        DrawAllTargets();
    }
    
    private void DrawAllTargets()
    {
        if (horseRaceFirstPersonController.HorseRaceThirdPersonData.PredefineWayPoints == default) return;

        foreach (var x in horseRaceFirstPersonController.HorseRaceThirdPersonData.PredefineWayPoints)
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