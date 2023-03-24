#if UNITY_EDITOR
using UnityEngine;

public partial class HorseRaceFirstPersonAIDriver
{
    private Vector3 lastFrame;

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

    private void LateUpdate()
    {
        if ((lastFrame - horseRaceThirdPersonBehaviour.transform.position).magnitude <= Mathf.Epsilon)
        {
            int x = 10;
        }
        lastFrame = horseRaceThirdPersonBehaviour.transform.position;
    }
}
#endif