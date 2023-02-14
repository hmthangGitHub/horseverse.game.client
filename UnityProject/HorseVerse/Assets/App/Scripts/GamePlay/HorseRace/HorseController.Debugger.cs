#if UNITY_EDITOR
using System.Linq;
using UnityEngine;

public partial class HorseController
{
    private GameObject target;
    public GameObject Target => target ??= new GameObject($"{gameObject.name}_Target");
    private Color? color;
    private Color Color => color ??= UnityEngine.Random.ColorHSV();

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && UnityEditor.Selection.gameObjects.Contains(this.gameObject))
        {
            DrawAllTargets();
            DrawLineToCurrentTarget();
        }
    }

    private void DrawAllTargets()
    {
        if (PredefineTargets != default)
        {
            for (int i = 0; i < PredefineTargets.Length; i++)
            {
                Gizmos.color = i <= FinishIndex ? Color : Color.red;
                var x = PredefineTargets[i];
                Gizmos.DrawSphere(x.target, 0.5f);
                
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(x.target, x.target + x.rotation * Vector3.forward * 1.0f);

                UnityEditor.Handles.color = Color;
                UnityEditor.Handles.Label(this.transform.position, $"{i}");
            }
        }
    }

    private void DrawLineToCurrentTarget()
    {
        if (currentTargetIndex >= 0)
        {
            var delta = PredefineTargets[currentTargetIndex]
                .target - transform.position;
            var angle = Vector3.SignedAngle(delta, transform.forward, Vector3.up);
            UnityEditor.Handles.Label(this.transform.position + Vector3.back, $"angle {angle}");
            Debug.DrawLine(this.transform.position, Target.transform.position, Color);
            Debug.DrawLine(this.transform.position, this.transform.position + navMeshAgent.velocity.normalized * 5.0f,
                Color);
        }
    }
}
#endif