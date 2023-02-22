using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using RuntimeHandle;
using UnityEngine;

public class TrapEditorRollingStone : TrapEditorBase
{
    [System.Serializable]
    public class Entity
    {
        public Position Target;
        public Position Direction;
        public Position Trigger;
        public int TriggerSize;
        public bool TriggerZoneFullBlock;
    }

    private Entity entity;

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

    private Entity getEntity()
    {
        if (entity == default) entity = new Entity();
        entity.Target = Position.FromVector3(Target.transform.localPosition);
        entity.Direction = Position.FromVector3(Direction.transform.localPosition);
        entity.Trigger = Position.FromVector3(Trigger.transform.localPosition);
        entity.TriggerSize = TriggerSize;
        entity.TriggerZoneFullBlock = TriggerZoneFullBlock;
        return entity;
    }

    public override string GetExtraData()
    {
        return JsonConvert.SerializeObject(getEntity()).Replace(",", "..."); ;
    }
}
