using System.Collections;
using System.Collections.Generic;
using RuntimeHandle;
using UnityEngine;

public class TrapEditorRollingStone : MonoBehaviour
{
    public GameObject Target;
    public GameObject Direction;
    public GameObject Trigger;
    public int TriggerSize;
    public bool TriggerZoneFullBlock;


    void OnEnable()
    {
        RestoreToDefault();
    }

    public void RestoreToDefault()
    {
        OffAll();
        Target.SetActive(true);
    }

    private void OffAll()
    {
        Direction.SetActive(false);
        Trigger.SetActive(false);
        var kd = Direction.GetComponent<RuntimeTransformHandle>();
        if (kd) Destroy(kd);
        var kt = Trigger.GetComponent<RuntimeTransformHandle>();
        if (kd) Destroy(kt);
    }

    public void ActiveDirection()
    {
        OffAll();
        Direction.SetActive(true);
        var comp = Direction.AddComponent<RuntimeTransformHandle>();
        comp.axes = HandleAxes.XZ;
    }

    public void ActiveTrigger()
    {
        OffAll();
        Trigger.SetActive(true);
        var comp = Trigger.AddComponent<RuntimeTransformHandle>();
        comp.axes = HandleAxes.XZ;
    }
}
